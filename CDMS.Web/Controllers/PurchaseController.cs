using CDMS.Language;
using CDMS.Model;
using CDMS.Model.ViewModel;
using CDMS.Service;
using CDMS.Web;
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
    public class PurchaseController : BaseController
    {
        private readonly IPurchaseComplexService _PurchaseComplexService;
        private readonly IGlobalService _GlobalService;

        public PurchaseController(
               IGlobalService globalService,
               IPurchaseComplexService PurchaseComplexService
               )
        {
            this._PurchaseComplexService = PurchaseComplexService;
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
            string orderby, string sort, int page = 1)
        {
            InitViewBag(null);
            InitChildViewBag(null);

            #region 設定頁碼 + 傳前端資料(ViewBag)
            int CurrentPage = page < 1 ? 1 : page;

            ViewBag.start = start == null ? "" : start;
            ViewBag.finish = finish == null ? "" : finish;
            ViewBag.company = company == null ? "" : company;
            ViewBag.product = product == null ? "" : product;
            ViewBag.productName = productName == null ? "" : productName;

            ViewBag.p = CurrentPage;
            ViewBag.orderby = sort == null ? "" : orderby;
            ViewBag.sort = sort == null ? "" : sort;
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
                sql += " && (Purchase.PurchaseDate >=(@0))";

            if (!string.IsNullOrEmpty(finish))
                sql += " && (Purchase.PurchaseDate <=(@1))";

            if (!string.IsNullOrEmpty(company))
                sql += " && (Purchase.SupplierID.Contains(@2))";

            if (!string.IsNullOrEmpty(product))
                sql += " && (ChildList.Any(ProductID.Contains(@3)))";

            if (!string.IsNullOrEmpty(productName))
                sql += " && (ChildList.Any(ProductName.Contains(@4)))";


            var query =
                this._PurchaseComplexService.GetAll().Where(sql, obj.ToArray());

            orderby = "Purchase.PurchaseID";
            sort = "desc";

            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sort))
                query = query.OrderBy(orderby + " " + sort);

            #endregion

            #region 回傳
            return View("_List", query.ToPagedList(page, 1));
            #endregion
        }

        [HttpGet]
        public ActionResult _Item(string id)
        {
            //child action don't share the same ViewBag with its “parents” action
            var info = new PurchaseDetailViewModel()
            {
                SeqNo = 0,
                Discount = 100,
                IsDirty = true
            };

            InitChildViewBag(null);

            return PartialView("_Item", info);
        }

        public ActionResult Create()
        {
            var info = new PurchaseComplex()
            {

#if DEBUG
                Purchase = new PurchaseViewModel()
                {
                    PurchaseID = "0".PadLeft(10, '0'),
                    PurchaseDate = DateTime.Today,
                    InvoiceAmount = 10000,
                    InvoiceID = "0".PadLeft(10, '0'),
                    //SupplierID = "000000",
                    //ContactPerson = "聯絡人",
                    //ContactPhone = "聯絡電話",
                    CurrencyID = "1",
                    ExchangeRate = 1,
                    AccountMonth = "1702",
                    WarehouseID = "1",
                    Remarks = "Remarks",
                    PostingTime = DateTime.Today.AddMonths(1)

                },
                ChildList = new List<PurchaseDetailViewModel>()
                //ChildList = new List<PurchaseDetailViewModel>() {
                //    new PurchaseDetailViewModel() {
                //        SeqNo           = 0 ,
                //        PurchaseID       = "0000000000",
                //        ProductID       = "88888888888888888888",
                //        ProductName     = "產品1",
                //        PriceKindID     = "1",
                //        ConditionID     = "1",
                //        Discount        = 100,
                //        ForeignPrice    = 30000,
                //        Price           = 40000,
                //        Qty             = 1,
                //        Amount          = 40000,
                //        Remarks         ="Remarks",
                //        IsDirty         = true
                //    },
                //    new PurchaseDetailViewModel() {
                //        SeqNo           = 0 ,
                //        PurchaseID       = "0000000000",
                //        ProductID       = "99999999999999999999",
                //        ProductName     = "產品2",
                //        PriceKindID     = "2",
                //        ConditionID     = "2",
                //        Discount        = 100,
                //        ForeignPrice    = 60000,
                //        Price           = 80000,
                //        Qty             = 2,
                //        Amount          = 160000,
                //        Remarks         ="Remarks",
                //        IsDirty         = true
                //    },
                //}
#endif
            };

            InitViewBag(info);
            InitChildViewBag(null);
            return View(info);
        }

        private void InitViewBag(PurchaseComplex info)
        {
            // 倉庫
            ViewBag.CurrencyKindList =
                new SelectList(this._GlobalService.GetCurrencyKindList(), "Key", "Value", info?.Purchase.CurrencyID);

            // 倉庫
            ViewBag.WareHouseList =
                new SelectList(this._GlobalService.GetWarehouseList(), "Key", "Value", info?.Purchase.WarehouseID);

            // 公司供應商廠商名稱
             ViewBag.CompanyList =
              new SelectList(this._GlobalService.GetSupplierList(), "Value", "Display", info?.Purchase?.SupplierID);


            ViewBag.YseNoList =
                    new SelectList(YesNo.GetAll(), "value", "Text", info?.Purchase.Activate);

        }

        private void InitChildViewBag(PurchaseDetailViewModel info)
        {
            ViewBag.PriceKindList =
                new SelectList(this._GlobalService.GetPriceKindList(), "Key", "Value", info?.PriceKindID);

            ViewBag.ConditionKindList =
               new SelectList(this._GlobalService.GetConditionKindList(), "Key", "Value", info?.ConditionID);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PurchaseComplex info)
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
                info = this._PurchaseComplexService.Create(info);
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.CloseWindow = false;
                result.Url = Url.Action("Edit", new { id = info.Purchase.PurchaseID });
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
                var query = this._PurchaseComplexService.Get(id);
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
        [MultipleButton(Name = "action", Argument = "Edit")]
        public ActionResult Edit(PurchaseComplex info)
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
                this._PurchaseComplexService.Update(info);
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.CloseWindow = false;
                result.Url = Url.Action("Edit", new { id = info.Purchase.PurchaseID });
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
                var query = this._PurchaseComplexService.Get(id);
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
        public ActionResult DeleteConfirmed(PurchaseComplex model)
        {
            ResultModel result = new ResultModel();
            try
            {

                #region Service資料庫
                if (this._PurchaseComplexService.IsUsed(model))
                {
                    result.Message = "MessageChaneDelete2UpdateComplete".ToLocalized();
                    model.Purchase.Activate = YesNo.No.Value;
                    this._PurchaseComplexService.Update(model);
                }
                else
                {
                    this._PurchaseComplexService.Delete(model);
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