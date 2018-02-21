using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CDMS.Service
{
    public interface IBankDepositService
    {
        void Create(BankDeposit info);
        void Update(BankDeposit info);
        void Delete(BankDeposit info);

        BankDeposit Get(int id);
        
        IQueryable<BankDeposit> GetAll();

        bool IsUsed(BankDeposit info);

        IQueryable<BankDepositListViewModel> GetListView();
    }
}
