using CDMS.Model;
using System.Collections.Generic;

namespace CDMS.Service
{
    public interface IProcessService
    {        
        void Update(Inspection model);
        
        Inspection Get(int id_inspection);

        Inspection GetById(int id_inspection);             
    }
}
