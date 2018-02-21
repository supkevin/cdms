using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class InquiryViewModel : BaseInquiry
    {
        //[Required]
        public string CompanyName { get; set; }
    }

    public class InquiryDetailViewModel : BaseInquiryDetail
    {
        //[DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        //new public decimal? Price { get; set; }

        public string ProductName { get; set; }
        public Boolean IsDirty { get; set; }        
    }
        
    public class InquiryComplex
    {
        public InquiryComplex()
        {
            this.Inquiry = new InquiryViewModel(); 
            this.ChildList = new List<InquiryDetailViewModel>();
        }

        public InquiryViewModel Inquiry { get; set; }
        public List<InquiryDetailViewModel> ChildList { get; set; }        
    }

    // 詢價歷史記錄
    public class InquiryHistoryViewModel {

        public string ProductName { get; set; }

        public string ProductID { get; set; }

        // (詢價日期1+單價)
        public string Inquiry1 { get; set; }
                
        public string Inquiry2 { get; set; }
                
        public string Inquiry3 { get; set; }

        //  (倉庫1+倉庫2)
        public int Stock { get; set; }
    } 
}
