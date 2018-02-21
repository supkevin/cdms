using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CDMS.Service
{
    public interface IStockTrackService
    {
        IQueryable<StockTrackViewModel> GetSummary(DateTime? start, DateTime? finish);

        IQueryable<StockTrackViewModel> GetSummary(
            DateTime? start, DateTime? finish, 
            string productStart = "0", string productFinish = "Z", 
            string[] productKind = null, string[] warehouse = null);

        IQueryable<StockTrackDetailViewModel> GetDetails(
            DateTime? start, DateTime? finish, string productStart);

        IQueryable<StockTrackDetailViewModel> GetAll();
    }
}
