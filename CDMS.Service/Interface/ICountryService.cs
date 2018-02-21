using CDMS.Model;
using System.Collections.Generic;

namespace CDMS.Service
{
    public interface ICountryService
    {
        void Create(Country model);
        void Update(Country model);
        void Delete(Country model);

        Country Get(int id_country);

        Country GetById(int id_country);

        IEnumerable<Country> GetAll();
    }
}
