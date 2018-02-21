using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class BrandViewModel : Brand
    {
        [Required]
        [MaxLength(4)]
        new public string BrandID { get; set; }

        [Required]
        [MaxLength(20)]
        new public string BrandName { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }
        
        [Required]        
        new public string Activate { get; set; }
    }
}
