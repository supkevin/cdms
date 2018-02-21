using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{    
    public class MyCodeValue
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public Boolean Disabled { get; set; }

        public string Display
        {
            get { return $"{ this.Value }-{ this.Text}"; }
        }
    }
}

