using CDMS.Language;
using CDMS.Model.ViewModel;
using CDMS.Service;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Linq;
using CDMS.Model;

namespace CDMS.Web.Controllers
{
    public class PurchaseInvoiceController : BaseController
    {        
        private readonly IPurchaseInvoiceComplexService _PurchaseInvoiceComplexService;        
        private readonly IGlobalService _GlobalService;

        public PurchaseInvoiceController(
               IGlobalService globalService,
               IPurchaseInvoiceComplexService PurchaseInvoiceComplexService
               )
        {                
            this._PurchaseInvoiceComplexService = PurchaseInvoiceComplexService;
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
                    company, product , productName
                };

            //if (!string.IsNullOrEmpty(start))
            //   sql += " && (PurchaseInvoiceDate >=(@0))";

            //if (!string.IsNullOrEmpty(finish))
            //    sql += " && (PurchaseInvoiceDate <=(@1))";

            //if (!string.IsNullOrEmpty(company))
            //    sql += " && (CompanySupplier.Contains(@2))";

            var query = 
                this._PurchaseInvoiceComplexService.GetAll().Where(sql, obj.ToArray());

            orderby = "InvoiceID";
            sort = "desc";

            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sort))
                query = query.OrderBy(orderby + " " + sort);

            #endregion

            #region 回傳
            return View("_List", query.ToPagedList(page, PageSize));
            #endregion
        }

        [HttpGet]
        public ActionResult _Item(string id)
        {
            //child action don't share the same ViewBag with its “parents” action
            var info = new PurchaseInvoiceDetailViewModel()
            {
                SeqNo = 0,
                IsDirty = true
            };

            InitChildViewBag(null);

            return PartialView("_Item", info);
        }

        public ActionResult Create()
        {
            var info = new PurchaseInvoiceComplex()
            {

#if DEBUG
                Invoice = new PurchaseInvoiceViewModel()
                {                                      
                    InvoiceID = "",
                    InvoiceDate = DateTime.Today,
                    Title = "Title",
                    TaxID = "0".PadLeft(8, '0'),    //統一編號
                    TaxLevelID = TaxLevel.TaxInclude.Value,
                    TaxExcluded = 10000,
                    TaxIncluded = 10500,
                    Tax = 500,
                    DiscountAmount = 2000,
                    DeductAmount = 1000,
                    SupplierID = "888888",
                    AccountMonth = "1712",
                    InvoiceStatusID = InvoiceStatus.Valid.Value,
                    Remarks = "Remarks"
                },                
                ChildList = new List<PurchaseInvoiceDetailViewModel>() {
                    new PurchaseInvoiceDetailViewModel() {
                        SeqNo           = 0, 
                        InvoiceID       = "",
                        ProductID       = "9".PadLeft(20, '9'),
                        Price           = 2000,
                        Qty             = 2,
                        Amount          = 4000,
                        IsDirty         = true
                    },
                    new PurchaseInvoiceDetailViewModel() {                        
                        SeqNo           = 0,
                        InvoiceID       = "",
                        ProductID       = "8".PadLeft(20, '8'),
                        Price           = 1000,
                        Qty             = 6,
                        Amount          = 6000,
                        IsDirty         = true
                    },
                }
#endif
            };
            
            InitViewBag(info);
            InitChildViewBag(null);
            return View(info);
        }

        private void InitViewBag(PurchaseInvoiceComplex info)
        {                      
            // 公司供應商廠商名稱
            ViewBag.CompanyList =
                new SelectList(this._GlobalService.GetCompanyShortNameList(), "Key", "Value", null);

            // 課稅別
            ViewBag.TaxLevelList =
                new SelectList(TaxLevel.GetAll(), "Value", "Text", null);

            // 營業稅率
            ViewBag.TaxRateList =
                new SelectList(TaxRate.GetAll(), "Value", "Text", null);

            // 狀態
            ViewBag.InvoiceStatusList =
                new SelectList(InvoiceStatus.GetAll(), "Value", "Text", null);
        }

        private void InitChildViewBag(PurchaseInvoiceDetailViewModel info)
        {
            //ViewBag.PriceKindList =
            //    new SelectList(this._GlobalService.GetPriceKindList(), "Key", "Value", info?.PriceKindID);

            //ViewBag.ConditionKindList =
            //   new SelectList(this._GlobalService.GetConditionKindList(), "Key", "Value", info?.ConditionID);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PurchaseInvoiceComplex info)
        {
            ResultModel result = new ResultModel();
            try
            {
                #region 驗證Model
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }
                
                if (this._PurchaseInvoiceComplexService.IsDataExists(info))
                {
                    ModelState.AddModelError("InvoiceID", 
                        $"{"InvoiceID".ToLocalized()}:{info.Invoice.InvoiceID} 已經存在！");
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }

                #endregion

                #region 前端資料變後端用資料ViewModel時用

                #endregion

                #region Service資料庫
                info = this._PurchaseInvoiceComplexService.Create(info);
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.CloseWindow = false;
                result.Url = Url.Action("Edit", new { id = info.Invoice.InvoiceID });
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
                var query = this._PurchaseInvoiceComplexService.Get(id);
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
        public ActionResult Edit(PurchaseInvoiceComplex info)
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

                #endregion

                #region Service資料庫
                this._PurchaseInvoiceComplexService.Update(info);
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.CloseWindow = false;
                result.Url = Url.Action("Edit", new { id = info.Invoice.InvoiceID });
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

        [HttpPost, ActionName("RemoveChild")]
        public ActionResult RemoveChild(long id)
        {
            ResultModel result = new ResultModel();

            try
            {
                #region Service資料庫
                if (id != 0)
                {
                    this._PurchaseInvoiceComplexService.RemoveChild(id);
                }
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
                var query = this._PurchaseInvoiceComplexService.Get(id);
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(PurchaseInvoiceComplex info)
        {
            ResultModel result = new ResultModel();
            try
            {
                #region Service資料庫
                this._PurchaseInvoiceComplexService.Delete(info);
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
    }
}