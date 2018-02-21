using CDMS.Language;
using CDMS.Model;
using CDMS.Model.ViewModel;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;

namespace CDMS.Service
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IRepository<Model.Product> _Repository;
        private readonly IRepository<Model.Company> _Company;
        private readonly IRepository<Model.Brand> _Brand;
        private readonly IRepository<Model.Code> _Code;
        private readonly IRepository<Model.v_Price> _Price;

        private readonly UserInfo _CurrentUser = IdentityService.GetUserData();

        public ProductService(IUnitOfWork unitofwork, 
            IRepository<Model.Product> repository,
            IRepository<Model.Company> company,
            IRepository<Model.Brand> brand,
            IRepository<Model.Code> code,
            IRepository<Model.v_Price> price
            )
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
            this._Company = company;
            this._Brand = brand;
            this._Code = code;
            this._Price = price;
        }

        private string GenerateProductID(Product info)
        {
            string result = "";
            result = info.ProductID;
            return result;
        }

        private Model.Product GetInfoOnCreate(Product info)
        {
            // 取得產品號;目前由使用者自型輸入
            info.ProductID = GenerateProductID(info);
            info.LastPerson = _CurrentUser.UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Model.Product GetInfoOnUpdate(Product info)
        {
            Product query = this.Get(info.ProductID);
                        
            query.ProductID = info.ProductID;
            query.KindID = info.KindID;
            query.ProductName = info.ProductName;
            query.UnitID = info.UnitID;
            query.LatestCost = 0;   // 改捉 v_Price
            query.SalesCost = 0;    // 改捉 v_Price
            query.BizCost = info.BizCost;
            query.Linked = info.Linked;
            query.ListPrice = info.ListPrice;
            query.SetPrice = info.SetPrice;
            query.RealPrice = info.RealPrice;
            query.SPEC = info.SPEC;
            query.BOM = info.BOM;
            query.SupplierID = info.SupplierID;
            query.SafeStock = info.SafeStock;
            query.Remarks = info.Remarks;
            query.Activate = info.Activate;
            query.LastPerson = _CurrentUser.UserID;;
            query.LastUpdate = DateTime.Now;

            return query;
        }

        private Model.Product GetInfoOnDelete(Product info)
        {
            Product query = this.Get(info.ProductID);

            query.Activate = YesNo.No.Value;
            query.LastPerson = _CurrentUser.UserID; ;
            query.LastUpdate = DateTime.Now;

            return query;
        }

        public void Create(Product info)
        {
            #region 邏輯驗證
            if (this.Get(info.ProductID) != null)
            {
                throw new Exception($"產品: {info.ProductID} 資料已經存在!");
            }
            #endregion

            #region 取資料
            info = GetInfoOnCreate(info);
            #endregion



            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            this._Repository.Create(info);
            this._UnitOfWork.SaveChange();
            #endregion
        }

        public void Update(Product info)
        {
            #region 取資料
            Product query = GetInfoOnUpdate(info);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region Models資料庫
            this._Repository.Update(query);
            this._UnitOfWork.SaveChange();
            #endregion
        }

        public bool IsUsed(Product info) {
            var query = this._Repository.Get(x => x.ProductID == info.ProductID);
            var result = query.InquiryDetail.Any();

            if (!result)
            {
                result = query.PurchaseDetail.Any();
            }

            this._Repository.HandleDetached(query); // 
            return result;            
        }

        public void Delete(Product model)
        {
            #region 取資料
            Model.Product info = GetInfoOnDelete(model);
            #endregion

            #region 邏輯驗證
            if (info == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());

            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            if (this.IsUsed(info))
            {
                this._Repository.Update(info);
            }
            else
            {
                this._Repository.Delete(info);
            }

            this._UnitOfWork.SaveChange();
            #endregion
        }

        public Product Get(string id)
        {
            var info = this._Repository.Get(x => x.ProductID == id);
            var price = this._Price.Get(x => x.ProductID == id);

            if ((info != null) && (price != null))
            {
                info.LatestCost = price.LatestCost;
                info.SalesCost = price.SalesCost;
            }

            return info;               
        }

        public IQueryable<ProductViewModel> GetAll()
        {
            //return this._Repository.GetAll();
            var query = from u in this._Repository.GetAll()
                        join m in this._Price.GetAll() on u.ProductID equals m.ProductID into mm
                        from m in mm.DefaultIfEmpty()
                        select new ProductViewModel()
                        {
                            ProductID = u.ProductID,
                            ProductName = u.ProductName,
                            KindID = u.KindID,
                            UnitID = u.UnitID,
                            BrandID = u.BrandID,
                            LatestCost = (m == null) ? 0 : m.LatestCost,
                            SalesCost = (m == null) ? 0 : m.SalesCost,
                            BizCost = u.BizCost,
                            Linked = u.Linked,
                            ListPrice = u.ListPrice,
                            SetPrice = u.SetPrice,
                            RealPrice = u.RealPrice,
                            SPEC = u.SPEC,
                            BOM = u.BOM,
                            SupplierID = u.SupplierID,
                            SafeStock = u.SafeStock,
                            Remarks = u.Remarks,
                            Activate = u.Activate,
                        };
            return query;        
        }

        public IQueryable<ProductListViewModel> GetListView()
        {            
            var query = from u in this._Repository.GetAll()
                        join p in this._Company.GetAll() on u.SupplierID equals p.CompanyID into g
                        from p in g.DefaultIfEmpty()
                        join c in this._Code.GetAll()
                            .Where(x=>x.CodeType == CodeType.Unit.Value) on u.UnitID equals c.CodeValue into cc
                        from c in cc.DefaultIfEmpty()
                        join k in this._Code.GetAll()
                            .Where(x => x.CodeType == CodeType.ProductKind.Value) on u.KindID equals k.CodeValue into kk
                        from k in kk.DefaultIfEmpty()
                        join m in this._Price.GetAll() on u.ProductID equals m.ProductID into mm
                        from m in mm.DefaultIfEmpty()
                        select new ProductListViewModel
                        {
                            ProductID = u.ProductID,
                            ProductName = u.ProductName,
                            KindID = u.KindID,
                            UnitID = u.UnitID,
                            BrandID = u.BrandID,
                            LatestCost = (m == null) ? 0 : m.LatestCost,
                            SalesCost = (m == null) ? 0 :  m.SalesCost,
                            BizCost = u.BizCost,
                            Linked = u.Linked,
                            ListPrice = u.ListPrice,
                            SetPrice = u.SetPrice,
                            RealPrice = u.RealPrice,
                            SPEC = u.SPEC,
                            BOM = u.BOM,
                            SupplierID = u.SupplierID,
                            SafeStock = u.SafeStock,
                            Remarks = u.Remarks,
                            Activate = u.Activate,
                            SupplierName = (p == null) ? "" : p.ShortName,
                            UnitName = (c == null) ? "" : c.CodeName,
                            KindName = (k == null) ? "" : k.CodeName
                        };

            return query;

        }

        public List<ProductViewModel> GetForSelect(string id)
        {
            return GetAll().Where(x => x.ProductID == id).ToList();
        }

        public List<ProductViewModel> GetForAutoComplete(string term, int count)
        {
            try
            {
                var query = GetAll()
                    .Where(x => x.Activate == YesNo.Yes.Value && x.ProductID.Contains(term));

                return query.Take(count).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public bool IsDataExists(Product info)
        {
            var query = this._Repository
                .Get(x => x.ProductID == info.ProductID);
            return (query != null);
        }
    }
}
