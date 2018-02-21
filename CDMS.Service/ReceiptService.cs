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
    public class ReceiptService : IReceiptService
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IRepository<Model.Receipt> _Repository;
        private readonly IRepository<Model.BankAccount> _BankAccount;
        private readonly IRepository<Model.Company> _Company;
        private readonly IRepository<Model.BankDeposit> _BankDeposit;

        private readonly UserInfo _CurrentUser = IdentityService.GetUserData();

        public ReceiptService(IUnitOfWork unitofwork,
            IRepository<Model.Receipt> repository,
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

        private string GenerateID(Receipt info)
        {
            int seq = 1;
            string result = "";

            string key = $"R{DateTime.Today.ToString("yyMM")}";

            var current =
                this._Repository.GetAll()
                .Where(x => x.ReceiptID.StartsWith(key))
                .OrderByDescending(x => x.ReceiptID)
                .FirstOrDefault();

            if (current != null)
            {
                // 移除前面日期部分留下流水號
                seq = Convert.ToInt16(current.ReceiptID.Replace(key, ""));
                seq += 1;
            }

            result = $"{key}{seq.ToString().PadLeft(5, '0')}";
            return result;
        }

        private Model.Receipt GetInfoOnCreate(Receipt info)
        {
            info.ReceiptID = GenerateID(info);
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Model.Receipt GetInfoOnDelete(Receipt info)
        {
            info.Activate = YesNo.No.Value;
            info.LastPerson = _CurrentUser.UserID; ;
            info.LastUpdate = DateTime.Now;

            return info;
        }
        
        private Receipt GetInfoOnUpdate(Receipt source)
        {
            Receipt info = Mapper.Map<Receipt>(source);

            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private string GetDipositSummary(Receipt info) {
            return
                string.IsNullOrEmpty(info.CheckNum) ?
                DepositSummary.CashDeposit.Text : DepositSummary.BillCollection.Text;
        }

        // 處理銀行票據
        private void ProcessBankDeposit(Receipt info) {

            BankDeposit deposite = new BankDeposit()
            {
                DealDate = info.ReceiptDate,
                ExpiryDate = info.DueDate,
                Summary = GetDipositSummary(info),
                DebitAmount = 0,
                CreditAmount = info.CheckAmount + info.CashAmount,
                CompanyID = info.CustomerID,
                CheckNum = info.CheckNum,
                //BankID = info.BankID,
                //AccountID = info.AccountID,
                SourceID = info.ReceiptID,
                CheckStatus = CheckStatus.Uncounted.Value,
                Activate = YesNo.Yes.Value
            };

            // 先刪除舊資料
            var delate = 
                _BankDeposit.GetAll().Where(x => x.SourceID == info.ReceiptID).SingleOrDefault();

            if (delate != null)
            {
                _BankDeposit.Delete(delate);
            }

            _BankDeposit.Create(deposite);
        }

        public void Create(Receipt model)
        {
            model = GetInfoOnCreate(model);

            // 處理銀行票據資料
            ProcessBankDeposit(model);

            this._Repository.Create(model);
            this._UnitOfWork.SaveChange();
        }

        public void Update(Receipt info)
        {
            Receipt query = GetInfoOnUpdate(info);

            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());

            // 處理銀行票據資料
            ProcessBankDeposit(query);

            this._Repository.Update(query);
            this._UnitOfWork.SaveChange();
        }

        public void Delete(Receipt info)
        {
            if (info == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());

            this._Repository.Delete(info);
            this._UnitOfWork.SaveChange();
        }

        public Receipt Get(string id)
        {
            return this._Repository.Get(x => x.ReceiptID == id);
        }

        public IQueryable<Receipt> GetAll()
        {
            return this._Repository.GetAll();
        }

        public bool IsCheckNumExists(Receipt info )
        {
            if (string.IsNullOrEmpty(info.CheckNum.Trim())) return false; 
            var query = this._Repository
                .GetAll()
                .Any(x => x.CheckNum == info.CheckNum && x.ReceiptID != info.ReceiptID);

            return query;
        }

        public bool IsUsed(Receipt info)
        {
            //var query = this._Repository.Get(x => x.ReceiptID == info.ReceiptID);
            //var result = query.Product.Any();
            //this._Repository.HandleDetached(query); // 
            //return result;
            return true;
        }

        public IQueryable<ReceiptListViewModel> GetListView()
        {

            var query = from u in this._Repository.GetAll()
                        join c in this._Company.GetAll() on u.CustomerID equals c.CompanyID into cc
                        from c in cc.DefaultIfEmpty()
                        join b in this._BankAccount.GetAll() on u.BankAccountID equals b.SeqNo into bb
                        from b in bb.DefaultIfEmpty()
                        select new ReceiptListViewModel()
                        {
                            ReceiptID = u.ReceiptID,
                            CustomerID = u.CustomerID,
                            AccountMonth = u.AccountMonth,
                            BankAccountID = u.BankAccountID ?? 0, 
                            ReceiptDate = u.ReceiptDate,
                            DueDate = u.DueDate,
                            CheckNum = u.CheckNum,
                            CheckAmount = u.CheckAmount ?? 0 ,
                            CashAmount = u.CashAmount ?? 0,
                            DiscountAmount = u.DiscountAmount ?? 0,
                            ReturnAmount = u.ReturnAmount ?? 0,
                            Remarks = u.Remarks,
                            CustomerName = c== null ? "" : c.ShortName,
                            BankName = b == null ? "" : b.BankName,
                            AccountID = b == null ? "" : b.AccountID,
                            Status = "1"
                        };

            return query; 
        }
    }
}
