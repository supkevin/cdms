using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class UserViewModel : BaseUser
    {
        //[Required]
        //[MaxLength(10)]
        //new public string UserID { get; set; }

        //[Required]
        //[MaxLength(20)]
        //new public string UserName { get; set; }

        //[Required]
        //new public string TitleID { get; set; }

        //[Required]
        //new public Nullable<System.DateTime> OnboardDate { get; set; }

        //[Required]
        //new public string PermissionID { get; set; }

        //[Required]
        //new public string SupervisorID { get; set; }

        //new public string Telephone { get; set; }

        //new public string Address { get; set; }
        //new public string Email { get; set; }

        //new public string AnnualTarget { get; set; }

        //new public string QuotationLevelID { get; set; }

        //[Required]
        //new public string Password { get; set; }

        //[Required]
        //new public Nullable<System.DateTime> BeginDate { get; set; }

        //new public Nullable<System.DateTime> EndDate { get; set; }

        //new public string Remarks { get; set; }
    }

    public class UserListViewModel : User
    {
        // Fake
        public string TitleName { get; set; }
    }
}
