using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{
    /// <summary>
    /// 交易項目
    /// </summary>
    public class DealItem : TextValue<DealItem>
    {       
        static List<DealItem> _Source = null;

        public DealItem(string value, string text) : base(value, text)
        {            
        }

        public static List<DealItem> GetAll()
        {
            if (null == _Source)
            {
                _Source = new List<Model.DealItem>();
                _Source.Add(DealItem.Purchase);
                _Source.Add(DealItem.Sales);
                _Source.Add(DealItem.PurchaseInvoice);
                _Source.Add(DealItem.SalesInvoice);
                _Source.Add(DealItem.Receipt);
                _Source.Add(DealItem.Payment);
            }

            return _Source;
        }
             
        public static DealItem FromValue(string value)
        {
            if (value == "1")
                return Purchase;
            if (value == "2")
                return Sales;
            if (value == "3")
                return PurchaseInvoice;
            if (value == "4")
                return SalesInvoice;
            if (value == "5")
                return Receipt;
            if (value == "6")
                return Payment;
            return null;
        }

        // 進貨、銷貨、進貨發票、銷貨發票、收款、付款

        public static DealItem Purchase => new DealItem("1", "進貨");
        public static DealItem Sales => new DealItem("2", "銷貨");
        public static DealItem PurchaseInvoice => new DealItem("3", "進貨發票");
        public static DealItem SalesInvoice => new DealItem("4", "銷貨發票");
        public static DealItem Receipt => new DealItem("5", "收款");
        public static DealItem Payment => new DealItem("6", "付款");
    }
}
