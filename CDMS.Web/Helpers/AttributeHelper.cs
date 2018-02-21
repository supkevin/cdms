using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace CDMS.Web.Helpers
{
    public static class HtmlAttributeHelper
    {
        // 合併屬性
        public static IDictionary<string, object> MergeAttributes(
            this HtmlHelper helper, object htmlAttributesObject, object defaultHtmlAttributesObject)
        {
            var concatKeys = new string[] { "class" };

            var htmlAttributesDict = htmlAttributesObject as IDictionary<string, object>;
            var defaultHtmlAttributesDict = defaultHtmlAttributesObject as IDictionary<string, object>;

            RouteValueDictionary htmlAttributes = (htmlAttributesDict != null)
                ? new RouteValueDictionary(htmlAttributesDict)
                : HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributesObject);
            RouteValueDictionary defaultHtmlAttributes = (defaultHtmlAttributesDict != null)
                ? new RouteValueDictionary(defaultHtmlAttributesDict)
                : HtmlHelper.AnonymousObjectToHtmlAttributes(defaultHtmlAttributesObject);

            foreach (var item in htmlAttributes)
            {
                // 可累加的屬性
                if (concatKeys.Contains(item.Key))
                {
                    // 屬性存在累加
                    defaultHtmlAttributes[item.Key] = (defaultHtmlAttributes[item.Key] != null)
                        ? string.Format("{0} {1}", defaultHtmlAttributes[item.Key], item.Value)
                        : item.Value;
                }
                else
                {
                    if (string.IsNullOrEmpty(item.Value.ToString())  && defaultHtmlAttributes.Keys.Contains(item.Key))
                    {
                        defaultHtmlAttributes.Remove(item.Key);
                    }
                    else
                    {
                        defaultHtmlAttributes[item.Key] = item.Value;
                    }
                }
            }

            return defaultHtmlAttributes;
        }
    }
}