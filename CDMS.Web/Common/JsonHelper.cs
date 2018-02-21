using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CDMS.Web.Common
{
    public class JsonHelper
    {
        public static string ObjectToJson(Object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
        public static object JsonToObject<TResult>(string json)
        {
            return JsonConvert.DeserializeObject<TResult>(json);
        }
    }

    
}
