using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class PaymentViewModel : BasePayment
    {        
               
    }

    public class PaymentListViewModel
    {
        //付款日期
        //供應商名稱
        //現金金額
        //付款行庫
        //帳號
        //支票號碼
        //支票金額
        //到期日
        //票據狀況
        //備註

        public string PaymentID { get; set; }
        public string SupplierID { get; set; }
        public int BankAccountID { get; set; }
        public string AccountMonth { get; set; }
        public DateTime? PayDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string CheckNum { get; set; }
        public decimal CheckAmount { get; set; }
        public decimal CashAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ReturnAmount { get; set; }
        public string Remarks { get; set; }
        public string Activate { get; set; }

        // Fake 
        public string SupplierName { get; set; }        //客戶名稱
        public string BankName { get; set; }            //託收(存入)行庫
        public string AccountID { get; set; }           //帳號
        public string Status { get; set; }              //票據狀況
    }
}
