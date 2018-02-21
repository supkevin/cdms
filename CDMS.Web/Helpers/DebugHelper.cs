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

namespace CDMS.Web
{
    public static class DebugHelper
    {
        // 判斷目前是否是Debug模式
        public static Boolean DEBUG(this HtmlHelper helper)
        {

#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}