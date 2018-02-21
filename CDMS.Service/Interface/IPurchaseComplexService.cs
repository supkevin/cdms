using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface IPurchaseComplexService
    {
        PurchaseComplex Create(PurchaseComplex model);

        void Update(PurchaseComplex model);
        void Delete(PurchaseComplex model);
        
        PurchaseComplex Get(string id);
        
        IQueryable<PurchaseComplex> GetAll();

        bool IsUsed(PurchaseComplex info);
    }
}
