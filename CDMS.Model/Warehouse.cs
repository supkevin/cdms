using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{

    public class Warehouse : TextValue<Warehouse>
    {       
        static List<Warehouse> _Source = null;

        public Warehouse() : base("", "")
        {            
        }
        public Warehouse(string value, string text) : base(value, text)
        {            
        }

        public static List<Warehouse> GetAll()
        {
            if (null == _Source)
            {
                _Source = new List<Model.Warehouse>();
                _Source.Add(Warehouse.One);
                _Source.Add(Warehouse.Two);                
            }

            return _Source;
        }
             
        public static Warehouse FromValue(string value)
        {
            if (value == "1")
                return One;
            if (value == "2")
                return Two;
            return null;
        }

        public static Warehouse One => new Warehouse("1", "倉庫1");
        public static Warehouse Two => new Warehouse("2", "倉庫2");        
    }
}
