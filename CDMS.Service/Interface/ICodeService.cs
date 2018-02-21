using CDMS.Model;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface ICodeService
    {
        void Create(Code info);
        void Update(Code info);
        void Delete(Code info);

        Code Get(long id);
        
        IEnumerable<Code> GetAll();

        bool IsCodeExists(Code info);

        bool IsUsed(Code info);
    }
}
