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
    
    public partial class StockChange
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StockChange()
        {
            this.StockChangeDetail = new HashSet<StockChangeDetail>();
        }
    
        public string StockChangeID { get; set; }
        public string ChangeReasonID { get; set; }
        public string WarehouseOldID { get; set; }
        public string WarehouseNewID { get; set; }
        public string ChangePersonID { get; set; }
        public System.DateTime ChangeDate { get; set; }
        public string Remarks { get; set; }
        public string LastPerson { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockChangeDetail> StockChangeDetail { get; set; }
    }
}
