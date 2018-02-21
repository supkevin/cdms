using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CDMS.Model.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        public string UserID { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}