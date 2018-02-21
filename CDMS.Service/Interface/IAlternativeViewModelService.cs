using CDMS.Model;
using System.Collections.Generic;
using CDMS.Model.ViewModel;

namespace CDMS.Service
{
    public interface IAlternativeViewModelService
    {
        IEnumerable<AlternativeViewModel> GetAll();
    }
}
