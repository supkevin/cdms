using CDMS.Model;
using System.Collections.Generic;

namespace CDMS.Service
{
    public interface IOverTypeService
    {
        void Create(OverType model);

        void Update(OverType model);

        void Delete(OverType model);

        OverType Get(int id_overseatype);

        OverType GetById(int id_overseatype);

        IEnumerable<OverType> GetAll();
    }
}
