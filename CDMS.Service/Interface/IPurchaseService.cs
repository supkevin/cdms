using CDMS.Model;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface IPurchaseService
    {
        void Create(Purchase model);
        void Update(Purchase model);
        void Delete(Purchase model);

        Purchase Get(string id);
        
        IQueryable<Purchase> GetAll();                
    }
}
