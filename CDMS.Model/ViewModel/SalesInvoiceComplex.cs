using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class SalesInvoiceViewModel : BaseSalesInvoice
    {
        public string CompanyName { get; set; }        
    }

    public class SalesInvoiceDetailViewModel : BaseSalesInvoiceDetail
    {
        public string ProductName { get; set; }
        public Boolean IsDirty { get; set; }        
    }
        
    public class SalesInvoiceComplex
    {
        public SalesInvoiceComplex()
        {
            this.Invoice = new SalesInvoiceViewModel(); 
            this.ChildList = new List<SalesInvoiceDetailViewModel>();
        }

        public SalesInvoiceViewModel Invoice { get; set; }
        public List<SalesInvoiceDetailViewModel> ChildList { get; set; }
    }
}
