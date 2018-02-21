using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using CDMS.Language;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using CDMS.Web.Utility;
using System.Text;
using System.Web.Mvc.Html;
using System.Xml.Linq;

namespace CDMS.Web.Helpers
{
    // https://stackoverflow.com/questions/38226034/disable-enable-dropdownlistfor-based-on-model-property-in-mvc/47597356#47597356
    public static class DropDownHelper
    {
        public static MvcHtmlString MyDropDownListFor<TModel, TProperty>
                     (this HtmlHelper<TModel> htmlHelper,
                      Expression<Func<TModel, TProperty>> expression,
                      IEnumerable<SelectListItem> selectItems,                      
                      object htmlAttributes,
                      bool isDisabled = false)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression,
                                                                        htmlHelper.ViewData);
            var disabled = 
                (selectItems as SelectList).DisabledValues;

            var wanted = (disabled as IEnumerable<KeyValuePair<string, string>>).Select(x=>x.Key).ToArray();

            IEnumerable<SelectListItem> items =
                selectItems.Select(value => new SelectListItem
                {
                    Text = value.Text,
                    Value = value.Value,
                    Disabled = wanted.Contains(value.Value),
                    Selected = value.Equals(metadata.Model)
                });

            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            
            if (isDisabled && !attributes.ContainsKey("disabled"))
            {
                attributes.Add("disabled", "disabled");
            }
            return htmlHelper.DropDownListFor(expression, items, attributes);
        }
    }

    //public class CustomSelectItem : SelectListItem
    //{
    //    public bool Enabled { get; set; }
    //}

    //public static class CustomHtmlHelpers
    //{
    //    public static MvcHtmlString MyDropDownList(this HtmlHelper html, IEnumerable<CustomSelectItem> selectList)
    //    {
    //        var selectDoc = XDocument.Parse(html.DropDownList("", (IEnumerable<SelectListItem>)selectList).ToString());

    //        var options = from XElement el in selectDoc.Element("select").Descendants()
    //                      select el;

    //        foreach (var item in options)
    //        {
    //            var itemValue = item.Attribute("value");
    //            if (!selectList.Where(x => x.Value == itemValue.Value).Single().Enabled)
    //                item.SetAttributeValue("disabled", "disabled");
    //        }

    //        // rebuild the control, resetting the options with the ones you modified
    //        selectDoc.Root.ReplaceNodes(options.ToArray());
    //        return MvcHtmlString.Create(selectDoc.ToString());
    //    }
    //}
}