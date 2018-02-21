using CDMS.Model;
using System.Collections.Generic;
namespace CDMS.Service
{
    public interface IObservationService
    {
        void Create(Observation model);
        void Update(Observation model);
        void Delete(Observation model);

        Observation Get(int id_observation);

        Observation GetById(int id_observation);

        IEnumerable<Observation> GetAll();

        List<Observation> GetForSelect(int id_feedback);
    }
}
