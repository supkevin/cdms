using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface IPurchaseInvoiceComplexService
    {
        PurchaseInvoiceComplex Create(PurchaseInvoiceComplex model);

        void Update(PurchaseInvoiceComplex model);
        void Delete(PurchaseInvoiceComplex model);

        void RemoveChild(long id);

        PurchaseInvoiceComplex Get(string id);
        
        IQueryable<PurchaseInvoice> GetAll();

        bool IsDataExists(PurchaseInvoiceComplex info);
    }
}
