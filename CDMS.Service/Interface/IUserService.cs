using CDMS.Model.ViewModel;
using CDMS.Model;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface IUserService
    {
        void Create(User info);
        void Update(User info);
        void Delete(User info);

        User Get(string id);
        
        IQueryable<UserViewModel> GetAll();

        IQueryable<UserListViewModel> GetListView();

        bool IsDataExists(User info);
        bool IsUsed(User info);

        List<UserViewModel> GetForAutoComplete(string term, int count = 10);

    }
}
