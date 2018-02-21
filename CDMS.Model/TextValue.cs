using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{ 
    public abstract class TextValue<T>
    {
        public string Text { get; set; }
        public string Value { get; set; }

        public TextValue(string value, string text)
        {
            Value = value;
            Text = text;
        }
    }
}

