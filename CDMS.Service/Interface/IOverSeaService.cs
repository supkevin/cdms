using CDMS.Model;
using System.Collections.Generic;

namespace CDMS.Service
{
    public interface IOverSeaService
    {
        void Create(OverSea model);
        void BatchCreate(List<OverSea> model);

        void Update(OverSea model);
        void Delete(OverSea model);

        OverSea Get(int id_oversea);

        OverSea GetById(int id_oversea);

        IEnumerable<OverSea> GetAll();

        IEnumerable<OverSea> GetForOverType(int id_overtype);


        IEnumerable<OverSea> GetForCountry(int id_country);

        OverSea GetForInspection(string cx_pid, string cx_date);
    }
}
