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
    
    public partial class v_StockTrack
    {
        public string ChangeType { get; set; }
        public string ProductID { get; set; }
        public System.DateTime ChangeDate { get; set; }
        public string SourceID { get; set; }
        public string CompanyID { get; set; }
        public string WarehouseID { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string ProductName { get; set; }
        public string KindID { get; set; }
        public string CompanyName { get; set; }
        public string ChangeReason { get; set; }
        public Nullable<int> StockQty { get; set; }
    }
}
