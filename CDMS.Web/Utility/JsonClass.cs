using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
namespace CDMS.Web
{
    public class JsonClass
    {
        public static string DataTableToJson(DataTable dt)
        {
            return JsonConvert.SerializeObject(dt, Formatting.Indented);
        }

        public static DataTable JsonToDataTable(string pJson)
        {
            return JsonConvert.DeserializeObject<DataTable>(pJson.Trim());
        }

        public static string ListToJson(object List)
        {
            return JsonConvert.SerializeObject(List, Formatting.Indented);
        }

        public static List<TResult> JsonToList<TResult>(string pJson)
        {
            return JsonConvert.DeserializeObject<List<TResult>>(pJson.Trim());
        }

        public static string ObjectToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static object JsonToObject<TResult>(string pJson)
        {
            return JsonConvert.DeserializeObject<TResult>(pJson);
        }
    }
}