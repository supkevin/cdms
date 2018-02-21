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
using System.Threading.Tasks;
using System.Reflection;

namespace CDMS.Web.Controllers
{
    public class _DynamicController : BaseController
    {
        private readonly IAlternativeService _AlternativeService;

        public _DynamicController(IAlternativeService alternativeService)
        {
            this._AlternativeService = alternativeService;

            this.WriteInfo("OKOK", MethodBase.GetCurrentMethod());
        }

        public ActionResult Index()
        {

            var temp = new List<AddressEditorViewModel>();
            temp.Add(new AddressEditorViewModel { Id = 1, Street = "Street", City = "City" });

            var viewModel = new PersonEditViewModel
            {
                Addresses = temp
            };

            return View(viewModel);
        }

        public ActionResult Edit(PersonEditViewModel info)
        {
            #region 驗證Model
            if (!ModelState.IsValid)
            {
                throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
            }
            #endregion

            return View("Index", info);
        }

        [HttpGet]
        public ActionResult AddItem()
        {
            return PartialView("_Item", new AddressEditorViewModel());
        }
    }
}