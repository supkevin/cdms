using System.Web.Mvc;
using CDMS.Web.Common;
using System.Reflection;
using System;
using CDMS.Web.ActionFilter;
using System.IO;

namespace CDMS.Web.Controllers
{
    public class BaseController : Controller
    {
        protected const int PageSize = 10;

        [HttpGet]
        [DeleteFileAttribute] //Action Filter, it will auto delete the file after download,                               
        public ActionResult Download(string file)
        {
            //get the temp folder and file path in server
            string fullPath = Path.Combine(Server.MapPath("~/Temp"), file);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/vnd.ms-excel", file);
        }

        public BaseController()
        {
           
        }

        override protected void OnActionExecuted(ActionExecutedContext filterContext)
        {            
            string actionName = filterContext.RouteData.Values["action"].ToString();
            string controllerName = filterContext.RouteData.Values["controller"].ToString();

            // 利用這兩個值顯示麵包屑路徑Breadcrumb
            ViewBag.ActionName = actionName;
            ViewBag.ControllerName = controllerName;
        }

        #region WriteError Log
        public void WriteError(string message, MethodBase function)
        {
            //需在Global Application_Start設定捉Config
            Log.WriteLog(LogLevel.Error, message, function);
            //_Log.Error(message); 
        }
        public void WriteError(Exception ex, MethodBase function)
        {
            //_Log.Error(ex.Message,ex);
            Log.WriteLog(LogLevel.Error, ex.ToString(), function);
        }

        public void WriteInfo(string message, MethodBase function)
        {
            //_Log.Invoice(message);
            Log.WriteLog(LogLevel.Info, message, function);
        }

        public void WriteInfo(string type, string message, MethodBase function)
        {
            //Log Log = new Log();
            //_Log.Invoice(string.Format("{0}=>{1}", type, message));            
            Log.WriteLog(LogLevel.Info, string.Format("{0}=>{1}", type, message), function);
        }
        #endregion
    }

}