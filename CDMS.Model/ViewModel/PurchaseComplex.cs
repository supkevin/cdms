using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class PurchaseViewModel : BasePurchase
    {
        public string CompanyName { get; set; }        
    }

    public class PurchaseDetailViewModel : PurchaseDetail
    {
        public string ProductName { get; set; }
        public Boolean IsDirty { get; set; }        
    }
        
    public class PurchaseComplex
    {
        public PurchaseComplex()
        {
            this.Purchase = new PurchaseViewModel(); 
            this.ChildList = new List<PurchaseDetailViewModel>();
        }

        public PurchaseViewModel Purchase { get; set; }
        public List<PurchaseDetailViewModel> ChildList { get; set; }
    }
}
