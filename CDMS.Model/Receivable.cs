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
    
    public partial class Receivable
    {
        public string VoucherID { get; set; }
        public string CompanyID { get; set; }
        public string AccountMonth { get; set; }
        public Nullable<System.DateTime> DealDate { get; set; }
        public string DealItem { get; set; }
        public Nullable<decimal> SalesAmount { get; set; }
        public Nullable<decimal> SalesTax { get; set; }
        public Nullable<decimal> PurchaseAmount { get; set; }
        public Nullable<decimal> PurchaseTax { get; set; }
        public string LastPerson { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
    }
}
