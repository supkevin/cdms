using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class NewsViewModel : News
    {      
        [Required]
        new public DateTime? ReleaseDate { get; set; }

        [Required]
        new public DateTime? OffDate { get; set; }

        [Required]
        new public string DepartmentID { get; set; }

        [Required]
        new public string NewsTypeID { get; set; }

        [Required]
        new public string SetTop { get; set; }

        [Required]
        new public string Content { get; set; }

    }
}
