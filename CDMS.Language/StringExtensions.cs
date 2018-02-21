using System;

namespace CDMS.Language
{
    //依語系產生i18n
    public static class StringExtensions
    {
        //依語系產生i18n
        public static string ToLocalized(this string value)
        {
            return CDMS.Language.Resources.ResourceManager.GetString(value);

            //try
            //{
            //    return CDMS.Language.Resources.ResourceManager.GetString(value);
            //}
            //catch (Exception ex)
            //{
            //    return value;
            //}            
        }

        public static string CultureName(this string value)
        {
            return System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
        }

        public static string ToLocalized(this string nameSpace, string value)
        {
            string resKey = (string.IsNullOrEmpty(nameSpace) ? string.Empty : nameSpace + ".") + value;
            return CDMS.Language.Resources.ResourceManager.GetString(resKey);
        }


    }
}
