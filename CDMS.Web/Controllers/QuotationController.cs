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
    public class QuotationController : BaseController
    {
        private readonly IQuotationComplexService _QuotationComplexService;        
        private readonly IGlobalService _GlobalService;

        public QuotationController(               
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
                this._QuotationComplexService.GetAll().Where(sql, obj.ToArray());

            orderby = "Quotation.QuotationID";
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
            var info = new QuotationDetailViewModel()
            {
                SeqNo = 0,
                Discount = 100,
                IsDirty = true
            };

            InitChildViewBag(null);

            return PartialView("_Item", info);
        }

        QuotationComplex _Info = new QuotationComplex() {
            Quotation = new QuotationViewModel()
            {
                QuotationDate = DateTime.Today,
                QuotePerson = IdentityService.GetUserData().UserID,
                ValidateDate = DateTime.Today.AddDays(30),
            },
            ChildList = new List<QuotationDetailViewModel>()
        };

        [System.Diagnostics.Conditional("DEBUG")]
        private void GenerateFakeData() {
            _Info.Quotation.InvoiceAddress = "發票地址";
            _Info.Quotation.ShippingAddress = "送貨地址";
        }

        public ActionResult Create()
        {
            var info = _Info;
            GenerateFakeData();

//            var info = new QuotationComplex()
//            {

            //#if DEBUG
            //                Quotation = new QuotationViewModel()
            //                {                    
            //                    //QuotationID = "0".PadLeft(10, '0'),
            //                    QuotationDate = DateTime.Today,                                    
            //                    CustomerID = "0".PadLeft(6, '0'),
            //                    TaxID = "0".PadLeft(8, '0'),
            //                    //ContactPerson = "報價單聯絡人",
            //                    //ContactPhone = "報價單聯絡人電話",
            //                    InvoiceAddress = "發票地址",
            //                    ShippingAddress = "送貨地址",
            //                    ShippingModeID = "1",
            //                    ShippingFee = 1000,
            //                    Total = 1000,
            //                    Remarks = "Remarks",
            //                    QuotePerson = "報價人",
            //                    ValidateDate = DateTime.Today.AddDays(30),                                  
            //                },          
            //                ChildList = new List<QuotationDetailViewModel>()
            //                //ChildList = new List<QuotationDetailViewModel>() {
            //                //    new QuotationDetailViewModel() {
            //                //        SeqNo           = 0 ,
            //                //        QuotationID       = "0000000000",
            //                //        ProductID       = "88888888888888888888",
            //                //        ProductName     = "產品1",
            //                //        PriceKindID     = "1",
            //                //        ConditionID     = "1",
            //                //        Discount        = 100,
            //                //        Price           = 40000,
            //                //        Qty             = 1,
            //                //        Amount          = 40000,
            //                //        Remarks         ="Remarks"
            //                //    },
            //                //    new QuotationDetailViewModel() {
            //                //        SeqNo           = 0 ,
            //                //        QuotationID       = "0000000000",
            //                //        ProductID       = "99999999999999999999",
            //                //        ProductName     = "產品2",
            //                //        PriceKindID     = "2",
            //                //        ConditionID     = "2",
            //                //        Discount        = 100,
            //                //        Price           = 80000,
            //                //        Qty             = 2,
            //                //        Amount          = 160000,
            //                //        Remarks         ="Remarks"
            //                //    },
            //                //}
            //#endif
            //            };

            InitViewBag(info);
            InitChildViewBag(null);
            return View(info);
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
                new SelectList(ResultType.GetAll(), "value", "Text", info?.Quotation.Result);

            ViewBag.YseNoList =
               new SelectList(YesNo.GetAll(), "value", "Text", info?.Quotation.Activate);

            
            // 報價人
            ViewBag.UserList =
               new SelectList(this._GlobalService.GetUserList(), "Value", "Display", info?.Quotation.QuotePerson);
            
        }

        private void InitChildViewBag(QuotationDetailViewModel info)
        {
            ViewBag.PriceKindList =
                new SelectList(this._GlobalService.GetPriceKindList(), "Key", "Value", info?.PriceKindID);

            ViewBag.ConditionKindList =
               new SelectList(this._GlobalService.GetConditionKindList(), "Key", "Value", info?.ConditionID);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuotationComplex info)
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
                info = this._QuotationComplexService.Create(info);
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

        public ActionResult Edit(string id)
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
        [MultipleButton(Name = "action", Argument = "Edit")]
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
                this._QuotationComplexService.Update(info);
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

        public ActionResult Delete(string id)
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

        [HttpPost, ActionName("RemoveChild")]
        public ActionResult RemoveChild(long id)
        {
            ResultModel result = new ResultModel();

            try
            {
                #region Service資料庫
                if (id != 0)
                {
                    this._QuotationComplexService.RemoveChild(id);
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
        public ActionResult DeleteConfirmed(QuotationComplex model)
        {
            ResultModel result = new ResultModel();

            try
            {
                #region Service資料庫
                if (this._QuotationComplexService.IsUsed(model))
                {
                    result.Message = "MessageChaneDelete2UpdateComplete".ToLocalized();
                    model.Quotation.Activate = YesNo.No.Value;
                    this._QuotationComplexService.Update(model);
                }
                else
                {
                    this._QuotationComplexService.Delete(model);
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