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
    public class PaymentController : BaseController
    {
        private readonly IGlobalService _GlobalService;
        private readonly IPaymentService _PaymentService;

        PaymentViewModel _Info = new PaymentViewModel()
        {
            
        };

        [System.Diagnostics.Conditional("DEBUG")]
        private void GenerateFakeData()
        {
            _Info.AccountMonth = "1801";
            _Info.SupplierID = "123456";
            _Info.CheckNum = "012345678901234";
            _Info.CheckAmount = 10000;
            _Info.CashAmount = 20000;
            _Info.ReturnAmount = 30000;
            _Info.DiscountAmount = 40000;
            _Info.DueDate = DateTime.Today.AddDays(30);
            _Info.PayDate = DateTime.Today;
            _Info.Remarks = $"{DateTime.Today.ToString(GlobalSettings.DATE_FORMAT)} 備註";
        }

        public PaymentController(
            IGlobalService globalService,
            IPaymentService PaymentService
            )
        {
            this._GlobalService = globalService;
            this._PaymentService = PaymentService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult _List(
            string customer, string accountMonth,
            string orderby = "PaymentID", string sort = "desc", int page = 1)
        {
            ViewBag.customer = customer;
            ViewBag.accountMonth = accountMonth;

            ViewBag.p = page < 1 ? 1 : page;
            ViewBag.orderby = orderby;
            ViewBag.sort = sort;
            
            string Sql = " 1 = 1 ";
            List<object> obj = new List<object> { customer, accountMonth };

            //if (!string.IsNullOrEmpty(customer))
            //{
            //    Sql += " && (BankID.Contains(@0) || BankName.Contains(@0) ";
            //    Sql += " || AccountID.Contains(@0)|| AccountName.Contains(@0) || Remarks.Contains(@0))";
            //}

            var query = this._PaymentService.GetListView()
                        .Where(Sql, obj.ToArray())
                        .OrderBy($"{ orderby } { sort }");
                     
            return View("_List", query.ToPagedList(page, PageSize));            
        }

        public ActionResult Create()
        {
            var info = _Info;
            GenerateFakeData();

            InitViewBag(info);
            return View(info);
        }

        private void InitViewBag(Payment info)
        {
            ViewBag.BankAccountList =
               new SelectList(this._GlobalService.GetBankAccountList(), "Value", "Text", info?.BankAccountID);

            ViewBag.YseNoList =
            new SelectList(YesNo.GetAll(), "value", "Text", info?.Activate);
        }

        private void CheckBusinessRules(Payment info) {
            if (this._PaymentService.IsCheckNumExists(info))
            {
                ModelState.AddModelError("CheckNum", $"支票號碼：{ info.CheckNum }，已經存在。");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PaymentViewModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }

                var info = Mapper.Map<Payment>(model);

                CheckBusinessRules(info);

                if (!ModelState.IsValid)
                {
                    string message = ModelStateErrorClass.FormatToString(ModelState);
                    result.Status = false;
                    result.Message = message;
                    return Json(result);
                }

                this._PaymentService.Create(info);

                result.Status = true;
                result.CloseWindow = false;
                result.Url = Url.Action("Edit", new { id = info.PaymentID });
                result.Message = "MessageComplete".ToLocalized();
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message.ToString();
            }
            return Json(result);
        }

        public ActionResult Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return View("Error");

                var query = this._PaymentService.Get(id);                

                InitViewBag(query);

                return View(Mapper.Map<PaymentViewModel>(query));   
            }
            catch (Exception ex)
            {
                #region 有錯誤時錯誤訊息
                TempData["Message"] = ex.Message;
                return View("Error");
                #endregion
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "Edit")]
        public ActionResult Edit(PaymentViewModel model)
        {
            ResultModel result = new ResultModel() { CloseWindow = false };
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }

                var info = Mapper.Map<Payment>(model);

                CheckBusinessRules(info);

                if (!ModelState.IsValid)
                {
                    string message = ModelStateErrorClass.FormatToString(ModelState);
                    result.Status = false;
                    result.Message = message;
                    return Json(result);
                }

                this._PaymentService.Update(info);

                result.Status = true;
                result.CloseWindow = false;
                result.Url = Url.Action("Edit", new { id = info.PaymentID });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "Delete")]
        public ActionResult DeleteConfirmed(Payment model)
        {
            ResultModel result = new ResultModel();
            try
            {
                #region Service資料庫
                if (this._PaymentService.IsUsed(model))
                {
                    result.Message = "MessageChaneDelete2UpdateComplete".ToLocalized();

                    model.Activate = YesNo.No.Value;
                    this._PaymentService.Update(model);
                }
                else
                {
                    this._PaymentService.Delete(model);
                }
                #endregion
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