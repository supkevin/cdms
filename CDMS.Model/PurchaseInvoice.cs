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
    
    public partial class PurchaseInvoice
    {
        public string InvoiceID { get; set; }
        public string AccountMonth { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string SupplierID { get; set; }
        public string Title { get; set; }
        public string TaxID { get; set; }
        public string TaxLevelID { get; set; }
        public Nullable<decimal> TaxExcluded { get; set; }
        public Nullable<decimal> TaxIncluded { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> DeductAmount { get; set; }
        public Nullable<int> TaxRate { get; set; }
        public string InvoiceStatusID { get; set; }
        public string Remarks { get; set; }
        public string Activate { get; set; }
        public string LastPerson { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
    }
}
