using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class ReceiptViewModel : BaseReceipt
    {        
               
    }

    public class ReceiptListViewModel
    {
        //收款日期
        //客戶名稱
        //現金金額
        //支票號碼
        //到期日
        //票面金額
        //託收(存入)行庫
        //帳號
        //票據狀況
        //備註
        public string ReceiptID { get; set; }
        public string CustomerID { get; set; }
        public string AccountMonth { get; set; }
        public int BankAccountID { get; set; }
        public DateTime? ReceiptDate { get; set; }         //收款日期
        public DateTime? DueDate { get; set; }
        public string CheckNum { get; set; }
        public decimal CheckAmount { get; set; }
        public decimal CashAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ReturnAmount { get; set; }
        public string Remarks { get; set; }

        // Fake 
        public string CustomerName { get; set; }        //客戶名稱
        public string BankName { get; set; }            //託收(存入)行庫
        public string AccountID { get; set; }           //帳號
        public string Status { get; set; }              //票據狀況
    }
}
