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
    
    public partial class Inspection
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Inspection()
        {
            this.Inspection_Image = new HashSet<Inspection_Image>();
        }
    
        public int ID_Inspection { get; set; }
        public int ID_OverSea { get; set; }
        public int ID_Country { get; set; }
        public int ID_Store { get; set; }
        public int ID_Workplace { get; set; }
        public int ID_Feedback { get; set; }
        public int ID_Observation { get; set; }
        public int ID_Track { get; set; }
        public int ID_Status { get; set; }
        public string ID_Department { get; set; }
        public string CX_PID { get; set; }
        public string CX_Name { get; set; }
        public string CX_Content { get; set; }
        public string CX_Improve { get; set; }
        public string CX_Date { get; set; }
        public string CX_Create { get; set; }
        public System.DateTime DT_Create { get; set; }
        public string CX_Modify { get; set; }
        public Nullable<System.DateTime> DT_Modfiy { get; set; }
        public string CX_DealWith { get; set; }
        public string CX_Process { get; set; }
        public Nullable<System.DateTime> DT_Process { get; set; }
        public string CX_Close { get; set; }
        public Nullable<System.DateTime> DT_Close { get; set; }
    
        public virtual Country Country { get; set; }
        public virtual Feedback Feedback { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Inspection_Image> Inspection_Image { get; set; }
        public virtual Observation Observation { get; set; }
        public virtual Store Store { get; set; }
        public virtual Track Track { get; set; }
        public virtual Workplace Workplace { get; set; }
    }
}
