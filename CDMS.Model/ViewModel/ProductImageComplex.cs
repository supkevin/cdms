using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class ProductImageViewModel : ProductImage
    {             
    }
        
    public class ProductImageComplex
    {
        public ProductImageComplex()
        {
            this.Product = new Product(); 
            this.ChildList = new List<ProductImageViewModel>();
        }

        public Product Product { get; set; }
        public List<ProductImageViewModel> ChildList { get; set; }
    }
}
