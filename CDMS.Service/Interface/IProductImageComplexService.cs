using CDMS.Model;
using System.Collections.Generic;
using CDMS.Model.ViewModel;
using System.Linq;

namespace CDMS.Service
{
    public interface IProductImageComplexService
    {
        ProductImageComplex Create(ProductImageComplex model);

        //void Update(ProductImageComplex model);
        void Delete(int id);

        ProductImageComplex Get(string id);

        IQueryable<ProductImageComplex> GetAll();
    }
}
