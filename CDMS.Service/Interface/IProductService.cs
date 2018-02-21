using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface IProductService
    {
        void Create(Product model);
        void Update(Product model);
        void Delete(Product model);

        Product Get(string id);

        IQueryable<ProductViewModel> GetAll();

        IQueryable<ProductListViewModel> GetListView();

        List<ProductViewModel> GetForSelect(string id);

        List<ProductViewModel> GetForAutoComplete(string term, int count = 10);

        bool IsDataExists(Product info);

        bool IsUsed(Product info);
    }
}
