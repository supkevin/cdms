using CDMS.Model;
using System.Collections.Generic;
namespace CDMS.Service
{
    public interface IUnitService
    {
        Code Get(string id);
        
        IEnumerable<Code> GetAll();

        List<Code> GetForSelect(string id);
    }
}
