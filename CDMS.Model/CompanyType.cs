using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{

    public class CompanyType : TextValue<CompanyType>
    {       
        static List<CompanyType> _Source = null;

        public CompanyType(string value, string text) : base(value, text)
        {            
        }

        public static List<CompanyType> GetAll()
        {
            if (null == _Source)
            {
                _Source = new List<Model.CompanyType>();
                _Source.Add(CompanyType.Customer);
                _Source.Add(CompanyType.Suppiler);
                _Source.Add(CompanyType.Both);
            }

            return _Source;
        }
             
        public static CompanyType FromValue(string value)
        {
            if (value == "1")
                return Customer;
            if (value == "2")
                return Suppiler;
            if (value == "3")
                return Both;
            return null;
        }

        public static CompanyType Customer => new CompanyType("1", "客戶");
        public static CompanyType Suppiler => new CompanyType("2", "廠商");
        public static CompanyType Both => new CompanyType("3", "客戶+廠商");
    }
}
