using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface ISalesComplexService
    {
        SalesComplex Create(SalesComplex model);

        void Update(SalesComplex model);
        void Delete(SalesComplex model);

        void RemoveChild(long id);

        SalesComplex Get(string id);
        
        IQueryable<SalesComplex> GetAll();

        bool IsUsed(SalesComplex info);

        IQueryable<v_CustomerLatestSales> GetLatestSales(string productID, int count = 10);

        SalesHistoryViewModel GetHistory(string productID);
    }
}
