using CDMS.Language;
using CDMS.Model;
using CDMS.Service;
using CDMS.Web;
using CDMS.Web.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace CDMS.Web.Controllers
{
    //public ActionResult Breadcrumb()
    //{
    //    ViewBag.ServerVar = "Html.RenderAction is appropriate for dynamic data";
    //    return PartialView();
    //}    
    public class BreadcrumbController : BaseController
    {
        //private readonly IWorkFlowService _workflowService;

        //// GET: Employee
        //public BreadcrumbController(IWorkFlowService workflowService)
        //{
        //    this._workflowService = workflowService;
        //}

        public BreadcrumbController()
        {

        }

        [ChildActionOnly]
        public ActionResult _Breadcrumb(string formType, string aid, string btntype)
        {
            return PartialView(null);
        }
    }
}