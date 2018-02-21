using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CDMS.Model.ViewModel
{
    public class QuotationViewModel : BaseQuotation
    {
        public string CompanyName { get; set; }
        public string QuotePersonName { get; set; }

        [Obsolete("Don't use this", true)]
        [Browsable(false)]
        new private string Auditor { get; set; }

        [Obsolete("Don't use this", true)]
        private new DateTime? ReviewDate { get; set; }

        [Obsolete("Don't use this", true)]
        private new string Result { get; set; }

        [Obsolete("Don't use this", true)]
        private new string SaleID { get; set; }

        [Obsolete("Don't use this", true)]
        private new DateTime? SaleDate { get; set; }     
    }
   
    public class QuotationDetailViewModel : BaseQuotationDetail
    {
        public string ProductName { get; set; }
        public Boolean IsDirty { get; set; }        
    }
        
    public class QuotationComplex
    {
        public QuotationComplex()
        {
            this.Quotation = new QuotationViewModel(); 
            this.ChildList = new List<QuotationDetailViewModel>();
        }

        public QuotationViewModel Quotation { get; set; }
        public List<QuotationDetailViewModel> ChildList { get; set; }
    }
}
