using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class PurchaseInvoiceViewModel : BasePurchaseInvoice
    {
        public string CompanyName { get; set; }
    }

    public class PurchaseInvoiceDetailViewModel : BasePurchaseInvoiceDetail
    {
        public string ProductName { get; set; }
        public Boolean IsDirty { get; set; }        
    }
        
    public class PurchaseInvoiceComplex
    {
        public PurchaseInvoiceComplex()
        {
            this.Invoice = new PurchaseInvoiceViewModel(); 
            this.ChildList = new List<PurchaseInvoiceDetailViewModel>();            
        }

        public PurchaseInvoiceViewModel Invoice { get; set; }
        public List<PurchaseInvoiceDetailViewModel> ChildList { get; set; }
    }
}
