using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CDMS.Service
{
    public interface IProductSalesRankingService
    {
        IQueryable<ProductSalesRankingViewModel> GetAll(DateTime start, DateTime finish);
    }
}
