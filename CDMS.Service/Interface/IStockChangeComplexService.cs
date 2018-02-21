using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface IStockChangeComplexService
    {
        StockChangeComplex Create(StockChangeComplex model);

        void Update(StockChangeComplex model);
        void Delete(StockChangeComplex model);

        void RemoveChild(long id);

        StockChangeComplex Get(string id);

        IQueryable<StockChangeComplex> GetAll();

        IQueryable<StockChangeListViewModel> GetListView();

        bool IsUsed(StockChangeComplex info);
    }
}
