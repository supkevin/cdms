using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CDMS.Service
{
    public interface IPaymentService
    {
        void Create(Payment info);
        void Update(Payment info);
        void Delete(Payment info);

        Payment Get(string id);

        IQueryable<Payment> GetAll();

        bool IsCheckNumExists(Payment info);
        bool IsUsed(Payment info);

        IQueryable<PaymentListViewModel> GetListView();
    }
}
