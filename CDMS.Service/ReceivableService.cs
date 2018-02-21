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
    public class ReceivableService : IReceivableService
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IRepository<Model.Receivable> _Repository;
        private readonly IRepository<Model.BankAccount> _BankAccount;
        private readonly IRepository<Model.Company> _Company;
        private readonly IRepository<Model.Sales> _Sales;
        private readonly IRepository<Model.SalesDetail> _SalesDetail;
        private readonly IRepository<Model.Purchase> _Purchase;
        private readonly IRepository<Model.PurchaseDetail> _PurchaseDetail;

        private readonly IRepository<Model.SalesInvoice> _SalesInvoice;
        private readonly IRepository<Model.SalesInvoiceDetail> _SalesInvoiceDetail;

        private readonly IRepository<Model.PurchaseInvoice> _PurchaseInvoice;
        private readonly IRepository<Model.PurchaseInvoiceDetail> _PurchaseInvoiceDetail;

        private readonly IRepository<Model.Receipt> _Receipt;
        private readonly IRepository<Model.Payment> _Payment;

        private readonly IRepository<Model.v_ReceivableSummary> _ReceivableSummary;
        private readonly IRepository<Model.v_ReceivableDetail> _ReceivableDetail;

        private readonly UserInfo _CurrentUser = IdentityService.GetUserData();

        public ReceivableService(IUnitOfWork unitofwork,
            IRepository<Model.Receivable> repository,
            IRepository<Model.BankAccount> bankAccount,
            IRepository<Model.Company> company,
            IRepository<Model.Sales> sales,
            IRepository<Model.SalesDetail> salesDetail,
            IRepository<Model.Purchase> purchase,
            IRepository<Model.PurchaseDetail> purchaseDetail,
            IRepository<Model.SalesInvoice> salesInvoice,
            IRepository<Model.SalesInvoiceDetail> salesInvoiceDetail,
            IRepository<Model.PurchaseInvoice> purchaseInvoice,
            IRepository<Model.PurchaseInvoiceDetail> purchaseInvoiceDetail,
            IRepository<Model.Receipt> receipt,
            IRepository<Model.Payment> payment,
            IRepository<Model.v_ReceivableSummary> receivableSummary,
            IRepository<Model.v_ReceivableDetail> receivableDetail
            )
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
            this._BankAccount = bankAccount;
            this._Company = company;
            this._Sales = sales;
            this._SalesDetail = salesDetail;
            this._Purchase = purchase;
            this._PurchaseDetail = purchaseDetail;
            this._SalesInvoice = salesInvoice;
            this._SalesInvoiceDetail  = salesInvoiceDetail;

            this._PurchaseInvoice = purchaseInvoice;
            this._PurchaseInvoiceDetail = purchaseInvoiceDetail;

            this._Receipt = receipt;
            this._Payment = payment;

            this._ReceivableSummary = receivableSummary;
            this._ReceivableDetail = receivableDetail;            
        }

        private Model.Receivable GetInfoOnCreate(Receivable info)
        {
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Model.Receivable GetInfoOnDelete(Receivable info)
        {            
            info.LastPerson = _CurrentUser.UserID; ;
            info.LastUpdate = DateTime.Now;

            return info;
        }
        
        private Receivable GetInfoOnUpdate(Receivable source)
        {
            Receivable info = Mapper.Map<Receivable>(source);

            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        public void Create(Receivable model)
        {
            model = GetInfoOnCreate(model);
            this._Repository.Create(model);
            this._UnitOfWork.SaveChange();
        }

        public void Update(Receivable info)
        {
            Receivable query = GetInfoOnUpdate(info);

            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());

            this._Repository.Update(query);
            this._UnitOfWork.SaveChange();
        }

        public void Delete(Receivable info)
        {
            if (info == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());

            this._Repository.Delete(info);
            this._UnitOfWork.SaveChange();
        }

        public Receivable Get(string id)
        {
            return this._Repository.Get(x => x.VoucherID == id);
        }

        public IQueryable<Receivable> GetAll()
        {
            return this._Repository.GetAll();
        }

        public bool IsUsed(Receivable info)
        {
            //var query = this._Repository.Get(x => x.VoucherID == info.VoucherID);
            //var result = query.Product.Any();
            //this._Repository.HandleDetached(query); // 
            //return result;
            return true;
        }
            
        #region DeleteExists
        private void DeleteExists(string accountMonth)
        {
            // 刪除舊資料 // 過帳需要刪除的單據種類
            var delete = this._Repository.GetAll()
                .Where(x => x.AccountMonth == accountMonth 
                && GlobalSettings.Documents_Need_To_Delete.Contains(x.DealItem))
                .ToList();

            foreach (var item in delete)
            {
                this._Repository.Delete(item);
            }
            //this._UnitOfWork.SaveChange();
        }
        #endregion

        #region ProcessSales
        private void ProcessSales(string accountMonth)
        {
            // 銷貨資料
            var target = from u in this._Sales.GetAll()
                         .Where(x => x.Activate == YesNo.Yes.Value && x.AccountMonth == accountMonth)
                         join d in this._SalesDetail.GetAll() on u.SalesID equals d.SalesID into dd
                         from d in dd.DefaultIfEmpty()
                         select new {
                             SalesID = u.SalesID,
                             CustomerID = u.CustomerID,
                             AccountMonth = u.AccountMonth,
                             SalesDate = u.SalesDate,
                             SalesAmount = d == null ? 0 : d.Amount
                         };

            var query = from u in target
                        group u by new { u.SalesID, u.CustomerID, u.AccountMonth, u.SalesDate } into g
                        select new
                        {
                            SalesID = g.Key.SalesID,
                            CustomerID = g.Key.CustomerID,
                            AccountMonth = g.Key.AccountMonth,
                            SalesDate = g.Key.SalesDate,
                            SalesAmount = g.Sum(u => u.SalesAmount)
                        };

            query.ToList().ForEach(x =>
            {
                var info = new Receivable()
                {
                    VoucherID = x.SalesID,
                    CompanyID =x.CustomerID,
                    AccountMonth = x.AccountMonth,
                    DealDate = x.SalesDate,
                    DealItem = DealItem.Sales.Text,
                    SalesAmount = x.SalesAmount 
                };
                info = ProcessReceivable(info);
                this._Repository.Create(info);
            });
        }
        #endregion

        #region ProcessPrurchase
        private void ProcessPurchase(string accountMonth)
        { 
            // 進貨資料
            var target = from u in this._Purchase.GetAll()
                         .Where(x => x.Activate == YesNo.Yes.Value && x.AccountMonth == accountMonth)
                         join d in this._PurchaseDetail.GetAll() on u.PurchaseID equals d.PurchaseID into dd
                         from d in dd.DefaultIfEmpty()
                         select new
                         {
                             PurchaseID = u.PurchaseID,
                             SupplierID = u.SupplierID,
                             AccountMonth = u.AccountMonth,
                             PurchaseDate = u.PurchaseDate,
                             Amount = d == null ? 0 : d.Amount
                         };

            var query = from u in target
                        group u by new { u.PurchaseID, u.SupplierID, u.AccountMonth, u.PurchaseDate } into g
                        select new
                        {
                            SalesID = g.Key.PurchaseID,
                            CustomerID = g.Key.SupplierID,
                            AccountMonth = g.Key.AccountMonth,
                            PurchaseDate = g.Key.PurchaseDate,
                            Amount = g.Sum(u => u.Amount)
                        };

            query.ToList().ForEach(x =>
            {
                var info = new Receivable()
                {
                    VoucherID = x.SalesID,
                    CompanyID = x.CustomerID,
                    AccountMonth = x.AccountMonth,
                    DealDate = x.PurchaseDate,
                    DealItem = DealItem.Purchase.Text,
                    PurchaseAmount = x.Amount
                };
                info = ProcessReceivable(info);
                this._Repository.Create(info);
            });          
        }
        #endregion

        #region ProcessSalesInvoice
        private void ProcessSalesInvoice(string accountMonth)
        {
            // 銷貨發票
            var query = from u in this._SalesInvoice.GetAll()
                         .Where(x => x.Activate == YesNo.Yes.Value && x.AccountMonth == accountMonth)    
                         select new
                         {
                             InvoiceID = u.InvoiceID,
                             CustomerID = u.CustomerID,
                             AccountMonth = u.AccountMonth,
                             InvoiceDate = u.InvoiceDate,
                             Tax = u.Tax
                         };
         
            query.ToList().ForEach(x =>
            {
                var info = new Receivable()
                {
                    VoucherID = x.InvoiceID,
                    CompanyID = x.CustomerID,
                    AccountMonth = x.AccountMonth,
                    DealDate = x.InvoiceDate,
                    DealItem = DealItem.SalesInvoice.Text,
                    SalesTax = x.Tax
                };
                info = ProcessReceivable(info);
                this._Repository.Create(info);
            });
        }
        #endregion

        #region ProcessPurchaseInvoice
        private void ProcessPurchaseInvoice(string accountMonth)
        {
            // 進貨資料
            var query = from u in this._PurchaseInvoice.GetAll()
                         .Where(x => x.Activate == YesNo.Yes.Value && x.AccountMonth == accountMonth)         
                         select new
                         {
                             InvoiceID = u.InvoiceID,
                             SupplierID = u.SupplierID,
                             AccountMonth = u.AccountMonth,
                             InvoiceDate = u.InvoiceDate,
                             Tax =u.Tax
                         };
           
            query.ToList().ForEach(x =>
            {
                var info = new Receivable()
                {
                    VoucherID = x.InvoiceID,
                    CompanyID = x.SupplierID,
                    AccountMonth = x.AccountMonth,
                    DealDate = x.InvoiceDate,
                    DealItem = DealItem.PurchaseInvoice.Text,
                    PurchaseTax = x.Tax
                };

                info = ProcessReceivable(info);
                this._Repository.Create(info);
            });
        }
        #endregion

        #region ProcessPayment
        //private void ProcessPayment(string accountMonth)
        //{ 
        //    // 付款資料
        //    var query = from u in this._Payment.GetAll()
        //                 .Where(x => x.Activate == YesNo.Yes.Value && x.AccountMonth == accountMonth)
        //                 select new 
        //                 {
        //                     VoucherID = u.PaymentID,
        //                     CompanyID = u.SupplierID,
        //                     AccountMonth = u.AccountMonth,
        //                     DealDate = u.PayDate,
        //                     DealItem = DealItem.Payment.Text,
        //                     PaymentAmountTotal = u.CheckAmount + u.CashAmount,
        //                     PaymentDiscount = u.DiscountAmount,
        //                     PaymentDeduct = u.ReturnAmount,
        //                 };

        //    query.ToList().ForEach(x =>
        //    {
        //        var info = new Receivable()
        //        {
        //            VoucherID = x.VoucherID,
        //            CompanyID = x.CompanyID,
        //            AccountMonth = x.AccountMonth,
        //            DealDate = x.DealDate,
        //            DealItem = x.DealItem,
        //            PaymentAmountTotal = x.PaymentAmountTotal,
        //            PaymentDiscount = x.PaymentDiscount,
        //            PaymentDeduct = x.PaymentDeduct
        //        };

        //        info = ProcessReceivable(info);
        //        this._Repository.Create(info);
        //    });
        //}
        #endregion

        #region ProcessReceipt
        //private void ProcessReceipt(string accountMonth)
        //{
        //    // 付款資料
        //    var query = from u in this._Receipt.GetAll()
        //                 .Where(x => x.Activate == YesNo.Yes.Value && x.AccountMonth == accountMonth)
        //                select new 
        //                {
        //                    VoucherID = u.ReceiptID,
        //                    CompanyID = u.CustomerID,
        //                    AccountMonth = u.AccountMonth,
        //                    DealDate = u.ReceiptDate,
        //                    DealItem = DealItem.Receipt.Text,
        //                    ReceiptAmountTotal = u.CheckAmount + u.CashAmount,
        //                    SalesDiscountTotal = u.DiscountAmount,
        //                    SalesDeductTotal = u.ReturnAmount,
        //                };

        //    query.ToList().ForEach(x =>
        //    {
        //        var info = new Receivable()
        //        {
        //            VoucherID = x.VoucherID,
        //            CompanyID = x.CompanyID,
        //            AccountMonth =x.AccountMonth,
        //            DealDate = x.DealDate,
        //            DealItem = x.DealItem,
        //            ReceiptAmountTotal = x.ReceiptAmountTotal,
        //            SalesDiscountTotal = x.SalesDiscountTotal,
        //            SalesDeductTotal = x.SalesDeductTotal,
        //        };

        //        info = ProcessReceivable(info);
        //        this._Repository.Create(info);
        //    });
        //}
        #endregion

        private Receivable ProcessReceivable(Receivable info)
        {
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info; 
        }

        public void Initialize(string accountMonth)
        {
            string year = accountMonth.Substring(0, 2);
            string month = accountMonth.Substring(2, 2);

            DateTime start = new DateTime(int.Parse(year) + 2000 , int.Parse(month), 1);
            DateTime finish = start.AddMonths(1).AddDays(-1); 

            try
            {
                DeleteExists(accountMonth);// 刪除舊資料

                ProcessSales(accountMonth);

                ProcessPurchase(accountMonth);

                ProcessSalesInvoice(accountMonth);

                ProcessPurchaseInvoice(accountMonth);

                // 收款及付款不需處理
                //ProcessReceipt(accountMonth);
                //ProcessPayment(accountMonth);

                this._UnitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        public IQueryable<ReceivableDetailViewModel> GetDetailListView()
        {

            var query = from u in this._ReceivableDetail.GetAll()
                        select new ReceivableDetailViewModel()
                        {                            
                            VoucherID = u.VoucherID,
                            ProductID = u.ProductID,
                            ProductName = u.ProductName,
                            Qty = u.Qty  ?? 0 ,
                            Price = u.Price ?? 0 ,
                            Amount = u.Amount ?? 0 ,                            
                        };

            return query;
        }

        public IQueryable<ReceivableSummaryViewModel> GetListView()
        {

            var query = from u in this._ReceivableSummary.GetAll()                                                
                        select new ReceivableSummaryViewModel()
                        {
                            CompanyID = u.CompanyID,
                            CompanyName = u.CompanyName,
                            AccountMonth = u.AccountMonth,
                            SalesAmountTotal = u.SalesAmount,
                            SalesTaxTotal  = u.SalesTax,
                            PurchaseAmountTotal = u.PurchaseAmount,
                            PurchaseTaxTotal = u.PurchaseTax,
                            ReceiptAmountTotal = u.ReceiptAmount,
                            SalesDeductTotal = u.SalesDeduct,
                            SalesDiscountTotal = u.SalesDiscount,
                            PaymentAmountTotal = u.PaymentAmount,
                            PurchaseDeductTotal = u.PurchaseDeduct,
                            PurchaseDiscountTotal = u.PurchaseDiscount,
                            Remaining = u.Remaining,
                            AccountDate = u.AccountDate,
                            LastBalance = u.LastBalance,
                            Balance = u.Balance ?? 0
                        };

            return query; 
        }

        public IQueryable<ReconciliationViewModel> GetReconciliationListView()
        {

            //SalesAmount
            //SalesTax
            //PurchaseAmount
            //PurchaseTax

            var query1 = from u in this._Repository.GetAll()
                        select new ReconciliationViewModel()
                        {
                            CompanyID = u.CompanyID,
                            AccountMonth = u.AccountMonth,                            
                            DealItem = u.DealItem,
                            VoucherID = u.VoucherID,
                            DealDate = u.DealDate,
                            SalesAmount = u.SalesAmount ?? 0,
                            SalesTax = u.SalesTax ?? 0,
                            PurchaseAmount = u.PurchaseAmount ?? 0,
                            PurchaseTax = u.PurchaseTax ?? 0,
                            ReceiptAmount = 0,
                            SalesDeduct = 0,
                            SalesDiscount = 0,
                            PaymentAmount = 0,
                            PurchaseDeduct = 0,
                            PurchaseDiscount = 0
                        };

            var query2 = from u in this._Payment.GetAll()
                         select new ReconciliationViewModel()
                         {
                             CompanyID = u.SupplierID,
                             AccountMonth = u.AccountMonth,                             
                             DealItem = DealItem.Payment.Text,
                             VoucherID = u.PaymentID,
                             DealDate = u.PayDate,
                             SalesAmount = 0,
                             SalesTax =  0,
                             PurchaseAmount =  0,
                             PurchaseTax =  0,
                             ReceiptAmount = 0,
                             SalesDeduct = 0,
                             SalesDiscount = 0,
                             PaymentAmount = u.CashAmount ?? 0  + u.CheckAmount ?? 0,
                             PurchaseDeduct = u.ReturnAmount ?? 0,
                             PurchaseDiscount = u.DiscountAmount ?? 0,
                         };

            var query3 = from u in this._Receipt.GetAll()
                         select new ReconciliationViewModel()
                         {
                             CompanyID = u.CustomerID,
                             AccountMonth = u.AccountMonth,
                             DealItem = DealItem.Receipt.Text,
                             VoucherID = u.ReceiptID,
                             DealDate = u.ReceiptDate,
                             SalesAmount = 0,
                             SalesTax = 0,
                             PurchaseAmount = 0,
                             PurchaseTax = 0,
                             ReceiptAmount = u.CashAmount ?? 0 + u.CheckAmount ?? 0,
                             SalesDeduct = u.ReturnAmount ?? 0,
                             SalesDiscount = u.DiscountAmount ?? 0,
                             PaymentAmount = 0,
                             PurchaseDeduct = 0,
                             PurchaseDiscount = 0,
                         };

            var query = query1.Union(query2).Union(query3)
                .OrderBy(x=>x.DealDate);

            return query;
        }
    }
}
