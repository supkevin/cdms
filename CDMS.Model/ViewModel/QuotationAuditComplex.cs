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
    // 稽核狀態才打開Auditor
    public class QuotationAuditViewModel
    {      
        [Required]
        public new string Auditor { get; set; }

        [Required]
        public new DateTime? ReviewDate { get; set; }

        [Required]
        private new string Result { get; set; }
    }
        
    public class QuotationAuditComplex
    {
        public QuotationAuditComplex()
        {
            this.Quotation = new QuotationViewModel();
            this.Audit = new QuotationAuditViewModel();
            this.ChildList = new List<QuotationDetailViewModel>();
        }

        public QuotationViewModel Quotation { get; set; }
        public QuotationAuditViewModel Audit { get; set; }
        public List<QuotationDetailViewModel> ChildList { get; set; }
    }
}
