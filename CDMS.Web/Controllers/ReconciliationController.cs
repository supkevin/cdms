using CDMS.Model;
using CDMS.Model.ViewModel;
using CDMS.Service;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace CDMS.Web.Controllers
{    
    public class ReconciliationController : BaseController
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

        public ReconciliationController(
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
            string start = "0", string finish = "Z", string dealItem = "",
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

            return View("_List", query.ToPagedList(page, PageSize));
        }

        private void InitViewBag(Receivable info)
        {
            ViewBag.AccountMonth = DateTime.Today.AddMonths(-1).ToString("yyMM");
        }

        public ActionResult Create(
            string company = "", string accountMonth = "",
            string orderby = "DealDate", string sort = "desc", int page = 1)
        {
            var query = GetQuery(company, accountMonth, orderby, sort, page);

            return View("Create", query.ToList());
        }

        private IQueryable<ReconciliationViewModel> GetQuery(
            string company = "", string accountMonth = "",
            string orderby = "DealDate", string sort = "desc", int page = 1)
        {

            var summary = this._ReceivableService.GetListView()
               .Where(x => x.CompanyID == company && x.AccountMonth == accountMonth)
               .SingleOrDefault();

            ViewBag.Summary =
                     summary == null ? new ReceivableSummaryViewModel() : summary;

            string sql = " 1 = 1 ";
            List<object> obj = new List<object> { company, accountMonth };

            sql += " && (CompanyID == @0)";
            sql += " && (AccountMonth == @1)";

            var query = this._ReceivableService.GetReconciliationListView()
                        .Where(sql, obj.ToArray())
                        .OrderBy($"{ orderby } { sort }");
                       
            return query;
        }

        public ActionResult Edit(
            string company = "", string accountMonth = "",
            string orderby = "DealDate", string sort = "desc", int page = 1)
        {
            var query = GetQuery(company, accountMonth, orderby, sort, page);

            var infos = string.Join(",", query.Select(x => x.VoucherID).ToArray());
            ViewBag.Keys = infos == null ? "" : infos;

            return View("Edit", query.ToList());
        }

        [HttpPost]
        public ActionResult _Item(string keys = "",
           string orderby = "VoucherID", string sort = "desc", int page = 1)
        {
            string sql = " 1 = 1 ";
            List<object> obj = new List<object> { };

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