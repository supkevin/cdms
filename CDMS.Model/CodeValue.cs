using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{ 
    public abstract class CodeValue<T>
    {
        public string Text { get; set; }
        public int Value { get; set; }

        public CodeValue(int value, string text)
        {
            Value = value;
            Text = text;
        }
    }
}

