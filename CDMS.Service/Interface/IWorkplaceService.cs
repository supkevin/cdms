using CDMS.Model;
using System.Collections.Generic;

namespace CDMS.Service
{
    public interface IWorkplaceService
    {
        void Create(Workplace model);
        void Update(Workplace model);
        void Delete(Workplace model);

        Workplace Get(int id_workplace);

        Workplace GetById(int id_workplace);

        IEnumerable<Workplace> GetAll();
    }
}
