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
    public class SalesController : BaseController
    {        
        private readonly ISalesComplexService _SalesComplexService;        
        private readonly IGlobalService _GlobalService;

        public SalesController(     
               IGlobalService globalService,
               ISalesComplexService SalesComplexService
               )
        {            
            this._SalesComplexService = SalesComplexService;
            this._GlobalService = globalService;
        }

        public ActionResult Index()
        {
            InitViewBag(null);
            return View();
        }

        [HttpPost]
        public ActionResult _List(
            string start, string finish, 
            string company,
            string product, string productName,
            string[] warehouseID = null, 
            string orderby = "Sales.SalesID", string sort = "desc", int page = 1)
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
            ViewBag.warehouseID = warehouseID == null ? null : warehouseID;

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
                sql += " && (Sales.SalesDate >=(@0))";

            if (!string.IsNullOrEmpty(finish))
                sql += " && (Sales.SalesDate <=(@1))";

            if (!string.IsNullOrEmpty(company))
                sql += " && (Sales.CustomerID.Contains(@2))";

            if (!string.IsNullOrEmpty(product))
                sql += " && (ChildList.Any(ProductID.Contains(@3)))";

            if (!string.IsNullOrEmpty(productName))
                sql += " && (ChildList.Any(ProductName.Contains(@4)))";
     
            var query =
                this._SalesComplexService.GetAll().Where(sql, obj.ToArray());

            orderby = "Sales.SalesID";
            sort = "desc";

            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sort))
                query = query.OrderBy(orderby + " " + sort);

            #endregion

            // IQuerable不能用string[].Contains用這種方式
            query = query.AsEnumerable()
                    .Where(x =>
                        (
                        warehouseID == null || warehouseID.Contains(x.Sales.WarehouseID)
                        )                
                    ).AsQueryable();

            #region 回傳
            return View("_List", query.ToPagedList(page, 1));
            #endregion
        }

        [HttpGet]
        public ActionResult _Item(string id)
        {
            //child action don't share the same ViewBag with its “parents” action
            var info = new SalesDetailViewModel()
            {
                SeqNo = 0,
                Discount = 100, 
                Qty = 1,
                IsDirty = true
            };

            InitChildViewBag(null);

            return PartialView("_Item", info);
        }

        SalesComplex _Info = new SalesComplex()
        {
            Sales = new SalesViewModel()
            {
                SalesDate = DateTime.Today,           
            },
            ChildList = new List<SalesDetailViewModel>()
        };

        [System.Diagnostics.Conditional("DEBUG")]
        private void GenerateFakeData()
        {
                    
        }

        public ActionResult Create()
        {
            var info = _Info;

            GenerateFakeData();
            
            InitViewBag(info);
            InitChildViewBag(null);
            return View(info);
        }

        private void InitViewBag(SalesComplex info)
        {           
            // 倉庫
            ViewBag.WareHouseList =
                new SelectList(this._GlobalService.GetWarehouseList(), "Key", "Value", info?.Sales.WarehouseID);

            // 公司
            ViewBag.CompanyList =
              new SelectList(this._GlobalService.GetCustomerList(), "Value", "Display", info?.Sales?.CustomerID);

            // 運送方式
            ViewBag.ShippingModeList =
                new SelectList(this._GlobalService.GetShippingModeList(), "Key", "Value", null);

            ViewBag.MultiWareHouseList =
              new SelectList(this._GlobalService.GetWarehouseList(), "Key", "Value", null);

            ViewBag.YseNoList =
              new SelectList(YesNo.GetAll(), "value", "Text", info?.Sales.Activate);

        }

        private void InitChildViewBag(SalesDetailViewModel info)
        {                
            ViewBag.PriceKindList =
                new SelectList(this._GlobalService.GetPriceKindList(), "Key", "Value", info?.PriceKindID);

            ViewBag.ConditionKindList =
               new SelectList(this._GlobalService.GetConditionKindList(), "Key", "Value", info?.ConditionID);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SalesComplex info)
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
                info = this._SalesComplexService.Create(info);
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.CloseWindow = false;
                result.Url = Url.Action("Edit", new { id = info.Sales.SalesID });
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
                var query = this._SalesComplexService.Get(id);
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
        public ActionResult Edit(SalesComplex info)
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
                this._SalesComplexService.Update(info);
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.CloseWindow = false;
                result.Url = Url.Action("Edit", new { id = info.Sales.SalesID });
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
                    this._SalesComplexService.RemoveChild(id);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "Delete")]
        public ActionResult DeleteConfirmed(SalesComplex model)
        {
            ResultModel result = new ResultModel();
            try
            {
                #region Service資料庫
                if (this._SalesComplexService.IsUsed(model))
                {
                    result.Message = "MessageChaneDelete2UpdateComplete".ToLocalized();
                    model.Sales.Activate = YesNo.No.Value;
                    this._SalesComplexService.Update(model);
                }
                else
                {
                    this._SalesComplexService.Delete(model);
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
        
        // 最近一筆銷貨金額
        public ActionResult GetLatestSales(string customerID, string productID)
        {            
            var query = this._SalesComplexService
                .GetLatestSales(productID)
                .Where(x => x.CustomerID == customerID)
                .SingleOrDefault();

            // 找不到給預設值
            if (query == null)
            {
                query = new v_CustomerLatestSales()
                {
                    LatestPrice = 0,
                    Discount = 100,
                    Qty = 1, 
                };
            }

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult _History(string key)
        {
            var info = _SalesComplexService.GetHistory(key);

            return PartialView("_History", info);
        }
    }
}