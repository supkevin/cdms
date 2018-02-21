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
    
    public partial class Sales
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sales()
        {
            this.SalesDetail = new HashSet<SalesDetail>();
        }
    
        public string SalesID { get; set; }
        public System.DateTime SalesDate { get; set; }
        public string CustomerID { get; set; }
        public string TaxID { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
        public string InvoiceAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingModeID { get; set; }
        public Nullable<int> ShippingFee { get; set; }
        public string WarehouseID { get; set; }
        public string AccountMonth { get; set; }
        public string Remarks { get; set; }
        public string InvoiceID { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<decimal> InvoiceAmount { get; set; }
        public Nullable<System.DateTime> PostingTime { get; set; }
        public string Activate { get; set; }
        public string LastPerson { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesDetail> SalesDetail { get; set; }
    }
}