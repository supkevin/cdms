using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Web.ViewModels
{
    public class AddressEditorViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        public string IsDeleted { get; set; }
    }

    public class PersonEditViewModel
    {
        public IEnumerable<AddressEditorViewModel> Addresses { get; set; }
    }
}