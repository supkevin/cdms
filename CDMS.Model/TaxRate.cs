using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{

    public class TaxRate : CodeValue<TaxRate>
    {       
        static List<TaxRate> _Source = null;

        public TaxRate(int value, string text) : base(value, text)
        {            
        }

        public static List<TaxRate> GetAll()
        {
            if (null == _Source)
            {
                _Source = new List<Model.TaxRate>();
                _Source.Add(TaxRate.Five);
            }

            return _Source;
        }
             
        public static TaxRate FromValue(int value)
        {
            if (value == 5)
                return Five;          
            return null;
        }
        
        public static TaxRate Five => new TaxRate(5, "5%");
    }
}
