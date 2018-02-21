using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{

    public class PriceKind : TextValue<PriceKind>
    {       
        static List<PriceKind> _Source = null;

        public PriceKind(string value, string text) : base(value, text)
        {            
        }

        public static List<PriceKind> GetAll()
        {
            if (null == _Source)
            {
                _Source = new List<Model.PriceKind>();
                _Source.Add(PriceKind.ListPrice);
                _Source.Add(PriceKind.SetPrice);
                _Source.Add(PriceKind.RealPrice);
            }

            return _Source;
        }
             
        public static PriceKind FromValue(string value)
        {
            if (value == "1")
                return ListPrice;
            if (value == "2")
                return SetPrice;
            if (value == "3")
                return RealPrice;
            return null;
        }

        public static PriceKind ListPrice => new PriceKind("1", "牌價");
        public static PriceKind SetPrice => new PriceKind("2", "定價");
        public static PriceKind RealPrice => new PriceKind("3", "實價");
    }
}
