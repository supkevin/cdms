using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;

namespace CDMS.Web.Helpers
{
    //必填欄位自動加註解
    //https://michael-mckenna.com/automatically-marking-required-labels-in-asp-net-mvc/
    public static class ChqHtmlHelperExtensions
    {
        //public static MvcHtmlString MyLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        //{
        //    var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
        //    string resolvedLabelText = metadata.DisplayName ?? metadata.PropertyName;
        //    if (metadata.IsRequired)
        //    {
        //        resolvedLabelText = string.Format("<font color='red'>*</font>{0}", resolvedLabelText);
        //    }
        //    return LabelExtensions.LabelFor<TModel, TValue>(html, expression, resolvedLabelText, htmlAttributes);
        //}

        public static MvcHtmlString MarkLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return MarkLabelFor(html, expression, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString MarkLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();

            if (String.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

            // 如果是必填欄位加註記號
            //tag.SetInnerText(labelText);
            string text = labelText ;            
            if (metadata.IsRequired)
            {
                text = (string.Format("<b style='color: Red;'>＊</b>{0}", text));
            }
                        
            tag.InnerHtml = text;

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }
    }

}