using CDMS.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace CDMS.Web.Controllers
{
    public class EmployeeController : Controller
    {
        [HttpPost]
        public ActionResult GetForAutocomplete(string pid)
        {
            EmployeeModel Script = new EmployeeModel();
            List<EmployeeModel> result = Script.GetEmpDataByStaffID(pid, "0", "0");
            return Json(result);
        }
    }
}