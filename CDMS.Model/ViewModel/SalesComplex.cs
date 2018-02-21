using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class SalesViewModel : BaseSales
    {
        public string CompanyName { get; set; }        
    }

    public class SalesDetailViewModel : SalesDetail
    {
        public string ProductName { get; set; }
        public Boolean IsDirty { get; set; }        
    }
        
    public class SalesComplex
    {
        public SalesComplex()
        {
            this.Sales = new SalesViewModel(); 
            this.ChildList = new List<SalesDetailViewModel>();
        }

        public SalesViewModel Sales { get; set; }
        public List<SalesDetailViewModel> ChildList { get; set; }
    }

    // 銷貨歷史記錄
    public class SalesHistoryViewModel
    {

        public string ProductName { get; set; }

        public string ProductID { get; set; }

        // (詢價日期1+單價)
        public v_LatestSales Inquiry1 { get; set; }

        public v_LatestSales Inquiry2 { get; set; }

        public v_LatestSales Inquiry3 { get; set; }

        //  (倉庫1+倉庫2)
        public int Stock { get; set; }
    }
}
