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

namespace CDMS.Web.Controllers
{
    public class _TestController : BaseController
    {
        private readonly IAlternativeService _AlternativeService;

        public _TestController(IAlternativeService alternativeService)
        {
            this._AlternativeService = alternativeService;
        }

        //public async Task<ActionResult> Edit(int id)
        //{
        //    //IEnumerable<Address> addressEntities = await GetAddressesAsync(id);

        //    var viewModel = new PersonEditViewModel
        //    {

        //        Addresses = addressEntities.Select(a => new AddressEditorViewModel
        //        {
        //            Id = a.Id,
        //            Street = a.Street,
        //            City = a.City
        //        }),
        //    };

        //    return View(viewModel);
        //}

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

        public ActionResult GetAddressEditor()
        {
            return PartialView("_AddressEditor", new AddressEditorViewModel());
        }

        [HttpPost]
        public ActionResult Edit(int? id, PersonEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // TODO: Save changes of viewModel.Addresses to database
                return RedirectToAction("Details", new { id = id });
            }
            else
                return View(viewModel);
        }
    }
}