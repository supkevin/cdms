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
using CDMS.Web.Common;
using System.Diagnostics;

namespace CDMS.Web.Controllers
{
    public class InquiryController : BaseController
    {        
        private readonly IInquiryComplexService _InquiryComplexService;
        private readonly ITokenService _SingletonTokenService;
        private readonly IGlobalService _GlobalService;

        public InquiryController(
               ITokenService tokenService,
               IGlobalService globalService,
               IInquiryComplexService inquiryComplexService
               )
        {            
            this._SingletonTokenService = tokenService;
            this._InquiryComplexService = inquiryComplexService;
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

            //var sq = titulo.Where("codFluxoConta == @0 && codEmpresaFluxoConta == @1", 
            //                        "SomeValue", "StringNumerical");
            //var result = documento.Where("@0.Any(it.codDocumento == outerIt.codDocumento && it.codEmpresaDocumento == outerIt.codEmpresaDocumento)", sq)
            //    .Select("new(codDocumento, codEmpresaDocumento)");

            if (!string.IsNullOrEmpty(start))
               sql += " && (Inquiry.InquiryDate >=(@0))";

            if (!string.IsNullOrEmpty(finish))
                sql += " && (Inquiry.InquiryDate <=(@1))";

            if (!string.IsNullOrEmpty(company))
                sql += " && (Inquiry.CompanySupplier.Contains(@2))";

            // 查詢Detail
            // https://stackoverflow.com/questions/30944201/how-to-make-exists-with-dynamic-linq
            if (!string.IsNullOrEmpty(product))
                sql += " && (ChildList.Any(ProductID.Contains(@3)))";

            if (!string.IsNullOrEmpty(productName))
                sql += " && (ChildList.Any(ProductName.Contains(@4)))";

            var query =
                this._InquiryComplexService.GetAll().Where(sql, obj.ToArray());

            orderby = "Inquiry.InquiryID";
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
            var info = new InquiryDetailViewModel()
            {
                SeqNo = 0,
                Discount = 100,
                IsDirty = true
            };

            InitChildViewBag(null);

            return PartialView("_Item", info);
        }

        [Conditional("DEBUG")]
        private void GenerateFake(InquiryComplex info) {

        }

        public ActionResult Create()
        {
            var info = new InquiryComplex()
            {

#if DEBUG
                Inquiry = new InquiryViewModel()
                {
                    //InquiryID = "0".PadLeft(10, '0'),
                    InquiryDate = DateTime.Today,
                    //ContactPerson = "聯絡人",
                    //ContactPhone = "聯絡電話",                    
                    CurrencyID = "1",
                    ExchangeRate = 1,
                    Remarks = "Remarks",
                    ValidateDate = DateTime.Today.AddDays(30),
                    ScheduleDate = DateTime.Today.AddDays(180),
                    Remind = 1
                },
                ChildList = new List<InquiryDetailViewModel>()
                //ChildList = new List<InquiryDetailViewModel>() {
                //    new InquiryDetailViewModel() {
                //        SeqNo           = 0 ,
                //        //InquiryID       = "0000000000",
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
                //    new InquiryDetailViewModel() {
                //        SeqNo           = 0 ,
                //        //InquiryID       = "0000000000",
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

        private void InitViewBag(InquiryComplex info)
        {
            ViewBag.Token = _SingletonTokenService.GetToken();
            
            // 幣別
            ViewBag.CurrencyKindList =
                new SelectList(this._GlobalService.GetCurrencyKindList(), "Key", "Value", info?.Inquiry.CurrencyID);

            // 公司
            ViewBag.CompanyList =
              new SelectList(this._GlobalService.GetSupplierList(), "Value", "Display", info?.Inquiry?.CompanyID);

            ViewBag.YseNoList =
               new SelectList(YesNo.GetAll(), "value", "Text", info?.Inquiry.Activate);
        }

        private void InitChildViewBag(InquiryDetailViewModel info)
        {
            ViewBag.PriceKindList =
                new SelectList(this._GlobalService.GetPriceKindList(), "Key", "Value", info?.PriceKindID);

//-----------------kevin新增修改   begin------------------
            ViewBag.ProductNameList =
              new SelectList(this._GlobalService.GetProductList(), "Value", "Value", info?.ProductID);
//-----------------kevin新增修改   end--------------------

            ViewBag.ConditionKindList =
               new SelectList(this._GlobalService.GetConditionKindList(), "Key", "Value", info?.ConditionID);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InquiryComplex info)
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
                    ModelState.AddModelError("AlternativeID", "至少需有一筆明細資料。");
                }
                else
                {
                    var query =
                       info.ChildList
                       .GroupBy(x => x.ProductID)
                       .ToDictionary(x => x.Key, x => x.Count())
                       .Where(x => x.Value > 1)
                       .ToList();

                    DuplicateValidator validator = new DuplicateValidator(query);

                    if (validator.Message.Count > 0)
                    {
                        foreach (var s in validator.Message)
                        {
                            ModelState.AddModelError("AlternativeID", s);
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
                info = this._InquiryComplexService.Create(info);
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.CloseWindow = false;
                result.Url = Url.Action("Edit", new { id = info.Inquiry.InquiryID });
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
                var query = this._InquiryComplexService.Get(id);
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
        public ActionResult Edit(InquiryComplex info)
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
                this._InquiryComplexService.Update(info);
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.CloseWindow = false;
                result.Url = Url.Action("Edit", new { id = info.Inquiry.InquiryID });
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
                var query = this._InquiryComplexService.Get(id);
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
                    this._InquiryComplexService.RemoveChild(id);
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
        public ActionResult DeleteConfirmed(InquiryComplex model)
        {
            ResultModel result = new ResultModel();

            try
            {          
                #region Service資料庫
                if (this._InquiryComplexService.IsUsed(model))
                {
                    result.Message = "MessageChaneDelete2UpdateComplete".ToLocalized();
                    model.Inquiry.Activate = YesNo.No.Value;
                    this._InquiryComplexService.Update(model);
                }
                else
                {
                    this._InquiryComplexService.Delete(model);
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

        ////[ValidateAntiForgeryToken]
        //public ActionResult Delete(string id)
        //{
        //    ResultModel result = new ResultModel();

        //    try
        //    {
        //        #region 驗證
        //        if (string.IsNullOrEmpty(id))
        //            return View("Error");
        //        #endregion

        //        var info = this._InquiryComplexService.Get(id);


        //        if (null != info)
        //        {
        //            this._InquiryComplexService.Delete(info);
        //        }
                
        //        #region 訊息頁面設定
        //        result.Status = true;
        //        result.Message = "MessageComplete".ToLocalized();
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        #region 有錯誤時錯誤訊息
        //        result.Status = false;
        //        result.Message = ex.Message.ToString();
        //        #endregion
        //    }
        //    return Json(result);
        //}

        [HttpPost]
        public ActionResult _History(string key)
        {            
            var info = _InquiryComplexService.GetHistory(key);
            
            return PartialView("_History", info);
        }
    }
}