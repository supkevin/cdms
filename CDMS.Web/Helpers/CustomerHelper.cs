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

namespace CDMS.Web.Helpers
{ 
    public static class CustomerHelper
    {
        /// <summary>
        /// 顯示圖片根據路徑
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="src"></param>
        /// <param name="height"></param>
        /// <param name="altText"></param>
        /// <param name="pClass"></param>
        /// <returns></returns>
        public static MvcHtmlString DisplayImageByPath(
                 this HtmlHelper helper, string src, int height, string altText = "", string pClass = "")
        {
            var builder = new TagBuilder("img");

            builder.MergeAttribute("src", src);
            builder.MergeAttribute("alt", altText);

            if (height != -1)
            {
                builder.MergeAttribute("height", height.ToString());
            }

            builder.MergeAttribute("class", pClass);

            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }

        /// <summary>
        /// 多筆維護
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <param name="htmlFieldName"></param>
        /// <returns></returns>
        public static MvcHtmlString EditorForMany<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, IEnumerable<TValue>>> expression,
            string htmlFieldName = null) where TModel : class
        {

            var items = expression.Compile()(html.ViewData.Model);

            var sb = new StringBuilder();
            var hasPrefix = false;

            if (String.IsNullOrEmpty(htmlFieldName))
            {
                var prefix = html.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix;
                hasPrefix = !String.IsNullOrEmpty(prefix);

                htmlFieldName = (prefix.Length > 0 ? 
                    (prefix + ".") : 
                    String.Empty) + ExpressionHelper.GetExpressionText(expression);
            }

            if (items != null)
            {
                foreach (var item in items)
                {
                    var dummy = new { Item = item };
                    var guid = Guid.NewGuid().ToString();

                    var memberExp = 
                        Expression.MakeMemberAccess(Expression.Constant(dummy), dummy.GetType().GetProperty("Item"));

                    var singleItemExp = 
                        Expression.Lambda<Func<TModel, TValue>>(memberExp, expression.Parameters);

                    sb.Append(String.Format(@"<input type=""hidden"" name=""{0}.Index"" value=""{1}"" />", htmlFieldName, guid));
                    sb.Append(html.EditorFor(singleItemExp, null,
                        String.Format("{0}[{1}]",
                        hasPrefix ? ExpressionHelper.GetExpressionText(expression) : htmlFieldName,
                        guid)));
                }
            }
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// 前端讀二進位圖片資料
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="pPhoto">二進位圖</param>
        /// <param name="pAlt">Alt</param>
        /// <param name="pMaxWdith">最大寬度</param>
        /// <returns></returns>
        public static MvcHtmlString DisPlayImage(this HtmlHelper helper, byte[] pPhoto, int pMaxWdith = 0, string pAlt = "", string pClass = "")
        {
            string imageSrc = null;
            if (pPhoto == null)
                return null;

            #region 二進位資料轉圖片
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    string imageBase64 = string.Empty;
            //    Image xImage = (Bitmap)((new ImageConverter()).ConvertFrom(pPhoto));
            //    if (ImageFormat.Bmp.Equals(xImage.RawFormat))
            //    {
            //        // strip out 78 byte OLE header (don't need to do this for normal images)
            //        ms.Write(pPhoto, 78, pPhoto.Length - 78);
            //    }
            //    else
            //    {
            //        ms.Write(pPhoto, 0, pPhoto.Length);
            //    }
            //    imageBase64 = Convert.ToBase64String(ms.ToArray());
            //    imageSrc = string.Format("data:image/jpeg;base64,{0}", imageBase64);
            //}

            imageSrc = ImageClass.GetImage64FromByte(pPhoto);
            #endregion

            #region 產生Html
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", imageSrc);

            string mStyle = pMaxWdith == 0 ? "100%" : string.Format("{0}px", pMaxWdith);

            builder.MergeAttribute("style", string.Format("max-width: {0} ; height: auto;", mStyle));

            if (!string.IsNullOrWhiteSpace(pAlt))
                builder.MergeAttribute("alt", pAlt);

            if (!string.IsNullOrWhiteSpace(pClass))
                builder.MergeAttribute("class", pClass);

            #endregion

            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }


        public static MvcHtmlString DisplayWithBreaksFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var model = html.Encode(metadata.Model).Replace("\r\n", "<br />\r\n");

            if (String.IsNullOrEmpty(model))
                return MvcHtmlString.Empty;

            return MvcHtmlString.Create(model);
        }

        public static MvcHtmlString DisplayStatus(this HtmlHelper html, string CX_From_Date, string CX_To_Date)
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string mNow = DateTime.Now.ToString("yyyy-MM-dd");

            if (CX_From_Date.CompareTo(mNow) <= 0 && CX_To_Date.CompareTo(mNow) >= 0)
            {//出差中
                sb.AppendFormat("<span style=\"color:blue;\"><b>");
                sb.AppendFormat("TextIsOut".ToLocalized());
                sb.AppendFormat("</b></span>");
            }
            else if (CX_From_Date.CompareTo(mNow) > 0)
            {//未出發
                sb.AppendFormat("<span style=\"color:red;\"><b>");
                sb.AppendFormat("TextIsNotGo".ToLocalized());
                sb.AppendFormat("</b></span>");

            }
            else if (CX_To_Date.CompareTo(mNow) < 0)
            {//已回國
                sb.AppendFormat("<span style=\"color:#227700;\"><b>");
                sb.AppendFormat("TextIsIn".ToLocalized());
                sb.AppendFormat("</b></span>");
            }

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}