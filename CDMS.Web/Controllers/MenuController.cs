using CDMS.Language;
using CDMS.Model;
using CDMS.Service;
using CDMS.Web;
using CDMS.Web.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using CDMS.Web.ActionFilter;
using System.IO;
using System.Text;
using CDMS.Web.Utility;

namespace CDMS.Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuComplexService _MenuComplexService;
        private readonly IUserService _UserService;
        private readonly IGlobalService _GlobalService;

        public MenuController(IMenuComplexService menuComplexService,
               IUserService userService,
               IGlobalService globaluserService
               )
        {
            this._MenuComplexService = menuComplexService;
            this._UserService = userService;
            this._GlobalService = globaluserService;
        }

        public ActionResult _List() {

            var user = this._UserService.Get(IdentityService.GetUserData().UserID);

            if (user == null)
            {
                RedirectToAction("login", "Login", null);
            }

            string permissionID = user.PermissionID;
            var menu = this._MenuComplexService.Get(permissionID);

            return View("_List", menu);            
        }

        public ActionResult _Breadcrumb(string key)
        {
            var menu = 
                this._GlobalService.GetMenuList()
                .Where(x => x.Key == key)
                .Select(x=>x.Value)
                .SingleOrDefault();
            
            return View("_Breadcrumb", menu);
        }
    }
}
