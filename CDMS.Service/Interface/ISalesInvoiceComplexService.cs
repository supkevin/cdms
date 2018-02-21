using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface ISalesInvoiceComplexService
    {
        SalesInvoiceComplex Create(SalesInvoiceComplex model);

        void Update(SalesInvoiceComplex model);
        void Delete(SalesInvoiceComplex model);

        void RemoveChild(long id);

        SalesInvoiceComplex Get(string id);
        
        IQueryable<SalesInvoice> GetAll();

        bool IsDataExists(SalesInvoiceComplex info);
    }
}
