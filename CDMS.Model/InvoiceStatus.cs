using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{

    public class InvoiceStatus : TextValue<InvoiceStatus>
    {       
        static List<InvoiceStatus> _Source = null;

        public InvoiceStatus(string value, string text) : base(value, text)
        {            
        }

        public static List<InvoiceStatus> GetAll()
        {
            if (null == _Source)
            {
                _Source = new List<Model.InvoiceStatus>();
                _Source.Add(InvoiceStatus.Valid);
                _Source.Add(InvoiceStatus.Invalid);
            }

            return _Source;
        }
             
        public static InvoiceStatus FromValue(string value)
        {
            if (value == "1")
                return Valid;
            if (value == "0")
                return Invalid;           
            return null;
        }
        
        public static InvoiceStatus Valid => new InvoiceStatus("1", "正常開立");
        public static InvoiceStatus Invalid => new InvoiceStatus("0", "作廢");   
    }
}
