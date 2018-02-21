using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface IQuotationComplexService
    {
        QuotationComplex Create(QuotationComplex model);

        void Update(QuotationComplex model);
        void Audit(QuotationComplex model);

        void Delete(QuotationComplex model);

        void RemoveChild(long id);

        QuotationComplex Get(string id);
        
        IQueryable<QuotationComplex> GetAll();
        bool IsUsed(QuotationComplex info);
    }
}
