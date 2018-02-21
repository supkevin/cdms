
using System.Web.Mvc;

namespace CDMS.Web
{
    public static class ModelStateErrorClass
    {
        public static string FormatToString(ModelStateDictionary ms)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int i = 1;
            foreach (var modelStateVal in ms.Values)
            {
                foreach (var error in modelStateVal.Errors)
                {
                    sb.AppendFormat(i + "." + error.ErrorMessage + "<br/>");
                    i++;
                }
            }
            return sb.ToString();
        }


        public static string FormatToString(System.Data.Entity.Validation.DbEntityValidationException ms)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int i = 1;
            foreach (var modelStateVal in ms.EntityValidationErrors)
            {
                foreach (var error in modelStateVal.ValidationErrors)
                {
                    sb.AppendFormat(i + "." + error.ErrorMessage + "<br/>");
                    i++;
                }
            }
            return sb.ToString();
        }
    }
}