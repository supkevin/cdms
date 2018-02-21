using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class AlternativeViewModel : Alternative
    {
        [Required]
        public string AlternativeName { get; set; }
        public bool IsDirty { get; set; }

        //SeqNo
        //ProductID
        //AlternativeID
        //Remarks
    }

    public class AlternativeListViewModel : Alternative
    { 
        // Fake
        new public string ProductName {get;set;}
        new public string AlternativeName { get; set; }        
    }    
}
