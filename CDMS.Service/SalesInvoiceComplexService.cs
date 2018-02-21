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
    public class SalesInvoiceComplexService : ISalesInvoiceComplexService
    {
        private readonly IUnitOfWork _UnitOfWork;

        private readonly IRepository<Model.SalesInvoice> _Repository;        
        private readonly IRepository<Model.Company> _Company;
        private readonly IRepository<Model.SalesInvoiceDetail> _DetailRepository;
        private readonly IRepository<Model.Product> _Product;

        public SalesInvoiceComplexService(
            IUnitOfWork unitofwork,
            IRepository<Model.SalesInvoice> repository,
            IRepository<Model.SalesInvoiceDetail> detailRepository,
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

        private string GenerateInvoiceID(SalesInvoice info)
        {            
            return info.InvoiceID;
        }

        private SalesInvoice GetSalesInvoiceOnCreate(SalesInvoiceComplex source)
        {
            SalesInvoice info = Mapper.Map<SalesInvoice>(source.Invoice);

            // 取得詢價單號;目前由使用者自型輸入
            info.InvoiceID = GenerateInvoiceID(info);            
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private SalesInvoice GetSalesInvoiceOnUpdate(SalesInvoiceComplex source)
        {
            SalesInvoice info = Mapper.Map<SalesInvoice>(source.Invoice);
                        
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private List<SalesInvoiceDetail> GetChildOnCreate(SalesInvoice master, SalesInvoiceComplex source)
        {
            List<SalesInvoiceDetail> infos = new List<SalesInvoiceDetail>();
            var wanted = source.ChildList.Where(x => x.IsDirty == true);

            foreach (var item in wanted)
            {
                SalesInvoiceDetail temp = Mapper.Map<SalesInvoiceDetail>(item);
                temp.InvoiceID = master.InvoiceID;
                temp.LastPerson = IdentityService.GetUserData().UserID;
                temp.LastUpdate = DateTime.Now;
                infos.Add(temp);
            }
            return infos;
        }

        public SalesInvoiceComplex Create(SalesInvoiceComplex source)
        {
            #region 取資料


            #endregion

            #region 邏輯驗證
            if (this.IsDataExists(source))
            {
                throw new Exception($"{"InvoiceID".ToLocalized()}:{source.Invoice.InvoiceID} 已經存在！");
            }
            #endregion

            #region 變為Models需要之型別及邏輯資料
            SalesInvoice main = GetSalesInvoiceOnCreate(source);

            List<SalesInvoiceDetail> children = GetChildOnCreate(main, source);
            #endregion

            #region Models資料庫

            this._Repository.Create(main);

            foreach (SalesInvoiceDetail item in children)
            {
                this._DetailRepository.Create(item);
            }

            this._UnitOfWork.SaveChange();
            #endregion

            return this.Get(main.InvoiceID); 
        }

        public void Update(SalesInvoiceComplex source)
        {
            #region 取資料


            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料
            SalesInvoice main = GetSalesInvoiceOnUpdate(source);

            List<SalesInvoiceDetail> children = GetChildOnCreate(main, source);
            #endregion

            #region Models資料庫

            this._Repository.Update(main);

            foreach (SalesInvoiceDetail item in children)
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

        public void Delete(SalesInvoiceComplex model)
        {
            //#region 取資料
            //Model.SalesInvoice query = this.Get(Model.InvoiceID);
            ////var queryoverseastaff = this._overseaService.GetForOverType(query.ID_OverType);
            //#endregion

            //#region 邏輯驗證
            //if (query == null)//沒有資料
            //    throw new Exception("MessageNoData".ToLocalized());

            ////驗證
            ////if (queryoverseastaff == null)//沒有資料
            ////    throw new Exception("MessageDataHasLinking".ToLocalized());
            //#endregion

            //#region 變為Models需要之型別及邏輯資料

            //#endregion

            //#region Models資料庫
            //this._Repository.Delete(query);
            //this._UnitOfWork.SaveChange();
            //#endregion
        }

        public void RemoveChild(long id)
        {
            #region 取資料
            SalesInvoiceDetail query = this._DetailRepository.Get(x => x.SeqNo == id);
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

        public SalesInvoiceComplex Get(string id)
        {
            SalesInvoiceComplex info = new SalesInvoiceComplex();
            var query =
                from u in this._Repository.GetAll()
                join p in this._Company.GetAll() on u.CustomerID equals p.CompanyID into g
                from p in g.DefaultIfEmpty()
                where (u.InvoiceID == id)
                select new SalesInvoiceViewModel()
                {
                    InvoiceID = u.InvoiceID,
                    InvoiceDate = u.InvoiceDate,
                    CustomerID = u.CustomerID,
                    Title = u.Title,
                    TaxID = u.TaxID,
                    TaxLevelID = u.TaxLevelID,
                    TaxExcluded = u.TaxExcluded,
                    DiscountAmount = u.DiscountAmount,
                    DeductAmount = u.DeductAmount,
                    AccountMonth = u.AccountMonth,
                    Tax = u.Tax,
                    TaxIncluded = u.TaxIncluded,
                    InvoiceStatusID = u.InvoiceStatusID,
                    Remarks = u.Remarks,
                    CompanyName = (p == null) ? "" : p.ShortName
                };

            info.Invoice = query.SingleOrDefault();

            var query2 =
               from u in this._DetailRepository.GetAll()
               join p in this._Product.GetAll() on u.ProductID equals p.ProductID into g
               from p in g.DefaultIfEmpty()
               where (u.InvoiceID == id)
               select new SalesInvoiceDetailViewModel()
               {
                   SeqNo = u.SeqNo,
                   InvoiceID = u.InvoiceID,
                   ProductID = u.ProductID,
                   Price = u.Price,
                   Qty = u.Qty,
                   Amount = u.Amount,
                   ProductName = (p == null) ? "" : p.ProductName
               };

            info.ChildList = query2.ToList();

            return info;
        }

        public IQueryable<SalesInvoice> GetAll()
        {
            //return new List<SalesInvoiceComplex>().AsQueryable();
            return this._Repository.GetAll();
        }

        public bool IsDataExists(SalesInvoiceComplex info)
        {
            var query = this._Repository
                .Get(x => x.InvoiceID == info.Invoice.InvoiceID);
            return (query != null);
        }
    }
}
