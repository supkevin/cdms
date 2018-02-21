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
    public class QuotationComplexService : IQuotationComplexService
    {
        private readonly IUnitOfWork _UnitOfWork;

        private readonly IRepository<Model.Quotation> _Repository;
        private readonly IRepository<Model.Company> _Company;
        private readonly IRepository<Model.QuotationDetail> _DetailRepository;
        private readonly IRepository<Model.Product> _Product;
        private readonly IRepository<Model.User> _User;

        public QuotationComplexService(
            IUnitOfWork unitofwork,
            IRepository<Model.Quotation> repository,
            IRepository<Model.QuotationDetail> detailRepository,
            IRepository<Model.Company> company,
            IRepository<Model.Product> product,
            IRepository<Model.User> user
            )
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
            this._DetailRepository = detailRepository;
            this._Company = company;
            this._Product = product;
            this._User = user;
        }

        private string GenerateQuotationID(Quotation info)
        {
            int seq = 1;
            string result = "";

            string key = $"Q{DateTime.Today.ToString("yyMMdd")}";

            var current =
                this._Repository.GetAll()
                .Where(x => x.QuotationID.StartsWith(key))
                .OrderByDescending(x => x.QuotationID)
                .FirstOrDefault();

            if (current != null)
            {
                // 移除前面日期部分留下流水號
                seq = Convert.ToInt16(current.QuotationID.Replace(key, ""));
                seq += 1;
            }

            result = $"{key}{seq.ToString().PadLeft(GlobalSettings.SequenceLength, '0')}";
            return result;
        }

        private Quotation GetQuotationOnCreate(QuotationComplex source)
        {
            Quotation info = Mapper.Map<Quotation>(source.Quotation);

            // 取得詢價單號;目前由使用者自型輸入
            info.QuotationID = GenerateQuotationID(info);
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Quotation GetQuotationOnUpdate(QuotationComplex source)
        {
            // Update 時不是所有欄位都讀出審核的部分沒資料出不能直接用automapper
            // 
            Quotation temp = Mapper.Map<Quotation>(source.Quotation);
            Quotation info = this._Repository.Get(x => x.QuotationID == source.Quotation.QuotationID);

            info.QuotationID = temp.QuotationID;
            info.QuotationDate = temp.QuotationDate;
            info.WarehouseID = temp.WarehouseID;
            info.CustomerID = temp.CustomerID;
            info.TaxID = temp.TaxID;
            info.ContactPerson = temp.ContactPerson;
            info.ContactPhone = temp.ContactPhone;
            info.InvoiceAddress = temp.InvoiceAddress;
            info.ShippingAddress = temp.ShippingAddress;
            info.ShippingModeID = temp.ShippingModeID;
            info.ShippingFee = temp.ShippingFee;
            info.Total = temp.Total;
            info.Remarks = temp.Remarks;
            info.QuotePerson = temp.QuotePerson;
            info.ValidateDate = temp.ValidateDate;
            info.Activate = temp.Activate;
            
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;

            return info;
        }

        private Quotation GetQuotationOnAudit(QuotationComplex source)
        {
            Quotation info = Mapper.Map<Quotation>(source.Quotation);

            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;

            return info;
        }

        private List<QuotationDetail> GetChildOnCreate(Quotation master, QuotationComplex source)
        {
            List<QuotationDetail> infos = new List<QuotationDetail>();
            var wanted = source.ChildList.Where(x => x.IsDirty == true);

            foreach (var item in wanted)
            {
                QuotationDetail temp = Mapper.Map<QuotationDetail>(item);
                temp.QuotationID = master.QuotationID;
                temp.LastPerson = IdentityService.GetUserData().UserID;
                temp.LastUpdate = DateTime.Now;
                infos.Add(temp);
            }
            return infos;
        }

        public QuotationComplex Create(QuotationComplex source)
        {
            #region 取資料


            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料
            Quotation main = GetQuotationOnCreate(source);

            List<QuotationDetail> children = GetChildOnCreate(main, source);
            #endregion

            #region Models資料庫

            this._Repository.Create(main);

            foreach (QuotationDetail item in children)
            {
                this._DetailRepository.Create(item);
            }

            this._UnitOfWork.SaveChange();
            #endregion

            return this.Get(main.QuotationID);
        }

        public void Update(QuotationComplex source)
        {
            #region 取資料


            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料
            Quotation main = GetQuotationOnUpdate(source);

            List<QuotationDetail> children = GetChildOnCreate(main, source);
            #endregion

            #region Models資料庫

            this._Repository.Update(main);

            foreach (QuotationDetail item in children)
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

        public void Audit(QuotationComplex source)
        {
            #region 取資料


            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料
            Quotation main = GetQuotationOnAudit(source);

            List<QuotationDetail> children = GetChildOnCreate(main, source);
            #endregion

            #region Models資料庫

            this._Repository.Update(main);

            foreach (QuotationDetail item in children)
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

        public void Delete(QuotationComplex model)
        {
            #region 邏輯驗證
            if (model == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            var info = Mapper.Map<Quotation>(model.Quotation);
            #endregion

            #region Models資料庫 

            foreach (QuotationDetailViewModel t in model.ChildList)
            {
                var d = Mapper.Map<QuotationDetail>(t);
                _DetailRepository.Delete(d);
            }

            this._Repository.Delete(info);
            this._UnitOfWork.SaveChange();
            #endregion        
        }

        public void RemoveChild(long id)
        {
            #region 取資料
            QuotationDetail query = this._DetailRepository.Get(x => x.SeqNo == id);
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

        public QuotationComplex Get(string id)
        {
            QuotationComplex info = new QuotationComplex();
            info = this.GetAll().Where(x => x.Quotation.QuotationID == id).Single();
            return info;
        }

        public IQueryable<QuotationComplex> GetAll()
        {
            var query1 = from u in this._Repository.GetAll()
                         join p in this._Company.GetAll() on u.CustomerID equals p.CompanyID into g
                         from p in g.DefaultIfEmpty()
                         join t in this._User.GetAll() on u.QuotePerson equals t.UserID into tt
                         from t in tt.DefaultIfEmpty()
                         select new QuotationViewModel()
                         {
                             QuotationID = u.QuotationID,
                             QuotationDate = u.QuotationDate,
                             WarehouseID = u.WarehouseID,
                             CustomerID = u.CustomerID,
                             TaxID = u.TaxID,
                             ContactPerson = u.ContactPerson,
                             ContactPhone = u.ContactPhone,
                             InvoiceAddress = u.InvoiceAddress,
                             ShippingAddress = u.ShippingAddress,
                             ShippingModeID = u.ShippingModeID,
                             ShippingFee = u.ShippingFee,
                             Total = u.Total,
                             Remarks = u.Remarks,
                             QuotePerson = u.QuotePerson,
                             ValidateDate = u.ValidateDate,
                             Auditor = u.Auditor,
                             ReviewDate = u.ReviewDate,
                             Result = u.Result,
                             SalesID = u.SalesID,
                             SalesDate = u.SalesDate,
                             Activate = u.Activate,                           
                             CompanyName = (p == null) ? "" : p.ShortName,
                             QuotePersonName = (t == null) ? "" : t.UserName
                         };

            var query2 =
              from u in this._DetailRepository.GetAll()
              join p in this._Product.GetAll() on u.ProductID equals p.ProductID into g
              from p in g.DefaultIfEmpty()
              select new QuotationDetailViewModel()
              {
                  SeqNo = u.SeqNo,
                  QuotationID = u.QuotationID,
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
                 select new QuotationComplex()
                 {
                     Quotation = u,
                     ChildList = (
                                 from p in query2
                                 where p.QuotationID == u.QuotationID
                                 select p
                                 ).ToList()
                 };

            return query;
        }

        public bool IsUsed(QuotationComplex info)
        {
            //var query = this._Repository.Get(x => x.InquiryID == model.Inquiry.InquiryID);
            //var result = query.Product.Any();
            //this._Repository.HandleDetached(query); // 
            //return result;
            return true; // 目前不知關聯到哪些資料表 
        }
    }
}
