using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface ICompanyService
    {
        void Create(Company info);
        void Update(Company info);
        void Delete(Company info);

        Company Get(string id);
        
        IQueryable<CompanyViewModel> GetAll();

        bool IsDataExists(Company info);

        List<CompanyViewModel> GetForAutoComplete(string term, int count = 10);

        bool IsUsed(Company info);
    }
}
