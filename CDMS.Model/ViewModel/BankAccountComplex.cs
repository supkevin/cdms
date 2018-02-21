using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class BankAccountViewModel : BankAccount
    {        
        [Required]
        [MaxLength(10)]
        new public string BankID { get; set; }

        [Required]
        [MaxLength(20)]
        new public string AccountID { get; set; }

        [Required]
        [MaxLength(30)]
        new public string BankName { get; set; }

        [Required]
        [MaxLength(20)]
        new public string AccountName { get; set; }
        
        [MaxLength(15)]
        new public Nullable<int> InitialAmount { get; set; }

        new public DateTime? InitialDate { get; set; }

        new public string LastNumber { get; set; }

        new public DateTime? LastDate { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }
    }
}
