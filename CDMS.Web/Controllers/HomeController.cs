using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDMS.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            // 顯示最新消息
            return RedirectToAction("Display", "News", null);
        }

    }
}