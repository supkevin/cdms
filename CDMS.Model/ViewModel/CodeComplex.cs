using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class CodeViewModel : Code
    {
        new public string CodeID { get; set; }

        [Required]
        new public string CodeType { get; set; }

        [Required]
        [MaxLength(10)]
        new public string CodeValue { get; set; }

        [Required]
        [MaxLength(10)]
        new public string CodeName { get; set; }

        [Required]
        new public string Activate { get; set; }
                
        new public string SortOrder { get; set; }
    }
}
