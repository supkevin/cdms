using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface IMenuComplexService
    {    
        IQueryable<MenuComplex> Get(string userID);
    }
}
