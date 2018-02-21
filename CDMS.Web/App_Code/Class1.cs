using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.ComponentModel;

namespace CDMS.Web.App_Code
{
    public static class DynamicExtension
    {
        public static dynamic ToDynamic(this Object obj)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj.GetType()))
                expando.Add(property.Name, property.GetValue(obj));
            return expando as ExpandoObject;
        }
    }
}