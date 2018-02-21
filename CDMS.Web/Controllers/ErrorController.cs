using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDMS.Web.Controllers
{
    public class ErrorController : BaseController
    {
        // GET: Error
        public ActionResult Index(string error)
        {
            ViewBag.Error = error;

            return View();
        }
    }
}