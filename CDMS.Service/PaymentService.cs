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
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IRepository<Model.Payment> _Repository;
        private readonly IRepository<Model.BankAccount> _BankAccount;
        private readonly IRepository<Model.Company> _Company;
        private readonly IRepository<Model.BankDeposit> _BankDeposit;

        private readonly UserInfo _CurrentUser = IdentityService.GetUserData();

        public PaymentService(IUnitOfWork unitofwork,
            IRepository<Model.Payment> repository,
            IRepository<Model.BankAccount> bankAccount,
            IRepository<Model.Company> company,
            IRepository<Model.BankDeposit> bankDeposit
            )
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
            this._BankAccount = bankAccount;
            this._Company = company;
            this._BankDeposit = bankDeposit;
        }

        private string GenerateID(Payment info)
        {
            int seq = 1;
            string result = "";

            string key = $"Z{DateTime.Today.ToString("yyMM")}";

            var current =
                this._Repository.GetAll()
                .Where(x => x.PaymentID.StartsWith(key))
                .OrderByDescending(x => x.PaymentID)
                .FirstOrDefault();

            if (current != null)
            {
                // 移除前面日期部分留下流水號
                seq = Convert.ToInt16(current.PaymentID.Replace(key, ""));
                seq += 1;
            }

            result = $"{key}{seq.ToString().PadLeft(5, '0')}";
            return result;
        }

        private Model.Payment GetInfoOnCreate(Payment info)
        {
            info.PaymentID = GenerateID(info);
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Model.Payment GetInfoOnDelete(Payment info)
        {
            info.Activate = YesNo.No.Value;
            info.LastPerson = _CurrentUser.UserID; ;
            info.LastUpdate = DateTime.Now;

            return info;
        }
        
        private Payment GetInfoOnUpdate(Payment source)
        {
            Payment info = Mapper.Map<Payment>(source);

            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private string GetDipositSummary(Payment info)
        {
            return
                string.IsNullOrEmpty(info.CheckNum) ?
                DepositSummary.CashWithdraw.Text : DepositSummary.TicketOpened.Text;
        }

        // 處理銀行票據
        private void ProcessBankDeposit(Payment info)
        {

            BankDeposit deposite = new BankDeposit()
            {
                DealDate = info.PayDate,
                ExpiryDate = info.DueDate,
                Summary = GetDipositSummary(info),
                DebitAmount = info.CheckAmount + info.CashAmount,
                CreditAmount = 0,
                CompanyID = info.SupplierID,
                CheckNum = info.CheckNum,
                //BankID = info.BankID,
                //AccountID = info.AccountID,
                SourceID = info.PaymentID,
                CheckStatus = CheckStatus.Uncounted.Value,
                Activate = YesNo.Yes.Value
            };

            // 先刪除舊資料
            var delate =
                _BankDeposit.GetAll().Where(x => x.SourceID == info.PaymentID).SingleOrDefault();

            if (delate != null)
            {
                _BankDeposit.Delete(delate);
            }

            _BankDeposit.Create(deposite);
        }

        public void Create(Payment model)
        {
            model = GetInfoOnCreate(model);
            
            // 處理銀行票據資料
            ProcessBankDeposit(model);

            this._Repository.Create(model);
            this._UnitOfWork.SaveChange();
        }

        public void Update(Payment info)
        {
            Payment query = GetInfoOnUpdate(info);

            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            
            // 處理銀行票據資料
            ProcessBankDeposit(query);

            this._Repository.Update(query);
            this._UnitOfWork.SaveChange();
        }

        public void Delete(Payment info)
        {
            if (info == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());

            this._Repository.Delete(info);
            this._UnitOfWork.SaveChange();
        }

        public Payment Get(string id)
        {
            return this._Repository.Get(x => x.PaymentID == id);
        }

        public IQueryable<Payment> GetAll()
        {
            return this._Repository.GetAll();
        }

        public bool IsCheckNumExists(Payment info)
        {
            if (string.IsNullOrEmpty(info.CheckNum.Trim())) return false;

            var query = this._Repository
                .GetAll()
                .Any(x => x.CheckNum == info.CheckNum && x.PaymentID != info.PaymentID);

            return query;
        }

        public bool IsUsed(Payment info)
        {
            //var query = this._Repository.Get(x => x.PaymentID == info.PaymentID);
            //var result = query.Product.Any();
            //this._Repository.HandleDetached(query); // 
            //return result;
            return true;
        }

        public IQueryable<PaymentListViewModel> GetListView()
        {

            var query = from u in this._Repository.GetAll()
                        join c in this._Company.GetAll() on u.SupplierID equals c.CompanyID into cc
                        from c in cc.DefaultIfEmpty()
                        join b in this._BankAccount.GetAll() on u.BankAccountID equals b.SeqNo into bb
                        from b in bb.DefaultIfEmpty()
                        select new PaymentListViewModel()
                        {
                            PaymentID = u.PaymentID,
                            SupplierID = u.SupplierID,
                            AccountMonth = u.AccountMonth,
                            BankAccountID = u.BankAccountID ?? 0,
                            PayDate = u.PayDate,
                            DueDate = u.DueDate,
                            CheckNum = u.CheckNum,
                            CheckAmount = u.CheckAmount ?? 0,
                            CashAmount = u.CashAmount ?? 0,
                            DiscountAmount = u.DiscountAmount ?? 0,
                            ReturnAmount = u.ReturnAmount ?? 0,
                            Remarks = u.Remarks,
                            SupplierName = c == null ? "" : c.ShortName,
                            BankName = b == null ? "" : b.BankName,
                            AccountID = b == null ? "" : b.AccountID,
                            Status = "1"
                        };

            return query; 
        }
    }
}
