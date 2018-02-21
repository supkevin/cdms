using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using CDMS.Model.ViewModel;
using CDMS.Model;

namespace CDMS.Service
{
    /*所有代碼資料集中在此*/
    public class GlobalService : IGlobalService
    {
        private static MemoryCache _Cache = MemoryCache.Default;
        private static int _CacheDuration = 60000; //second 

        private readonly IUnitOfWork _UnitOfWork;
        private readonly IRepository<Model.Code> _CodeRepository;
        private readonly IRepository<Model.Company> _Company;
        private readonly IRepository<Model.Product> _Product;
        private readonly IRepository<Model.User> _User;
        private readonly IRepository<Model.Menu> _Menu;
        private readonly IRepository<Model.Brand> _Brand;
        private readonly IRepository<Model.BankAccount> _BankAccount;

        public GlobalService(IUnitOfWork unitofwork,
            IRepository<Model.Code> repository,
            IRepository<Model.Company> company,
            IRepository<Model.Product> product,
            IRepository<Model.User> user,
            IRepository<Model.Menu> menu,
            IRepository<Model.Brand> brand,
            IRepository<Model.BankAccount> bankAccount
            )
        {
            this._UnitOfWork = unitofwork;
            this._CodeRepository = repository;
            this._Company = company;
            this._Product = product;
            this._User = user;
            this._Menu = menu;
            this._Brand = brand;
            this._BankAccount = bankAccount;
        }

        private CacheItemPolicy CreateCacheItemPolicy()
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now.AddMilliseconds(_CacheDuration);
            return policy;
        }
        private IQueryable<BankAccount> GetActivateBankAccount()
        {
            return this._BankAccount.GetAll()
                .Where(x => x.Activate == YesNo.Yes.Value);
        }

        private IQueryable<Brand> GetActivateBrand()
        {
            return this._Brand.GetAll()
                .Where(x => x.Activate == YesNo.Yes.Value);
        }

        private IQueryable<Code> GetActivateCode()
        {
            return this._CodeRepository.GetAll()
                .Where(x => x.Activate == YesNo.Yes.Value);
        }
      
        private IQueryable<User> GetActivateUser()
        {
            return this._User.GetAll()
                .Where(x => x.Activate == YesNo.Yes.Value);
        }

        private IQueryable<Product> GetActivateProduct()
        {
            return this._Product.GetAll()
                .Where(x => x.Activate == YesNo.Yes.Value);
        }

        private IQueryable<Company> GetActivateCompany()
        {
            return this._Company.GetAll()
                .Where(x => x.Activate == YesNo.Yes.Value);
        }

        // 銀行帳號
        public HashSet<MyCodeValue> GetBankAccountList()
        {
            string key = "BANK_ACCOUNT";

            if (null == _Cache[key])
            {
                HashSet<MyCodeValue> categories = new HashSet<MyCodeValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                    this.GetActivateBankAccount()
                    .OrderByDescending(x => x.Activate == YesNo.Yes.Value)
                    .Select(x => new MyCodeValue
                    {
                        Text = x.BankName + "-" + x.AccountID,
                        Value = x.SeqNo,
                        Disabled = x.Activate != YesNo.Yes.Value
                    }
                    ).ToList();

                categories = new HashSet<MyCodeValue>(query);
                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyCodeValue>)_Cache[key];
        }

        // 品牌
        public HashSet<MyTextValue> GetBrandList()
        {
            string key = "BRAND";

            if (null == _Cache[key])
            {
                HashSet<MyTextValue> categories = new HashSet<MyTextValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                    this.GetActivateBrand()
                    .OrderByDescending(x=> x.Activate == YesNo.Yes.Value)                    
                    .Select(x => new MyTextValue
                    {
                        Text = x.BrandName,
                        Value = x.BrandID,
                        Disabled = x.Activate != YesNo.Yes.Value
                    }
                    ).ToList();

                categories = new HashSet<MyTextValue>(query);
                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyTextValue>)_Cache[key];
        }

        // 公司
        public Dictionary<string, String> GetCompanyShortNameList()
        {
            string key = "COMPANY";

            if (null == _Cache[key])
            {
                Dictionary<String, String> categories = new Dictionary<String, String>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                categories =
                    this._Company.GetAll()
                    .OrderBy(x => x.CompanyID)
                    .ToDictionary(x => x.CompanyID, x => x.ShortName);

                _Cache.Set(key, categories, policy);
            }

            return (Dictionary<string, String>)_Cache[key];
        }

        public Dictionary<string, MenuViewModel> GetMenuList()
        {
            string key = "MENU";

            if (null == _Cache[key])
            {
                Dictionary<String, MenuViewModel> categories = new Dictionary<String, MenuViewModel>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                categories =
                    (
                    from u in this._Menu.GetAll().Where(x => x.ParentMenuID != null)
                    join p in this._Menu.GetAll().Where(x => x.ParentMenuID == null)
                    on u.ParentMenuID equals p.MenuID into g
                    from p in g.DefaultIfEmpty()
                    select new MenuViewModel
                    {
                        Name = u.MenuName,
                        ParentName = p == null ? "" : p.MenuName,
                        Path = u.MenuPath
                    })
                    .ToDictionary(x => x.Path, x => x);

                _Cache.Set(key, categories, policy);
            }

            return (Dictionary<string, MenuViewModel>)_Cache[key];
        }

        // 條件
        public Dictionary<string, String> GetConditionKindList()
        {            
            string key = CodeType.Condition.Value;
                    
            if (null == _Cache[key])
            {
                Dictionary<String, String> categories = new Dictionary<String, String>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                categories =
                    this.GetActivateCode().Where(x => x.CodeType == key)
                    .OrderByDescending(x => x.Activate == YesNo.Yes.Value)
                    .ToDictionary(x => x.CodeValue, x => x.CodeName);

                _Cache.Set(key, categories, policy);
            }

            return (Dictionary<string, String>)_Cache[key];
        }

        // 客戶
        public HashSet<MyTextValue> GetCustomerList()
        {
            string key = "CUSTOMER";

            if (null == _Cache[key])
            {
                HashSet<MyTextValue> categories = new HashSet<MyTextValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                    this.GetActivateCompany()
                    .Where(x => x.CompanyKindID == CompanyType.Both.Value
                             || x.CompanyKindID == CompanyType.Customer.Value)
                    .OrderByDescending(x => x.Activate == YesNo.Yes.Value)
                    .ThenBy(x => x.CompanyID)
                    .Select(x => new MyTextValue
                    {
                        Text = x.ShortName,
                        Value = x.CompanyID,
                        Disabled = x.Activate != YesNo.Yes.Value
                    }
                    ).ToList();

                categories = new HashSet<MyTextValue>(query);
                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyTextValue>)_Cache[key];
        }

        // 幣別
        public Dictionary<string, String> GetCurrencyKindList()
        {            
            string key = CodeType.Currency.Value;

            if (null == _Cache[key])
            {
                Dictionary<String, String> categories = new Dictionary<String, String>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                categories =
                    this.GetActivateCode()
                    .Where(x => x.CodeType == key)
                    .OrderByDescending(x => x.Activate == YesNo.Yes.Value)
                    .ToDictionary(x => x.CodeValue, x => x.CodeName);

                _Cache.Set(key, categories, policy);
            }

            return (Dictionary<string, String>)_Cache[key];
        }

        // 部門代碼
        public HashSet<MyTextValue> GetDepartmentList()
        {
            string key = CodeType.Department.Value;

            if (null == _Cache[key])
            {                
                HashSet<MyTextValue> categories = new HashSet<MyTextValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                    this.GetActivateCode().Where(x => x.CodeType == key)
                    .OrderByDescending(x=> x.Activate == YesNo.Yes.Value)                    
                    .ThenBy(x => x.SortOrder)                                        
                    .Select(x => new MyTextValue
                     {
                         Text = x.CodeName,
                         Value = x.CodeValue,
                         Disabled = x.Activate != YesNo.Yes.Value
                     }
                    ).ToList();

                categories = new HashSet<MyTextValue>(query); 
                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyTextValue>)_Cache[key];
        }

        // 消息類別
        public HashSet<MyTextValue> GetNewsTypeList()
        {            
            string key = CodeType.NewsType.Value;

            if (null == _Cache[key])
            {
                HashSet<MyTextValue> categories = new HashSet<MyTextValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                    this.GetActivateCode().Where(x => x.CodeType == key)
                    .OrderByDescending(x=> x.Activate == YesNo.Yes.Value)
                    .ThenBy(x => x.SortOrder)
                    .Select(x => new MyTextValue
                    {
                        Text = x.CodeName,
                        Value = x.CodeValue,
                        Disabled = x.Activate != YesNo.Yes.Value
                    }
                    ).ToList();

                categories = new HashSet<MyTextValue>(query);
                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyTextValue>)_Cache[key];
        }
      
        // 產品
        public HashSet<MyTextValue> GetProductList()
        {
            string key = "PRODUCT";

            if (null == _Cache[key])
            {
                HashSet<MyTextValue> categories = new HashSet<MyTextValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                     this.GetActivateProduct()                    
                    .OrderByDescending(x => x.Activate == YesNo.Yes.Value)
                    .ThenBy(x => x.ProductID)
                    .Select(x => new MyTextValue
                    {
                        Text = x.ProductName,
                        Value = x.ProductID,
                        Disabled = x.Activate != YesNo.Yes.Value
                    }
                    ).ToList();

                categories = new HashSet<MyTextValue>(query);
                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyTextValue>)_Cache[key];
        }

        // 條件
        public Dictionary<string, String> GetPriceKindList()
        {
            string key = "PRICE_KIND";

            if (null == _Cache[key])
            {
                Dictionary<String, String> categories = new Dictionary<String, String>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                categories =
                    PriceKind.GetAll()
                    .ToDictionary(x => x.Value, x => x.Text);

                _Cache.Set(key, categories, policy);
            }

            return (Dictionary<string, String>)_Cache[key];
        }

        // 
        public HashSet<MyTextValue> GetDealItemList()
        {
            string key = "DEAL_ITEM";
                       
            if (null == _Cache[key])
            {
                HashSet<MyTextValue> categories = new HashSet<MyTextValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                     DealItem.GetAll()                                                            
                    .Select(x => new MyTextValue
                    {
                        Text = x.Text,
                        Value = x.Value,
                        Disabled = true
                    }
                    ).ToList();

                categories = new HashSet<MyTextValue>(query);
                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyTextValue>)_Cache[key];       
        }

        public HashSet<MyTextValue> GetCheckStatusList()
        {
            string key = "CHECK_STATUS";

            if (null == _Cache[key])
            {
                HashSet<MyTextValue> categories = new HashSet<MyTextValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                     CheckStatus.GetAll()
                    .Select(x => new MyTextValue
                    {
                        Text = x.Text,
                        Value = x.Value,
                        Disabled = true
                    }
                    ).ToList();

                categories = new HashSet<MyTextValue>(query);
                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyTextValue>)_Cache[key];
        }

        public HashSet<MyTextValue> GetDepositSummaryList()
        {
            string key = "DEPOSIT_SUMMARY";

            if (null == _Cache[key])
            {
                HashSet<MyTextValue> categories = new HashSet<MyTextValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                     DepositSummary.GetAll()
                    .Select(x => new MyTextValue
                    {
                        Text = x.Text,
                        Value = x.Value,
                        Disabled = true
                    }
                    ).ToList();

                categories = new HashSet<MyTextValue>(query);
                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyTextValue>)_Cache[key];
        }


        // 產品類別
        public HashSet<MyTextValue> GetProductKindList()
        {
            string key = CodeType.ProductKind.Value;

            if (null == _Cache[key])
            {
                HashSet<MyTextValue> categories = new HashSet<MyTextValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                     this.GetActivateCode()
                    .Where(x => x.CodeType == key)
                    .OrderByDescending(x=> x.Activate == YesNo.Yes.Value)
                    .ThenBy(x => x.SortOrder)
                    .Select(x => new MyTextValue
                    {
                        Text = x.CodeName,
                        Value = x.CodeValue,
                        Disabled = x.Activate != YesNo.Yes.Value
                    }
                    ).ToList();

                categories = new HashSet<MyTextValue>(query);
                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyTextValue>)_Cache[key];
        }

        // 報價授權等級
        public HashSet<MyTextValue> GetQuotationLevelList()
        {
            string key = CodeType.QuotationLevel.Value;

            if (null == _Cache[key])
            {
                HashSet<MyTextValue> categories = new HashSet<MyTextValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                    this.GetActivateCode().Where(x => x.CodeType == key)
                    .OrderByDescending(x=> x.Activate == YesNo.Yes.Value)
                    .ThenBy(x => x.SortOrder)
                    .Select(x => new MyTextValue
                    {
                        Text = x.CodeName,
                        Value = x.CodeValue,
                        Disabled = x.Activate != YesNo.Yes.Value
                    }
                    ).ToList();

                categories = new HashSet<MyTextValue>(query);
                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyTextValue>)_Cache[key];
        }

        public Dictionary<string, String> GetShippingModeList()
        {
            string key = "SHIPPING_MODE";

            if (null == _Cache[key])
            {
                Dictionary<String, String> categories = new Dictionary<String, String>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                categories =
                    this.GetActivateCode().Where(x => x.CodeType == key)
                    .OrderBy(x => x.SortOrder)
                    .ToDictionary(x => x.CodeValue, x => x.CodeName);

                _Cache.Set(key, categories, policy);
            }

            Dictionary<String, String> result = (Dictionary<string, String>)_Cache[key];

            return result;
        }

        // 供應商
        public HashSet<MyTextValue> GetSupplierList()
        {
            string key = "SUPPLIER";

            if (null == _Cache[key])
            {
                HashSet<MyTextValue> categories = new HashSet<MyTextValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                    this.GetActivateCompany()
                    .Where(x => x.CompanyKindID == CompanyType.Both.Value 
                             || x.CompanyKindID == CompanyType.Suppiler.Value)
                    .OrderByDescending(x => x.Activate == YesNo.Yes.Value)
                    .ThenBy(x => x.CompanyID)
                    .Select(x => new MyTextValue
                    {
                        Text = x.ShortName,
                        Value = x.CompanyID,
                        Disabled = x.Activate != YesNo.Yes.Value
                    }
                    ).ToList();

                categories = new HashSet<MyTextValue>(query);
                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyTextValue>)_Cache[key];
        }

        // 職稱
        public HashSet<MyTextValue> GetTicketTypeList()
        {
            string key = "TICKET_TYPE";

            if (null == _Cache[key])
            {
                HashSet<MyTextValue> categories = new HashSet<MyTextValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                    this.GetActivateCode().Where(x => x.CodeType == key)
                    .OrderByDescending(x => x.Activate == YesNo.Yes.Value)
                    .ThenBy(x => x.SortOrder)
                    .Select(x => new MyTextValue
                    {
                        Text = x.CodeName,
                        Value = x.CodeValue,
                        Disabled = x.Activate != YesNo.Yes.Value
                    }
                    ).ToList();

                categories = new HashSet<MyTextValue>(query);
                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyTextValue>)_Cache[key];
        }

        // 職稱
        public HashSet<MyTextValue> GetTitleList()
        {
            string key = CodeType.Title.Value;

            if (null == _Cache[key])
            {
                HashSet<MyTextValue> categories = new HashSet<MyTextValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                    this.GetActivateCode().Where(x => x.CodeType == key)
                    .OrderByDescending(x=> x.Activate == YesNo.Yes.Value)
                    .ThenBy(x => x.SortOrder)
                    .Select(x => new MyTextValue
                    {
                        Text = x.CodeName,
                        Value = x.CodeValue,
                        Disabled = x.Activate != YesNo.Yes.Value
                    }
                    ).ToList();

                categories = new HashSet<MyTextValue>(query);
                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyTextValue>)_Cache[key];
        }
      
        // 產品單位
        public HashSet<MyTextValue> GetUnitKindList()
        {
            string key = CodeType.Unit.Value;

            if (null == _Cache[key])
            {
                HashSet<MyTextValue> categories = new HashSet<MyTextValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                    this.GetActivateCode().Where(x => x.CodeType == key)
                    .OrderByDescending(x => x.Activate == YesNo.Yes.Value)
                    .ThenBy(x => x.SortOrder)
                    .Select(x => new MyTextValue
                    {
                        Text = x.CodeName,
                        Value = x.CodeValue,
                        Disabled = x.Activate != YesNo.Yes.Value
                    }
                    ).ToList();

                categories = new HashSet<MyTextValue>(query);
                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyTextValue>)_Cache[key];
        }

        // 使用者
        public HashSet<MyTextValue> GetUserList()
        {
            string key = "USER";

            if (null == _Cache[key])
            {
                HashSet<MyTextValue> categories = new HashSet<MyTextValue>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                var query =
                 this.GetActivateUser()
                    .OrderByDescending(x => x.Activate == YesNo.Yes.Value)
                    .OrderBy(x => x.UserID)
                    .Select(x => new MyTextValue
                    {
                        Text = x.UserName,
                        Value = x.UserID,
                        Disabled = x.Activate != YesNo.Yes.Value
                    }
                    ).ToList();

                categories = new HashSet<MyTextValue>(query);
                _Cache.Set(key, categories, policy);

                _Cache.Set(key, categories, policy);
            }

            return (HashSet<MyTextValue>)_Cache[key];
        }

        // 倉庫不能用設定的
        public Dictionary<string, String> GetWarehouseList()
        {
            string key = "WAREHOUSE_KIND";

            if (null == _Cache[key])
            {
                Dictionary<String, String> categories = new Dictionary<String, String>();
                CacheItemPolicy policy = CreateCacheItemPolicy();

                categories =
                    Warehouse.GetAll()
                    .ToDictionary(x => x.Value, x => x.Text);

                _Cache.Set(key, categories, policy);
            }

            Dictionary<String, String> result = (Dictionary<string, String>)_Cache[key];

            return result;
        }
    }
}
