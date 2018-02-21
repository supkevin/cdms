using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CDMS.Service
{
    public interface IReceivableService
    {
        void Create(Receivable info);
        void Update(Receivable info);
        void Delete(Receivable info);

        Receivable Get(string id);

        IQueryable<Receivable> GetAll();

        bool IsUsed(Receivable info);

        IQueryable<ReceivableSummaryViewModel> GetListView();
        IQueryable<ReceivableDetailViewModel> GetDetailListView();

        // 對帳單
        IQueryable<ReconciliationViewModel> GetReconciliationListView();

        void Initialize(string accountMonth);
    }
}
