using CDMS.Language;
using CDMS.Model;
using CDMS.Model.ViewModel;
using CDMS.Service;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Linq;
using AutoMapper;

namespace CDMS.Web.Controllers
{
    public class CompanyController : BaseController
    {
        private readonly ICompanyService _CompanyService;
        private readonly IGlobalService _GlobalService;

        public CompanyController(ICompanyService companyService,
            IGlobalService globalService)
        {
            this._CompanyService = companyService;
            this._GlobalService = globalService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult _List(string start, string finish, string txt, string orderby, string sort, int page = 1)
        {
            InitViewBag(null);

            #region 設定頁碼 + 傳前端資料(ViewBag)
            int CurrentPage = page < 1 ? 1 : page;

            ViewBag.p = CurrentPage;
            ViewBag.txt = txt == null ? "" : txt;
            ViewBag.start = start == null ? "" : start;
            ViewBag.finish = finish == null ? "" : finish;

            ViewBag.orderby = orderby == null ? "" : orderby;
            ViewBag.sort = sort == null ? "" : sort;
            #endregion

            #region 組出SQL + 產生資料
            string sql = " 1 = 1 ";
            List<object> obj = new List<object> { txt, start, finish };

            if (!string.IsNullOrEmpty(txt))
                sql += " && (FullName.Contains(@0) || ShortName.Contains(@0) || Remarks.Contains(@0))";

            if (!string.IsNullOrEmpty(start)) sql += " && (CompanySupplier >= @1)";
            if (!string.IsNullOrEmpty(finish)) sql += " && (CompanySupplier <= @2)";
             
            var query = this._CompanyService.GetAll().Where(sql, obj.ToArray());

            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sort))
                query = query.OrderBy(orderby + " " + sort);

            #endregion

            #region 回傳
            return View("_List", query.ToPagedList(page, PageSize));
            #endregion
        }

        public ActionResult Create()
        {
            CompanyViewModel info = null;
#if DEBUG

            info = new CompanyViewModel()
            {
                FullName = "FullName",
                ShortName = "CompanyName",
                CompanyKindID = CompanyType.Both.Value,
                TaxID = "TaxID",
                ContactPerson = "ContactPerson".ToLocalized(),
                Telephone1 = "Telephone1",
                Telephone2 = "Telephone2",
                Fax = "Fax",
                Clerk = "Clerk",
                ClerkMobile = "ClerkMobile",
                Address = "地址",
                InvoiceAddress = "發票地址",
                ShippingAddress = "送貨地址",
                FactoryAddress = "工廠地址",
                NextMonth = 12,
                Remarks = "Remarks"
            };
#endif

            InitViewBag(info);
            return View(info);
        }

        private void InitViewBag(CompanyViewModel info)
        {
            ViewBag.CompanyKindList =
               new SelectList(CompanyType.GetAll(), "Value", "Text", info?.CompanyKindID);

            ViewBag.YseNoList =
              new SelectList(YesNo.GetAll(), "value", "Text", info?.Activate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CompanyViewModel m)
        {
            ResultModel result = new ResultModel();

            try
            {
                Company info = Mapper.Map<Company>(m);

                #region 驗證Model
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }
                #endregion

                if (this._CompanyService.IsDataExists(info))
                {
                    ModelState.AddModelError("CompanySupplier", $"{"CompanySupplier".ToLocalized()}:{info.CompanyID} 已經存在！");
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }

                #region 前端資料變後端用資料ViewModel時用

                #endregion

                #region Service資料庫
                this._CompanyService.Create(info);
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.Message = "MessageComplete".ToLocalized();
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

        public ActionResult Edit(string id)
        {
            try
            {
                #region 驗證
                if (string.IsNullOrEmpty(id))
                    return View("Error");
                #endregion

                #region 取資料
                var query = this._CompanyService.Get(id);
                CompanyViewModel info = Mapper.Map<CompanyViewModel>(query);
                #endregion

                #region ViewBag
                InitViewBag(info);
                #endregion

                #region 顯示特殊處理

                #endregion

                #region 下拉選單

                #endregion

                #region 回傳
                return View(info);
                #endregion
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
        public ActionResult Edit(CompanyViewModel m)
        {
            ResultModel result = new ResultModel();
            try
            {
                #region 驗證Model
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }
                #endregion

                #region 前端資料變後端用資料ViewModel時用
                var info = Mapper.Map<Company>(m);
                #endregion

                #region Service資料庫
                this._CompanyService.Update(info);
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.Message = "MessageComplete".ToLocalized();
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

        public ActionResult Delete(string id)
        {
            try
            {
                #region 驗證
                if (string.IsNullOrEmpty(id))
                    return View("Error");
                #endregion

                #region 取資料
                var query = this._CompanyService.Get(id);
                #endregion

                #region ViewBag

                #endregion

                #region 顯示特殊處理

                #endregion

                #region 下拉選單

                #endregion

                #region 回傳
                return View(query);
                #endregion
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
        [MultipleButton(Name = "action", Argument = "Delete")]
        public ActionResult DeleteConfirmed(Company model)
        {
            ResultModel result = new ResultModel() {};

            try
            {
                #region Service資料庫
                if (this._CompanyService.IsUsed(model))
                {
                    result.Message = "MessageChaneDelete2UpdateComplete".ToLocalized();

                    model.Activate = YesNo.No.Value; 
                    this._CompanyService.Update(model);                    
                }
                else
                {
                    this._CompanyService.Delete(model);
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

        // 要重新產生新的物件不然會有型別的物件時偵測到循環參考錯誤
        public ActionResult GetForAutocomplete(string term)
        {
            var query =
                this._CompanyService.GetForAutoComplete(term).ToList()
                .Select(x =>
                    new CodeName
                    {
                        Label = x.ShortName,
                        Value = x.CompanyID,
                        Source = x
                    })
                .ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetForSupplierAutocomplete(string term)
        {
            var query =
                this._CompanyService.GetForAutoComplete(term)
                .Where(x =>
                        x.CompanyKindID == CompanyType.Suppiler.Value
                     || x.CompanyKindID == CompanyType.Both.Value)
                .ToList()
                 .Select(x =>
                    new CodeName
                    {
                        Label = x.ShortName,
                        Value = x.CompanyID,
                        Source = x
                    })
                .ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetForCustomerAutocomplete(string term)
        {
            var query =
                this._CompanyService.GetForAutoComplete(term)
                .Where(x =>
                        x.CompanyKindID == CompanyType.Customer.Value
                     || x.CompanyKindID == CompanyType.Both.Value)
                .ToList()
                .Select(x => 
                    new CodeName {
                        Label = x.ShortName,
                        Value = x.CompanyID,
                        Source = x
                    })
                .ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCompany(string key)
        {
            var query =
                this._CompanyService.GetAll().Where(x => x.CompanyID == key).SingleOrDefault();
  
            return Json(query ?? new CompanyViewModel(), JsonRequestBehavior.AllowGet);
        }
    }
}