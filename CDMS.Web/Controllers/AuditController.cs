using CDMS.Language;
using CDMS.Model.ViewModel;
using CDMS.Model;
using CDMS.Service;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Linq;

namespace CDMS.Web.Controllers
{
    public class AuditController : BaseController
    {        
        private readonly IQuotationComplexService _QuotationComplexService;        
        private readonly IGlobalService _GlobalService;

        public AuditController(               
               IGlobalService globalService,
               IQuotationComplexService QuotationComplexService
               )
        {                     
            this._QuotationComplexService = QuotationComplexService;
            this._GlobalService = globalService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult _List(
            string start, string finish, 
            string company,
            string product, string productName,
            string orderby = "Quotation.QuotationID", string sort= "desc", int page = 1)
        {

            InitViewBag(null);
            InitChildViewBag(null);

            #region 設定頁碼 + 傳前端資料(ViewBag)
                        
            ViewBag.start = start;
            ViewBag.finish = finish;
            ViewBag.company = company;
            ViewBag.product = product;
            ViewBag.productName = productName;

            ViewBag.p = page < 1 ? 1 : page;
            ViewBag.orderby = orderby;
            ViewBag.sort = sort;
            #endregion

            #region 組出SQL + 產生資料
            string sql = " 1 = 1 ";
            List<object> obj =
               new List<object> {
                    string.IsNullOrEmpty(start) ? DateTime.MinValue : DateTime.Parse(start),
                    string.IsNullOrEmpty(finish) ? DateTime.MaxValue : DateTime.Parse(finish),
                    company,
                    product ,
                    productName
               };

            if (!string.IsNullOrEmpty(start))
                sql += " && (Quotation.QuotationDate >=(@0))";

            if (!string.IsNullOrEmpty(finish))
                sql += " && (Quotation.QuotationDate <=(@1))";

            if (!string.IsNullOrEmpty(company))
                sql += " && (Quotation.CustomerID.Contains(@2))";

            if (!string.IsNullOrEmpty(product))
                sql += " && (ChildList.Any(ProductID.Contains(@3)))";

            if (!string.IsNullOrEmpty(productName))
                sql += " && (ChildList.Any(ProductName.Contains(@4)))";

            var query = 
                this._QuotationComplexService.GetAll()
                .Where(sql, obj.ToArray())
                .OrderBy($"{orderby} {sort}");

            #endregion

            #region 回傳
            return View("_List", query.ToPagedList(page, PageSize));
            #endregion
        }

        [HttpGet]
        public ActionResult _Item(string id)
        {
            //child action don't share the same ViewBag with its “parents” action
            var info = new QuotationDetailViewModel()
            {
                SeqNo = 0,
                IsDirty = true
            };

            InitChildViewBag(null);

            return PartialView("_Item", info);
        }

        private void InitViewBag(QuotationComplex info)
        {           
            // 倉庫
            ViewBag.WareHouseList =
                new SelectList(this._GlobalService.GetWarehouseList(), "Key", "Value", info?.Quotation.WarehouseID);

            // 公司
            ViewBag.CompanyList =
              new SelectList(this._GlobalService.GetCustomerList(), "Value", "Display", info?.Quotation?.CustomerID);

            // 運送方式
            ViewBag.ShippingModeList =
                new SelectList(this._GlobalService.GetShippingModeList(), "Key", "Value", null);

            // 審核結果
            ViewBag.ResultTypeList =
                new SelectList(ResultType.GetAll() , "value", "Text", info?.Quotation.Result);

            // 報價人
            //ViewBag.UserList =
            //   new SelectList(this._GlobalService.GetUserList(), "Value", "Display", info?.Quotation.QuotePerson);
            ViewBag.UserList =
              new SelectList(this._GlobalService.GetUserList(), "Value", "Display", info?.Quotation.QuotePerson);


            ViewBag.YseNoList =
             new SelectList(YesNo.GetAll(), "value", "Text", info?.Quotation.Activate);
        }

        private void InitChildViewBag(QuotationDetailViewModel info)
        {
            ViewBag.PriceKindList =
                new SelectList(this._GlobalService.GetPriceKindList(), "Key", "Value", info?.PriceKindID);

            ViewBag.ConditionKindList =
               new SelectList(this._GlobalService.GetConditionKindList(), "Key", "Value", info?.ConditionID);
        }

        public ActionResult Edit(string id)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                    return View("Error");
        
                var query = this._QuotationComplexService.Get(id);
                               
                InitViewBag(query);
                InitChildViewBag(null);
               
                return View(query);   
            }
            catch (Exception ex)
            {
                #region 有錯誤時錯誤訊息
                TempData["Message"] = ex.Message;
                return View("Error");
                #endregion
            }
        }

        public ActionResult Display(string id)
        {
            try
            {
                #region 驗證
                if (string.IsNullOrEmpty(id))
                    return View("Error");
                #endregion

                #region 取資料
                var query = this._QuotationComplexService.Get(id);
                #endregion

                #region ViewBag
                InitViewBag(query);
                InitChildViewBag(null);
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
        public ActionResult Edit(QuotationComplex info)
        {
            ResultModel result = new ResultModel();
            try
            {
                #region 驗證Model
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }
                // 至少需有一筆明細資料
                if (info.ChildList == null || info.ChildList.Count == 0)
                {
                    ModelState.AddModelError("ChildList", "至少需有一筆明細資料。");
                }
                else
                {
                    var query =
                       info.ChildList
                       .GroupBy(x => x.ProductID)
                       .ToDictionary(x => x.Key, x => x.Count())
                       .Where(x => x.Value > 1)
                       .ToList();

                    CDMS.Web.Common.DuplicateValidator validator =
                        new CDMS.Web.Common.DuplicateValidator(query);

                    if (validator.Message.Count > 0)
                    {
                        foreach (var s in validator.Message)
                        {
                            ModelState.AddModelError("ChildList", s);
                        }
                    }
                }

                if (!ModelState.IsValid)
                {
                    string message = ModelStateErrorClass.FormatToString(ModelState);

                    result.Status = false;
                    result.Message = message;

                    return Json(result);
                }
                #endregion

                #region 前端資料變後端用資料ViewModel時用

                #endregion

                #region Service資料庫
                this._QuotationComplexService.Audit(info);
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.CloseWindow = false;
                result.Url = Url.Action("Edit", new { id = info.Quotation.QuotationID });
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
    }
}