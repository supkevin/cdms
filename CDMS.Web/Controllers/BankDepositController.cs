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
    public class BankDepositController : BaseController
    {
        private readonly IGlobalService _GlobalService;
        private readonly IBankDepositService _BankDepositService;

        BankDepositViewModel _Info = new BankDepositViewModel()
        {
            
        };

        [System.Diagnostics.Conditional("DEBUG")]
        private void GenerateFakeData()
        {        
        }

        public BankDepositController(
            IGlobalService globalService,
            IBankDepositService BankDepositService
            )
        {
            this._GlobalService = globalService;
            this._BankDepositService = BankDepositService;
        }

        public ActionResult Index()
        {
            InitViewBag(null);
            return View();
        }

        [HttpPost]
        public ActionResult _List(
            string startDate, string finishDate,
            string start, string finish,
            string bankAccount,
            string checkStatus,
            string summary,
            string orderby = "SourceID", string sort = "desc", int page = 1)
        {
            InitViewBag(null);

            ViewBag.startDate = startDate;
            ViewBag.finishDate = finishDate;

            ViewBag.start = start;
            ViewBag.finish = finish;
            ViewBag.bankAccount = bankAccount;
            ViewBag.checkStatus = checkStatus;
            ViewBag.summary = summary;

            ViewBag.p = page < 1 ? 1 : page;
            ViewBag.orderby = orderby;
            ViewBag.sort = sort;
            
            string sql = " 1 = 1 ";
            List<object> obj = 
                new List<object> {
                    string.IsNullOrEmpty(startDate) ? DateTime.MinValue : Convert.ToDateTime(startDate),
                    string.IsNullOrEmpty(finishDate) ? DateTime.MaxValue : Convert.ToDateTime(finishDate),                    
                    start,
                    finish,
                    string.IsNullOrEmpty(bankAccount) ? 0 : int.Parse(bankAccount) 
                };

            if (!string.IsNullOrEmpty(startDate))
            {
                sql += " && ( DealDate >= @0 )";
            }

            if (!string.IsNullOrEmpty(finishDate))
            {
                sql += " && ( DealDate <= @1 )";
            }

            if (!string.IsNullOrEmpty(start))
            {
                sql += " && ( CheckNum >= @2 )"; 
            }

            if (!string.IsNullOrEmpty(finish))
            {
                sql += " && ( CheckNum <= @3 )";
            }

            if (!string.IsNullOrEmpty(bankAccount))
            {
                sql += " && ( BankAccountID = @4 )";
            }

            var query = this._BankDepositService.GetListView()
                        .Where(sql, obj.ToArray())
                        .OrderBy($"{ orderby } { sort }");

            // IQuerable不能用string[].Contains用這種方式
            query = query.AsEnumerable()
                    .Where(x =>
                        (
                        (string.IsNullOrEmpty(checkStatus) || checkStatus.Split(',').Contains(x.CheckStatus))
                        && (string.IsNullOrEmpty(summary) || summary.Split(',').Contains(x.Summary))
                        )
                    ).AsQueryable();

            return View("_List", query.ToPagedList(page, PageSize));            
        }

        public ActionResult Create()
        {
            var info = _Info;
            GenerateFakeData();

            InitViewBag(info);
            return View(info);
        }

        private void InitViewBag(BankDeposit info)
        {
            ViewBag.BankAccountList =
                  new SelectList(this._GlobalService.GetBankAccountList(), "Value", "Text", null);

            HashSet < MyTextValue> sort = new HashSet<MyTextValue>();

            sort.Add(new MyTextValue { Text= "票據號碼", Value = "CheckNum" , Disabled = false });
            sort.Add(new MyTextValue { Text = "收(開)票日期", Value = "DealDate", Disabled = false });
            sort.Add(new MyTextValue { Text = "到期日期", Value = "ExpiryDate", Disabled = false });
            
            ViewBag.SortList = new SelectList(sort, "Value", "Text", null);


            ViewBag.DepositSummaryList =
                 new SelectList(this._GlobalService.GetDepositSummaryList(), "Text", "Text", null);

            ViewBag.CheckStatusList =
                 new SelectList(this._GlobalService.GetCheckStatusList(), "Value", "Text", null);

            ViewBag.DealItemList =
             new SelectList(this._GlobalService.GetDealItemList(), "Value", "Text", null);

            ViewBag.YseNoList =
            new SelectList(YesNo.GetAll(), "value", "Text", info?.Activate);
        }
    
        public ActionResult Edit(SimpleBankDepositViewModel model)
        {
            ResultModel result = new ResultModel() { CloseWindow = false };

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }

                var info = this._BankDepositService.Get(model.SeqNo);

                info =  Mapper.Map(model, info);                                
                this._BankDepositService.Update(info);

                result.Status = true;
                result.CloseWindow = false;                
                result.Message = "MessageComplete".ToLocalized();
            }
            catch (Exception ex)
            {
                #region 有錯誤時錯誤訊息
                result.Status = false;
                result.Message = ex.Message.ToString();
                #endregion
            }
            return Json(result);
        }
    }
}