using CDMS.Language;
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
    public class ReceivableController : BaseController
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

        public ReceivableController(
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

        private void InitViewBag(Receivable info)
        {
            ViewBag.AccountMonth = DateTime.Today.AddMonths(-1).ToString("yyMM");

            //ViewBag.BankAccountList =
            //   new SelectList(this._GlobalService.GetBankAccountList(), "Value", "Text", info?.BankAccountID);

            //ViewBag.YseNoList =
            //new SelectList(YesNo.GetAll(), "value", "Text", info?.Activate);
        }

        private void CheckBusinessRules(Receivable info)
        {
            //if (this._ReceivableService.IsCheckNumExists(info))
            //{
            //    ModelState.AddModelError("CheckNum", $"支票號碼：{ info.CheckNum }，已經存在。");
            //}
        }

        public ActionResult Create(Receivable info)
        {
            ResultModel result = new ResultModel();
            try
            {

                this._ReceivableService.Initialize(info.AccountMonth);

                result.Status = true;
                result.CloseWindow = false;
                result.Message = "MessageComplete".ToLocalized();
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message.ToString();
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult _List(
            string accountMonth,
            string orderby = "CompanyID", string sort = "desc", int page = 1)
        {
            ViewBag.accountMonth = accountMonth;

            ViewBag.p = page < 1 ? 1 : page;
            ViewBag.orderby = orderby;
            ViewBag.sort = sort;

            string sql = " 1 = 1 ";
            List<object> obj = new List<object> { accountMonth };
            sql += " && (AccountMonth == @0)";

            var query = this._ReceivableService.GetListView()
                        .Where(sql, obj.ToArray())
                        .OrderBy($"{ orderby } { sort }");

            return View("_List", query.ToPagedList(page, PageSize));
        }
    }
}