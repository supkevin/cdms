using System.Web.Mvc;

namespace CDMS.Language
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// 產生一個字串
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <remarks>
        /// Example:@Html.OutputText("Language".Toi18n())
        /// </remarks>
        public static MvcHtmlString OutputText(this HtmlHelper helper, string text)
        {
            return new MvcHtmlString(text);
        }
    }
}
