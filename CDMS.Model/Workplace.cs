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
    
    public partial class Workplace
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Workplace()
        {
            this.Inspection = new HashSet<Inspection>();
        }
    
        public int ID_Workplace { get; set; }
        public string CX_Workplace { get; set; }
        public int NQ_Sort { get; set; }
        public Nullable<int> FG_Department { get; set; }
        public string CX_Workplace_Remarks { get; set; }
        public string CX_Color { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Inspection> Inspection { get; set; }
    }
}
