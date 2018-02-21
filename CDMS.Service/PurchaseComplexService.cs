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
    public class PurchaseComplexService : IPurchaseComplexService
    {
        private readonly IUnitOfWork _UnitOfWork;

        private readonly IRepository<Model.Purchase> _Repository;        
        private readonly IRepository<Model.Company> _Company;
        private readonly IRepository<Model.PurchaseDetail> _DetailRepository;
        private readonly IRepository<Model.Product> _Product;

        public PurchaseComplexService(
            IUnitOfWork unitofwork,
            IRepository<Model.Purchase> repository,
            IRepository<Model.PurchaseDetail> detailRepository,
            IRepository<Model.Company> company,
            IRepository<Model.Product> product
            )
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
            this._DetailRepository = detailRepository;
            this._Company = company;
            this._Product = product;
        }

        private string GeneratePurchaseID(Purchase info)
        {
            int seq = 1;
            string result = "";

            string key = $"P{DateTime.Today.ToString("yyMMdd")}";

            var current =
                this._Repository.GetAll()
                .Where(x => x.PurchaseID.StartsWith(key))
                .OrderByDescending(x => x.PurchaseID)
                .FirstOrDefault();

            if (current != null)
            {
                // 移除前面日期部分留下流水號
                seq = Convert.ToInt16(current.PurchaseID.Replace(key, ""));
                seq += 1;
            }

            result = $"{key}{seq.ToString().PadLeft(GlobalSettings.SequenceLength, '0')}";
            return result;
        }

        private Purchase GetPurchaseOnCreate(PurchaseComplex source)
        {
            Purchase info = Mapper.Map<Purchase>(source.Purchase);

            // 取得詢價單號;目前由使用者自型輸入
            info.PurchaseID = GeneratePurchaseID(info);
            //info.PostingTime = DateTime.Today; 
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Purchase GetPurchaseOnUpdate(PurchaseComplex source)
        {
            Purchase info = Mapper.Map<Purchase>(source.Purchase);
                        
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private List<PurchaseDetail> GetChildOnCreate(Purchase master, PurchaseComplex source)
        {
            List<PurchaseDetail> infos = new List<PurchaseDetail>();
            var wanted = source.ChildList.Where(x => x.IsDirty == true);

            foreach (var item in wanted)
            {
                PurchaseDetail temp = Mapper.Map<PurchaseDetail>(item);
                temp.PurchaseID = master.PurchaseID;
                temp.LastPerson = IdentityService.GetUserData().UserID;
                temp.LastUpdate = DateTime.Now;
                infos.Add(temp);
            }
            return infos;
        }

        public PurchaseComplex Create(PurchaseComplex source)
        {
            #region 取資料


            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料
            Purchase main = GetPurchaseOnCreate(source);

            List<PurchaseDetail> children = GetChildOnCreate(main, source);
            #endregion

            #region Models資料庫

            this._Repository.Create(main);

            foreach (PurchaseDetail item in children)
            {
                this._DetailRepository.Create(item);
            }

            this._UnitOfWork.SaveChange();
            #endregion

            return this.Get(main.PurchaseID); 
        }

        public void Update(PurchaseComplex source)
        {
            #region 取資料


            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料
            Purchase main = GetPurchaseOnUpdate(source);

            List<PurchaseDetail> children = GetChildOnCreate(main, source);
            #endregion

            #region Models資料庫

            this._Repository.Update(main);

            foreach (PurchaseDetail item in children)
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

        public void Delete(PurchaseComplex model)
        {
            #region 邏輯驗證
            if (model == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            var info = Mapper.Map<Purchase>(model.Purchase);
            #endregion

            #region Models資料庫 

            foreach (PurchaseDetailViewModel t in model.ChildList)
            {
                var d = Mapper.Map<PurchaseDetail>(t);
                _DetailRepository.Delete(d);
            }

            this._Repository.Delete(info);
            this._UnitOfWork.SaveChange();
            #endregion        
        }


        public PurchaseComplex Get(string id)
        {
            PurchaseComplex info = new PurchaseComplex();
            //var query =
            //    from u in this._Repository.GetAll()
            //    join p in this._Company.GetAll() on u.SupplierID equals p.CompanySupplier into g
            //    from p in g.DefaultIfEmpty()
            //    where (u.PurchaseID == id)
            //    select new PurchaseViewModel()
            //    {
            //        PurchaseID = u.PurchaseID,
            //        PurchaseDate = u.PurchaseDate,
            //        SupplierID = u.SupplierID,
            //        ContactPerson = u.ContactPerson,
            //        ContactPhone = u.ContactPhone,
            //        InvoiceAmount = u.InvoiceAmount,
            //        InvoiceID = u.InvoiceID,
            //        CurrencyID = u.CurrencyID,
            //        ExchangeRate = u.ExchangeRate,
            //        AccountMonth = u.AccountMonth,
            //        WarehouseID = u.WarehouseID,
            //        Remarks = u.Remarks,
            //        PostingTime = u.PostingTime,
            //        CompanyName = (p == null) ? "" : p.CompanyName
            //    };

            //info.Purchase = query.SingleOrDefault();

            //var query2 =
            //   from u in this._DetailRepository.GetAll()
            //   join p in this._Company.GetAll() on u.ProductID equals p.ProductID into g
            //   from p in g.DefaultIfEmpty()
            //   where (u.PurchaseID == id)
            //   select new PurchaseDetailViewModel()
            //   {
            //       SeqNo = u.SeqNo,
            //       PurchaseID = u.PurchaseID,
            //       ProductID = u.ProductID,
            //       PriceKindID = u.PriceKindID,
            //       ConditionID = u.ConditionID,
            //       Discount = u.Discount,
            //       ForeignPrice = u.ForeignPrice,
            //       Price = u.Price,
            //       Qty = u.Qty,
            //       Amount = u.Amount,
            //       Remarks = u.Remarks,
            //       ProductName = (p == null) ? "" : p.ProductName
            //   };

            //info.ChildList = query2.ToList();

            info = this.GetAll().Where(x => x.Purchase.PurchaseID == id).Single();  

            return info;
        }

        public IQueryable<PurchaseComplex> GetAll()
        {
            var query1 = from u in this._Repository.GetAll()
                         join p in this._Company.GetAll() on u.SupplierID equals p.CompanyID into g
                         from p in g.DefaultIfEmpty()
                         select new PurchaseViewModel()
                         {
                             PurchaseID = u.PurchaseID,
                             PurchaseDate = u.PurchaseDate,
                             SupplierID = u.SupplierID,
                             ContactPerson = u.ContactPerson,
                             ContactPhone = u.ContactPhone,
                             InvoiceAmount = u.InvoiceAmount,
                             InvoiceID = u.InvoiceID,
                             CurrencyID = u.CurrencyID,
                             ExchangeRate = u.ExchangeRate,
                             AccountMonth = u.AccountMonth,
                             WarehouseID = u.WarehouseID,
                             Remarks = u.Remarks,
                             PostingTime = u.PostingTime,
                             Activate= u.Activate,                            
                             CompanyName = (p == null) ? "" : p.ShortName
                         };

            var query2 =
              from u in this._DetailRepository.GetAll()
              join p in this._Product.GetAll() on u.ProductID equals p.ProductID into g
              from p in g.DefaultIfEmpty()
              select new PurchaseDetailViewModel()
              {
                  SeqNo = u.SeqNo,
                  PurchaseID = u.PurchaseID,
                  ProductID = u.ProductID,
                  PriceKindID = u.PriceKindID,
                  ConditionID = u.ConditionID,
                  Discount = u.Discount,
                  ForeignPrice = u.ForeignPrice,
                  Price = u.Price,
                  Qty = u.Qty,
                  Amount = u.Amount,
                  Remarks = u.Remarks,
                  OriginalPrice = u.OriginalPrice,
                  ProductName = (p == null) ? "" : p.ProductName
              };

            var query =
                 from u in query1
                 select new PurchaseComplex()
                 {
                     Purchase = u,
                     ChildList = (
                                 from p in query2
                                 where p.PurchaseID == u.PurchaseID
                                 select p
                                 ).ToList()
                 };

            return query;
        }

        public bool IsUsed(PurchaseComplex info)
        {
            //var query = this._Repository.Get(x => x.InquiryID == model.Inquiry.InquiryID);
            //var result = query.Product.Any();
            //this._Repository.HandleDetached(query); // 
            //return result;
            return true; // 目前不知關聯到哪些資料表 
        }

    }
}
