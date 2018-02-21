using CDMS.Model;
using System.Collections.Generic;
namespace CDMS.Service
{
    public interface IBrandService
    {
        void Create(Brand info);
        void Update(Brand info);
        void Delete(Brand info);

        Brand Get(string id);
        
        IEnumerable<Brand> GetAll();

        List<Brand> GetForSelect(string id);
        
       bool IsCodeExists(Brand info);

       bool IsUsed(Brand info);
    }
}
