using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface IAlternativeService
    {
        void Create(Alternative model);
        void Update(Alternative model);
        void Delete(Alternative model);

        Alternative Get(long id);
        
        IEnumerable<Alternative> GetAll();

        IQueryable<AlternativeListViewModel> GetListView();
    }
}
