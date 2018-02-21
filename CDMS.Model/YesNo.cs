using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{

    public class YesNo : TextValue<YesNo>
    {       
        static List<YesNo> _Source = null;

        public YesNo(string value, string text) : base(value, text)
        {            
        }

        public static List<YesNo> GetAll()
        {
            if (null == _Source)
            {
                _Source = new List<Model.YesNo>();
                _Source.Add(YesNo.Yes);
                _Source.Add(YesNo.No);
            }

            return _Source;
        }
             
        public static YesNo FromValue(string value)
        {
            if (value == "Y")
                return Yes;
            if (value == "N")
                return No;
            return null;
        }

        public static YesNo Yes => new YesNo("Y", "是");
        public static YesNo No => new YesNo("N", "否");   
    }
}
