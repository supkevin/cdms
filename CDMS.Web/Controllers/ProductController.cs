using AutoMapper;
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
    public class ProductController : BaseController
    {
        private readonly IProductService _ProductService;
        private readonly IGlobalService _GlobalService;
        private readonly IProductImageComplexService _ImageService;
        private readonly IStockQueryService _Stock;

        public ProductController(IProductService productService,
               IGlobalService globalService,
               IProductImageComplexService image,
               IStockQueryService stock
               )
        {
            this._ProductService = productService;
            this._GlobalService = globalService;
            this._ImageService = image;
            this._Stock = stock;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult _List(
            string start, string finish, string txt,
            string orderby, string sort, int page = 1)
        {
            InitViewBag(null);

            #region 設定頁碼 + 傳前端資料(ViewBag)
            int CurrentPage = page < 1 ? 1 : page;

            ViewBag.p = CurrentPage;
            ViewBag.start = start == null ? "" : start;
            ViewBag.finish = finish == null ? "" : finish;

            ViewBag.txt = txt == null ? "" : txt;
            ViewBag.orderby = sort == null ? "" : orderby;
            ViewBag.sort = sort == null ? "" : sort;
            #endregion

            #region 組出SQL + 產生資料
            string Sql = " 1 = 1 ";
            List<object> obj = new List<object> { start, finish, txt };

            if (!string.IsNullOrEmpty(start))
                Sql += " && (ProductID >= @0)";

            if (!string.IsNullOrEmpty(finish))
                Sql += " && (ProductID <= @1)";

            if (!string.IsNullOrEmpty(txt))
                Sql += " && (ProductName.Contains(@2))";

            var query = this._ProductService.GetListView().Where(Sql, obj.ToArray());

            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sort))
                query = query.OrderBy(orderby + " " + sort);

            #endregion

            #region 回傳
            return View("_List", query.ToPagedList(page, PageSize));
            #endregion
        }

        public ActionResult Create()
        {

            ProductViewModel info = null;
#if DEBUG

            info = new ProductViewModel()
            {
                //ProductID = "0".PadLeft(20, '0'),
                KindID = "1",
                ProductName = "產品名稱",
                UnitID = "1",
                LatestCost = 0,
                SalesCost = 0,
                BizCost = 300,
                Linked = "N",
                ListPrice = 400,
                SetPrice = 500,
                RealPrice = 600,
                SPEC = "SPEC",
                BOM = "BOM",
                SupplierID = "0000000",
                SafeStock = 700,
                Remarks = "Remarks"
            };
#endif

            InitViewBag(null);
            return View(info);
        }

        private void InitViewBag(Product info)
        {
            // 單位
            var unitList = this._GlobalService.GetUnitKindList();
            ViewBag.UnitKindList =
              new SelectList(unitList, "Value", "Text",
                          info?.UnitID as object,
                          unitList.Where(x => x.Disabled == true).Select(x => x.Value).ToList());

            // 產品類別
            var productKindList = this._GlobalService.GetProductKindList();
            ViewBag.ProductKindList =
              new SelectList(productKindList, "Value", "Text",
                          info?.KindID as object,
                          productKindList.Where(x => x.Disabled == true).Select(x => x.Value).ToList());

            // 品牌 此處只顯示品牌代碼
            var brandList = this._GlobalService.GetBrandList();
            ViewBag.BrandList =
              new SelectList(brandList, "Value", "Value",
                          info?.BrandID as object,
                          brandList.Where(x => x.Disabled == true).Select(x => x.Value).ToList());
            
            // 公司
            ViewBag.CompanyList =
              new SelectList(this._GlobalService.GetSupplierList(), "Value", "Display", info?.SupplierID);

            //// 產品名
            //ViewBag.ProductsNameList =
            //  new SelectList(this._GlobalService.GetProductList(), "Value", "Display", info?.SupplierID);

            ViewBag.YseNoList =
             new SelectList(YesNo.GetAll(), "value", "Text", info?.Activate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductViewModel model)
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

                //// 供應商代碼錯誤
                //CheckSupplierID(model);

                // 組成最終的產品編號
                model.ProductID = $"{ model.ProductID.ToString().TrimEnd()} {model.BrandID}";

                var info = Mapper.Map<Product>(model);

                if (this._ProductService.IsDataExists(info))
                {
                    ModelState.AddModelError("ProductID", $"{"ProductID".ToLocalized()}:{info.ProductID} 已經存在！");
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }

                #region Service資料庫
                this._ProductService.Create(info);
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
                var query = this._ProductService.Get(id);
                #endregion

                #region ViewBag
                InitViewBag(query);
                #endregion

                #region 顯示特殊處理

                #endregion

                #region 下拉選單

                #endregion

                #region 回傳
                return View(Mapper.Map<ProductViewModel>(query));
                #endregion
            }
            catch (Exception ex)
            {
                #region 有錯誤時錯誤訊息
                TempData["Message"] = ex.ToString();
                return View("Error");
                #endregion
            }
        }

        public ActionResult Alternative(string id)
        {
            try
            {
                #region 驗證
                if (string.IsNullOrEmpty(id))
                    return View("Error");
                #endregion

                #region 取資料
                var query = this._ProductService.Get(id);
                #endregion

                #region ViewBag
                InitViewBag(query);
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
              
        public ActionResult _Image(string id)
        {
            try
            {                
                if (string.IsNullOrEmpty(id))
                    return View("Error");
                                
                var info = this._ImageService.Get(id);
                                
                return View("_Image", info);             
            }
            catch (Exception ex)
            {
                #region 有錯誤時錯誤訊息
                TempData["Message"] = ex.Message;
                return View("Error");
                #endregion
            }
        }

        public ActionResult Image(string id)
        {
            try
            {
                #region 驗證
                if (string.IsNullOrEmpty(id))
                    return View("Error");
                #endregion

                #region 取資料
                var query = this._ProductService.Get(id);
                #endregion

                #region ViewBag
                InitViewBag(query);
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

        private void CheckSupplierID(ProductViewModel model)
        {

            if (!string.IsNullOrEmpty(model.SupplierID)
                   && (string.IsNullOrEmpty(Request["txtSupplierName"])))
            {
                ModelState.AddModelError("SupplierID",
                    $"{"SupplierID".ToLocalized()}:{model.SupplierID} 代碼錯誤！");

                throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "Edit")]
        public ActionResult Edit(ProductViewModel model)
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

                //// 供應商代碼錯誤
                //CheckSupplierID(model);

                #region 前端資料變後端用資料ViewModel時用
                var info = Mapper.Map<Product>(model);
                #endregion

                #region Service資料庫
                this._ProductService.Update(info);
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.CloseWindow = false;
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
                var query = this._ProductService.Get(id);
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
        public ActionResult DeleteConfirmed(Product model)
        {
            ResultModel result = new ResultModel();
            try
            {              
                #region Service資料庫
                if (this._ProductService.IsUsed(model))
                {
                    result.Message = "MessageChaneDelete2UpdateComplete".ToLocalized();

                    model.Activate = YesNo.No.Value;
                    this._ProductService.Update(model);
                }
                else
                {
                    this._ProductService.Delete(model);
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

        [HttpPost]
        public ActionResult GetForSelect(string id)
        {
            List<Product> result = new List<Product>(0);
            var query = this._ProductService.GetForSelect(id);

            foreach (var item in query)
            {
                Product Create = new Product()
                {
                    ProductID = item.ProductID,
                    ProductName = item.ProductName
                };

                result.Add(Create);
            }
            return Json(result);
        }

        public ActionResult GetForAutocomplete(string term)
        {
            var query =
                this._ProductService.GetForAutoComplete(term).ToList()
                .Select(x =>
                    new CodeName
                    {
                        Label = x.ProductName,
                        Value = x.ProductID,
                        Source = x
                    })
                .ToList();

            //return Content(JsonConvert.SerializeObject(query), "application/json");
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //kevin
        public ActionResult GetForSupplierAutocomplete(string term)
        {
            var query =
                this._ProductService.GetForAutoComplete(term)
                .ToList()
                 .Select(x =>
                    new CodeName
                    {
                        Label = x.ProductName,
                        Value = x.ProductID,
                        Source = x
                    })
                .ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        //kevin

        public ActionResult GetPrice(string productID, string priceKind)
        {
            
            decimal result = 0;

            if (!string.IsNullOrEmpty(productID) && !string.IsNullOrEmpty(priceKind))
            {
                var product = this._ProductService.Get(productID);

                if (priceKind == PriceKind.ListPrice.Value)
                {
                    result = product.ListPrice ?? 0;
                }
                else if (priceKind == PriceKind.SetPrice.Value)
                {
                    result = product.SetPrice ?? 0;
                }
                else if (priceKind == PriceKind.RealPrice.Value)
                {
                    result = product.RealPrice ?? 0;
                }
                else {
                    result = 0;
                }          
            }          

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // 產品庫存量
        public ActionResult GetStock(string id)
        {
            int result = 0;
            var query = this._Stock.GetAll().Where(x => x.ProductID == id)                
                .SingleOrDefault();

            if (query != null)
            {
                result = query.QtyTotal;
            }
                           
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}