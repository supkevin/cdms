using CDMS.Model;
using System.Collections.Generic;


namespace CDMS.Service
{
    public interface IStoreService
    {
        void Create(Store model);
        void Update(Store model);
        void Delete(Store model);

        Store Get(int id_store);

        Store GetById(int id_store);

        IEnumerable<Store> GetAll();

        List<Store> GetForSelect(int id_country);
    }
}
