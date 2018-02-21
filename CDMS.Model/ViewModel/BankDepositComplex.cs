using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class BankDepositViewModel : BaseBankDeposit
    {

    }

    public class SimpleBankDepositViewModel
    {
        public int SeqNo { get; set; }
        public int BankAccountID { get; set; }
        public string CheckStatus { get; set; }
    }

    public class BankDepositListViewModel
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

        public int SeqNo { get; set; }
        public string SourceID { get; set; }
        public DateTime? DealDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Summary { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string CompanyID { get; set; }
        public string CheckNum { get; set; }
        public string BankID { get; set; }
        public string AccountID { get; set; }
        public int BankAccountID { get; set; }
        public string CheckStatus { get; set; }
        public string Activate { get; set; }

        // Fake 
        public string CompanyName { get; set; }           //客戶名稱
        public decimal Amount { get { return this.DebitAmount + this.CreditAmount; } } //不論收款或付款      
    }
}
