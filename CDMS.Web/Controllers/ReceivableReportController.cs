using CDMS.Language;
using CDMS.Model;
using CDMS.Model.ViewModel;
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
using AutoMapper;

namespace CDMS.Web.Controllers
{
    public class ReceivableReportController : BaseController
    {
        private readonly IGlobalService _GlobalService;
        private readonly IReceivableService _ReceivableService;

        ReceivableViewModel _Info = new ReceivableViewModel()
        {
            
        };

        [System.Diagnostics.Conditional("DEBUG")]
        private void GenerateFakeData()
        {
         
        }

        public ReceivableReportController(
            IGlobalService globalService,
            IReceivableService ReceivableService
            )
        {
            this._GlobalService = globalService;
            this._ReceivableService = ReceivableService;
        }

        public ActionResult Index()
        {
            InitViewBag(null);
            return View();
        }

        [HttpPost]
        public ActionResult _List(
            string accountMonth,
            string start ="0", string finish="Z", string dealItem = "",
            string orderby = "CompanyID", string sort = "desc", int page = 1)
        {
            ViewBag.accountMonth = accountMonth;

            ViewBag.p = page < 1 ? 1 : page;
            ViewBag.orderby = orderby;
            ViewBag.sort = sort;

            string sql = " 1 = 1 ";
            List<object> obj = new List<object> { start, finish, accountMonth };

            if (!string.IsNullOrEmpty(start))
                sql += " && (CompanyID >= @0)";

            if (!string.IsNullOrEmpty(finish))
                sql += " && (CompanyID <= @1)";

            if (!string.IsNullOrEmpty(accountMonth))
                sql += " && (AccountMonth == @2)";

            var query = this._ReceivableService.GetListView()
                        .Where(sql, obj.ToArray())
                        .OrderBy($"{ orderby } { sort }");

            var info = query.ToPagedList(page, 1).SingleOrDefault();

            ViewBag.CurrentCompany = info == null ? "" : info.CompanyID;
            ViewBag.DealItem = info == null ? "" : dealItem;
            
            return View("_List", query.ToPagedList(page, 1));
        }

        private void InitViewBag(Receivable info)
        {
            ViewBag.AccountMonth = DateTime.Today.AddMonths(-1).ToString("yyMM");

            HashSet<MyTextValue> dealItem = new HashSet<MyTextValue>();

            dealItem.Add(new MyTextValue { Text = "進貨單", Value = DealItem.Purchase.Text, Disabled = false });
            dealItem.Add(new MyTextValue { Text = "進貨發票", Value = DealItem.PurchaseInvoice.Text, Disabled = false });
            dealItem.Add(new MyTextValue { Text = "銷貨單", Value = DealItem.Sales.Text, Disabled = false });
            dealItem.Add(new MyTextValue { Text = "銷貨發票", Value = DealItem.SalesInvoice.Text, Disabled = false });

            ViewBag.DealItemList = new SelectList(dealItem, "Value", "Text", null);
        }

        [HttpPost]
        public ActionResult _Receivable(
            string currentCompany ="" ,string dealItem = "",
            string orderby = "DealDate", string sort = "desc", int page = 1)
        {      
            string sql = " 1 = 1 ";
            List<object> obj = new List<object> { currentCompany };
                        
            sql += " && (CompanyID == @0)";

            var query = this._ReceivableService.GetAll()
                        .Where(sql, obj.ToArray())
                        .OrderBy($"{ orderby } { sort }");
            
            // IQuerable不能用string[].Contains用這種方式
            query = query.AsEnumerable()
                    .Where(x =>
                      string.IsNullOrEmpty(dealItem) || dealItem.Split(',').Contains(x.DealItem)
                    ).AsQueryable();

            var infos = string.Join(",", query.Select(x => x.VoucherID).ToArray());
            ViewBag.Keys = infos == null ? "" : infos;
            
            return View("_Receivable", query.ToList());
        }

        [HttpPost]
        public ActionResult _Item(string keys="",
            string orderby = "VoucherID", string sort = "desc", int page = 1)
        {
           string sql = " 1 = 1 ";
            List<object> obj = new List<object> {};

            var query = this._ReceivableService.GetDetailListView()
                        .Where(sql, obj.ToArray())
                        .OrderBy($"{ orderby } { sort }");

            // IQuerable不能用string[].Contains用這種方式
            query = query.AsEnumerable()
                    .Where(x =>                                              
                       keys.Split(',').Contains(x.VoucherID)
                    ).AsQueryable();

            return View("_Item", query.ToList());
        }
    }
}