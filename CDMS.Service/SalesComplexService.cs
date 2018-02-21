using CDMS.Language;
using CDMS.Model;
using CDMS.Model.ViewModel;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace CDMS.Service
{
    public class SalesComplexService : ISalesComplexService
    {
        private readonly IUnitOfWork _UnitOfWork;

        private readonly IRepository<Model.Sales> _Repository;        
        private readonly IRepository<Model.Company> _Company;
        private readonly IRepository<Model.SalesDetail> _DetailRepository;
        private readonly IRepository<Model.Product> _Product;
        private readonly IRepository<Model.v_CustomerLatestSales> _CustomerLatestSales;
        private readonly IRepository<Model.v_LatestSales> _LatestSales;

        public SalesComplexService(
            IUnitOfWork unitofwork,
            IRepository<Model.Sales> repository,
            IRepository<Model.SalesDetail> detailRepository,
            IRepository<Model.Company> company,
            IRepository<Model.Product> product,
            IRepository<Model.v_CustomerLatestSales> customerlatestSales,
            IRepository<Model.v_LatestSales> latestSales
            )
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
            this._DetailRepository = detailRepository;
            this._Company = company;
            this._Product = product;
            this._CustomerLatestSales = customerlatestSales;
            this._LatestSales = latestSales;
        }

        private string GenerateSalesID(Sales info)
        {
            int seq = 1;
            string result = "";

            string key = $"S{DateTime.Today.ToString("yyMMdd")}";

            var current =
                this._Repository.GetAll()
                .Where(x => x.SalesID.StartsWith(key))
                .OrderByDescending(x => x.SalesID)
                .FirstOrDefault();

            if (current != null)
            {
                // 移除前面日期部分留下流水號
                seq = Convert.ToInt16(current.SalesID.Replace(key, ""));
                seq += 1;
            }

            result = $"{key}{seq.ToString().PadLeft(GlobalSettings.SequenceLength, '0')}";
            return result;
        }

        private Sales GetSalesOnCreate(SalesComplex source)
        {
            Sales info = Mapper.Map<Sales>(source.Sales);

            // 取得詢價單號;目前由使用者自型輸入
            info.SalesID = GenerateSalesID(info);
            //info.PostingTime = DateTime.Today; 
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Sales GetSalesOnUpdate(SalesComplex source)
        {
            Sales info = Mapper.Map<Sales>(source.Sales);
                        
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private List<SalesDetail> GetChildOnCreate(Sales master, SalesComplex source)
        {
            List<SalesDetail> infos = new List<SalesDetail>();
            var wanted = source.ChildList.Where(x => x.IsDirty == true);

            foreach (var item in wanted)
            {
                SalesDetail temp = Mapper.Map<SalesDetail>(item);
                temp.SalesID = master.SalesID;
                temp.LastPerson = IdentityService.GetUserData().UserID;
                temp.LastUpdate = DateTime.Now;
                infos.Add(temp);
            }
            return infos;
        }

        public SalesComplex Create(SalesComplex source)
        {
            #region 取資料


            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料
            Sales main = GetSalesOnCreate(source);

            List<SalesDetail> children = GetChildOnCreate(main, source);
            #endregion

            #region Models資料庫

            this._Repository.Create(main);

            foreach (SalesDetail item in children)
            {
                this._DetailRepository.Create(item);
            }

            this._UnitOfWork.SaveChange();
            #endregion

            return this.Get(main.SalesID); 
        }

        public void Update(SalesComplex source)
        {
            #region 取資料


            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料
            Sales main = GetSalesOnUpdate(source);

            List<SalesDetail> children = GetChildOnCreate(main, source);
            #endregion

            #region Models資料庫

            this._Repository.Update(main);

            foreach (SalesDetail item in children)
            {
                if (item.SeqNo == 0)
                {
                    this._DetailRepository.Create(item);
                }
                else
                {
                    this._DetailRepository.Update(item);
                }
            }

            this._UnitOfWork.SaveChange();
            #endregion
        }

        public void Delete(SalesComplex model)
        {
            #region 邏輯驗證
            if (model == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            var info = Mapper.Map<Sales>(model.Sales);
            #endregion

            #region Models資料庫 

            foreach (SalesDetailViewModel t in model.ChildList)
            {
                var d = Mapper.Map<SalesDetail>(t);
                _DetailRepository.Delete(d);
            }

            this._Repository.Delete(info);
            this._UnitOfWork.SaveChange();
            #endregion        
        }

        public void RemoveChild(long id)
        {
            #region 取資料
            SalesDetail query = this._DetailRepository.Get(x => x.SeqNo == id);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());

            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            this._DetailRepository.Delete(query);
            this._UnitOfWork.SaveChange();
            #endregion
        }

        public SalesComplex Get(string id)
        {
            SalesComplex info = new SalesComplex();
                        
            info = this.GetAll().Where(x => x.Sales.SalesID == id).Single();   
            return info;
        }

        public IQueryable<SalesComplex> GetAll()
        {
            var query1 = from u in this._Repository.GetAll()
                         join p in this._Company.GetAll() on u.CustomerID equals p.CompanyID into g
                         from p in g.DefaultIfEmpty()
                         select new SalesViewModel()
                         {
                             SalesID = u.SalesID,
                             SalesDate = u.SalesDate,
                             CustomerID = u.CustomerID,
                             TaxID = u.TaxID,
                             ContactPerson = u.ContactPerson,
                             ContactPhone = u.ContactPhone,
                             InvoiceAddress = u.InvoiceAddress,
                             ShippingAddress = u.ShippingAddress,
                             ShippingModeID = u.ShippingModeID,
                             ShippingFee = u.ShippingFee,
                             WarehouseID = u.WarehouseID,
                             AccountMonth = u.AccountMonth,
                             Remarks = u.Remarks,
                             InvoiceID = u.InvoiceID,
                             InvoiceDate = u.InvoiceDate,
                             InvoiceAmount = u.InvoiceAmount,
                             PostingTime = u.PostingTime,
                             Activate = u.Activate,
                             CompanyName = (p == null) ? "" : p.ShortName
                         };

            var query2 =
              from u in this._DetailRepository.GetAll()
              join p in this._Product.GetAll() on u.ProductID equals p.ProductID into g
              from p in g.DefaultIfEmpty()
              select new SalesDetailViewModel()
              {
                  SeqNo = u.SeqNo,
                  SalesID = u.SalesID,
                  ProductID = u.ProductID,                  
                  PriceKindID = u.PriceKindID,
                  ConditionID = u.ConditionID,
                  Discount = u.Discount,
                  Price = u.Price,
                  Qty = u.Qty,
                  Amount = u.Amount,
                  Remarks = u.Remarks,
                  OriginalPrice = u.OriginalPrice,
                  ProductName = (p == null) ? "" : p.ProductName
              };

            var query =
                 from u in query1
                 select new SalesComplex()
                 {
                     Sales = u,
                     ChildList = (
                                 from p in query2
                                 where p.SalesID == u.SalesID
                                 select p
                                 ).ToList()
                 };

            return query;
        }

        public bool IsUsed(SalesComplex info)
        {
            //var query = this._Repository.Get(x => x.InquiryID == model.Inquiry.InquiryID);
            //var result = query.Product.Any();
            //this._Repository.HandleDetached(query); // 
            //return result;
            return true; // 目前不知關聯到哪些資料表 
        }

        public IQueryable<v_CustomerLatestSales> GetLatestSales(string productID, int count = 10)
        {
            try
            {
                var query = this._CustomerLatestSales.GetAll()
                    .Where(x => x.ProductID == productID);

                return query.Take(count);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public SalesHistoryViewModel GetHistory(string productID)
        {
         
            var temp = from u in _LatestSales.GetAll()
                       where u.ProductID == productID
                       select u;
                                  
            var procuct = _Product.Get(x => x.ProductID == productID);

            if (null == procuct) throw new NullReferenceException($"找不到產品,產品編號: { productID } !");
      
            var query = new SalesHistoryViewModel()
            {
                ProductID = procuct.ProductID,
                ProductName = procuct.ProductName,

                Inquiry1 =  (
                            from t in temp
                            where t.RowIndex == 1
                            select t
                            ).SingleOrDefault(),

                Inquiry2 = (
                            from t in temp
                            where t.RowIndex == 2
                            select t
                            ).SingleOrDefault(),

                Inquiry3 = (
                            from t in temp
                            where t.RowIndex == 3
                            select t
                            ).SingleOrDefault(),
            };

            return query;
        }
    }
}
