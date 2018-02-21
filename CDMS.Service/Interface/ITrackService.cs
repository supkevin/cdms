using CDMS.Model;
using System.Collections.Generic;

namespace CDMS.Service
{
    public interface ITrackService
    {
        void Create(Track model);
        void Update(Track model);
        void Delete(Track model);

        Track Get(int id_track);

        Track GetById(int id_track);

        IEnumerable<Track> GetAll();
    }
}
