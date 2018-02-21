using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface IStockQueryService
    {        
        IQueryable<StockQueryViewModel> GetAll();

        IQueryable<StockQueryViewModel> GetAlternativeList(string id);
    }
}
