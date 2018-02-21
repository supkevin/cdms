using CDMS.Model;
using System.Collections.Generic;
namespace CDMS.Service
{
    public interface IProductKindService
    {
        //void Create(Code model);
        //void Update(vProductKind model);
        //void Delete(vProductKind model);

        Code Get(string id);
        
        IEnumerable<Code> GetAll();

        List<Code> GetForSelect(string id);
    }
}
