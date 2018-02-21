using CDMS.Model;
using System.Collections.Generic;

namespace CDMS.Service
{
    public interface IInspectionService
    {
        void Create(Inspection model);
        void Update(Inspection model);
        void Delete(Inspection model);
        void Close(Inspection model);
        
        Inspection Get(int id_inspection);

        Inspection GetById(int id_inspection);

        IEnumerable<Inspection> GetAll();

        Inspection GetSameToday(string cx_pid,string cx_date);
    }
}
