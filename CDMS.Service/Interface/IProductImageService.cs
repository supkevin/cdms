using CDMS.Model;
using System.Collections.Generic;

namespace CDMS.Service
{
    public interface IProductImageService
    {

        ProductImage Get(int id_ProductImage);

        void Delete(ProductImage model);

        void DeleteWithOutSaveChange(string productID);

        IEnumerable<ProductImage> GetForInspection(string productID);      
    }
}
