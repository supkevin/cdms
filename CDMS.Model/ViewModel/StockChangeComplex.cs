using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class StockChangeViewModel : BaseStockChange
    {        
       public string ChangePersonName { get; set; }
    }

    public class StockChangeDetailViewModel : StockChangeDetail
    {
        public string ProductName { get; set; }
        public Boolean IsDirty { get; set; }
    }

    public class StockChangeComplex
    {
        public StockChangeComplex()
        {
            this.StockChange = new StockChangeViewModel();
            this.ChildList = new List<StockChangeDetailViewModel>();
        }

        public StockChangeViewModel StockChange { get; set; }
        public List<StockChangeDetailViewModel> ChildList { get; set; }
    }

    public class StockChangeListViewModel
    {
        public DateTime? ChangeDate { get; set; }
        public string StockChangeID { get; set; }
        public string ChangeReasonID { get; set; }
        public string ChangePersonID { get; set; }
        public string WarehouseOldID { get; set; }
        public string WarehouseNewID { get; set; }
        public string ProductID { get; set; }
        public string Remarks { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public string ChangePersonName { get; set; }
    }
}
