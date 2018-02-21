using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class ProductViewModel : BaseProduct
    {
               
    }

    public class ProductListViewModel : Product
    {
        // Fake
        public string SupplierName { get; set; }

        public string UnitName { get; set; }

        public string KindName { get; set; }

        public string BrandName { get; set; }
    }
           
    public class ProductComplex
    {
        public ProductComplex() {
            this.ChildList = new List<AlternativeViewModel>();
        }

        public Product Product { get; set; }
        public List<AlternativeViewModel> ChildList { get; set; }        
    }
}
