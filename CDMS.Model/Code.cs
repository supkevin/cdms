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
    
    public partial class Code
    {
        public int CodeID { get; set; }
        public string CodeType { get; set; }
        public string CodeValue { get; set; }
        public string CodeName { get; set; }
        public string Activate { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public string IsSystem { get; set; }
        public string LastPerson { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
    }
}
