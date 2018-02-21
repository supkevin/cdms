using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{

    public class Permission : TextValue<Permission>
    {       
        static List<Permission> _Source = null;

        public Permission(string value, string text) : base(value, text)
        {            
        }

        public static List<Permission> GetAll()
        {
            if (null == _Source)
            {
                _Source = new List<Model.Permission>();
                _Source.Add(Permission.Administrator);
                _Source.Add(Permission.Account);
                _Source.Add(Permission.Sales);
                _Source.Add(Permission.Warehouse);
                _Source.Add(Permission.Normal);
            }

            return _Source;
        }
             
        public static Permission FromValue(string value)
        {
            if (value == "1")
                return Administrator;
            if (value == "2")
                return Account;
            if (value == "3")
                return Sales;
            if (value == "4")
                return Warehouse;
            if (value == "5")
                return Normal;
            return null;
        }

        public static Permission Administrator => new Permission("1", "最高權限");
        public static Permission Account => new Permission("2", "會計權限");
        public static Permission Sales => new Permission("3", "業務權限");
        public static Permission Warehouse => new Permission("4", "倉管權限");
        public static Permission Normal => new Permission("5", "一般權限");
    }
}
