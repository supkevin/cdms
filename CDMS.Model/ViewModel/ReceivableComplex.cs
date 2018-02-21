using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class ReceivableViewModel : BaseReceivable
    {
         
    }

    public class ReceivableDetailViewModel
    {
        public string VoucherID { get; set; }                 //憑證編號
        public string ProductID { get; set; }                 //產品編號
        public string ProductName { get; set; }               //產品名稱
        public int Qty { get; set; }                          //數量
        public decimal Price { get; set; }                    //單價
        public decimal Amount { get; set; }                   //金額          
    }

    public class ReceivableSummaryViewModel
    {   
        public string AccountMonth { get; set; }               //帳款月份
        public string CompanyID { get; set; }                  //廠商編號
        public string CompanyName { get; set; }                //廠商名稱
        public decimal LastBalance { get; set; }               //上期餘額
        public decimal SalesAmountTotal { get; set; }          //銷貨金額
        public decimal SalesTaxTotal  { get; set; }            //銷貨稅額
        public decimal PurchaseAmountTotal { get; set; }       //進貨金額
        public decimal PurchaseTaxTotal { get; set; }          //進貨稅額
        public decimal ReceiptAmountTotal { get; set; }        //本期已收款金額
        public decimal PaymentAmountTotal { get; set; }        //本期已付款金額
        public decimal SalesDiscountTotal { get; set; }        //銷貨折讓金額合計
        public decimal SalesDeductTotal { get; set; }          //銷貨抵扣金額合計
        public decimal PurchaseDiscountTotal { get; set; }     //進貨折讓金額合計
        public decimal PurchaseDeductTotal { get; set; }       //進貨抵扣金額合計
        public decimal Balance { get; set; }                   //本期餘額  
        public decimal Remaining { get; set; }                 //本期小計
        public DateTime? AccountDate { get; set; }             //帳款日期方便計算上下月

    }

    // 對帳單顯示格式
    public class ReconciliationViewModel
    {
        public string AccountMonth { get; set; }               //帳款月份
        public string CompanyID { get; set; }                  //廠商編號
        public string DealItem { get; set; }                   //
        public DateTime? DealDate { get; set; }                //
        public string VoucherID { get; set; }                  //
        public decimal SalesAmount { get; set; }
        public decimal SalesTax { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal PurchaseTax { get; set; }
        public decimal ReceiptAmount { get; set; }
        public decimal SalesDeduct { get; set; }
        public decimal SalesDiscount { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal PurchaseDeduct { get; set; }
        public decimal PurchaseDiscount { get; set; }

        // 本期已收金額
        public decimal ReceivedAmount {
            get {
                return this.ReceiptAmount - this.SalesDeduct - this.SalesDiscount;
            }
        }

        // 本期已付金額
        public decimal PaidAmount
        {
            get
            {
                return this.PaymentAmount - this.PurchaseDeduct - this.PurchaseDiscount;
            }
        }
    }
}
