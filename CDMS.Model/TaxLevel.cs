using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{

    public class TaxLevel : TextValue<TaxLevel>
    {       
        static List<TaxLevel> _Source = null;

        public TaxLevel(string value, string text) : base(value, text)
        {            
        }

        public static List<TaxLevel> GetAll()
        {
            if (null == _Source)
            {
                _Source = new List<Model.TaxLevel>();
                _Source.Add(TaxLevel.TaxInclude);
                _Source.Add(TaxLevel.TaxExclude);
                _Source.Add(TaxLevel.ZeroTax);
                _Source.Add(TaxLevel.DutyFree);
            }

            return _Source;
        }
             
        public static TaxLevel FromValue(string value)
        {
            if (value == "1")
                return TaxInclude;
            if (value == "2")
                return TaxExclude;
            if (value == "3")
                return ZeroTax;
            if (value == "4")
                return DutyFree;
            return null;
        }
        
        public static TaxLevel TaxInclude => new TaxLevel("1", "應稅內含");
        public static TaxLevel TaxExclude => new TaxLevel("2", "應稅外加");
        public static TaxLevel ZeroTax => new TaxLevel("3", "零稅率");
        public static TaxLevel DutyFree => new TaxLevel("4", "免稅");        
    }
}
