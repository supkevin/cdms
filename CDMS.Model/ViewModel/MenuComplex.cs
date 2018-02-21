using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class MenuViewModel
    {
        public string Name { get; set; }
        public string ParentName { get; set; }
        public string Path { get; set; }
    }

    public class MenuComplex
    {       
        public MenuComplex()
        {
            this.Group = new Menu(); 
            this.ChildList = new List<Menu>();
        }

        public Menu Group { get; set; }
        public List<Menu> ChildList { get; set; }
    }
}
