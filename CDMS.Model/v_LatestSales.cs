//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace CDMS.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class v_LatestSales
    {
        public string CustomerID { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string PriceKindID { get; set; }
        public string ConditionID { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> LatestPrice { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public System.DateTime SalesDate { get; set; }
        public Nullable<long> RowIndex { get; set; }
    }
}
