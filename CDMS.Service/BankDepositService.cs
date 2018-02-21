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
    public class BankDepositService : IBankDepositService
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IRepository<Model.BankDeposit> _Repository;
        private readonly IRepository<Model.BankAccount> _BankAccount;
        private readonly IRepository<Model.Company> _Company;

        private readonly UserInfo _CurrentUser = IdentityService.GetUserData();

        public BankDepositService(IUnitOfWork unitofwork,
            IRepository<Model.BankDeposit> repository,
            IRepository<Model.BankAccount> bankAccount,
            IRepository<Model.Company> company
            )
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
            this._BankAccount = bankAccount;
            this._Company = company;
        }
       
        private Model.BankDeposit GetInfoOnCreate(BankDeposit info)
        {
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Model.BankDeposit GetInfoOnDelete(BankDeposit info)
        {
            info.Activate = YesNo.No.Value;
            info.LastPerson = _CurrentUser.UserID; ;
            info.LastUpdate = DateTime.Now;

            return info;
        }
        
        private BankDeposit GetInfoOnUpdate(BankDeposit source)
        {
            BankDeposit info = Mapper.Map<BankDeposit>(source);

            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        public void Create(BankDeposit model)
        {
            model = GetInfoOnCreate(model);
            this._Repository.Create(model);
            this._UnitOfWork.SaveChange();
        }

        public void Update(BankDeposit info)
        {
            BankDeposit query = GetInfoOnUpdate(info);

            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());

            this._Repository.Update(query);
            this._UnitOfWork.SaveChange();
        }

        public void Delete(BankDeposit info)
        {
            if (info == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());

            this._Repository.Delete(info);
            this._UnitOfWork.SaveChange();
        }

        public BankDeposit Get(int id)
        {
            return this._Repository.Get(x => x.SeqNo == id);
        }

        public IQueryable<BankDeposit> GetAll()
        {
            return this._Repository.GetAll();
        }

        public bool IsUsed(BankDeposit info)
        {
            //var query = this._Repository.Get(x => x.BankDepositID == info.BankDepositID);
            //var result = query.Product.Any();
            //this._Repository.HandleDetached(query); // 
            //return result;
            return true;
        }

        public IQueryable<BankDepositListViewModel> GetListView()
        {

            var query = from u in this.GetAll()
                        join c in this._Company.GetAll() on u.CompanyID equals c.CompanyID into cc
                        from c in cc.DefaultIfEmpty()                        
                        select new BankDepositListViewModel()
                        {
                            SeqNo = u.SeqNo,
                            SourceID = u.SourceID,
                            DealDate = u.DealDate,
                            ExpiryDate = u.ExpiryDate,
                            Summary = u.Summary,
                            DebitAmount = u.DebitAmount ?? 0 ,
                            CreditAmount = u.CreditAmount ?? 0,
                            CompanyID = u.CompanyID,
                            CheckNum = u.CheckNum,
                            BankID = u.BankID,
                            AccountID = u.AccountID,
                            BankAccountID = u.BankAccountID ?? 0,
                            CheckStatus = u.CheckStatus,
                            Activate = u.Activate,
                            CompanyName = c== null ? "" : c.ShortName
                        };

            return query; 
        }
    }
}
