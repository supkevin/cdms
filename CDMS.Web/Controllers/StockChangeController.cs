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

namespace CDMS.Web.Controllers
{
    public class StockChangeController : BaseController
    {
        private readonly IStockChangeComplexService _StockChangeComplexService;
        private readonly IGlobalService _GlobalService;

        public StockChangeController(
               IStockChangeComplexService StockChangeComplexService,
               IGlobalService globalService
               )
        {
            this._StockChangeComplexService = StockChangeComplexService;
            this._GlobalService = globalService;
        }

        public ActionResult Index()
        {
            InitViewBag(null);
            return View();
        }

        [HttpPost]
        public ActionResult _List(
            string start, string finish, string txt,string warehouse,
            string orderby = "ChangeDate", string sort = "desc", int page = 1)
        {
            InitViewBag(null);
                        
            ViewBag.p = page < 1 ? 1 : page;

            ViewBag.start = start;
            ViewBag.finish = finish;
            ViewBag.warehouse = warehouse;

            string sql = " 1 = 1 ";
            List<object> obj = new List<object> {                
                string.IsNullOrEmpty(start) ? DateTime.MinValue : DateTime.Parse(start),
                string.IsNullOrEmpty(finish) ? DateTime.MaxValue : DateTime.Parse(finish),
            };

            if (!string.IsNullOrEmpty(start))
                sql += " && (ChangeDate >=(@0))";

            if (!string.IsNullOrEmpty(finish))
                sql += " && (ChangeDate <=(@1))";

            var query = this._StockChangeComplexService.GetListView().Where(sql, obj.ToArray());

            if (!string.IsNullOrEmpty(warehouse))
            {
                var house = warehouse.Split(',');
                // IQuerable不能用string[].Contains用這種方式
                query = query.AsEnumerable()
                    .Where(x =>
                        ( 
                        house.Contains(x.WarehouseOldID)
                        || house.Contains(x.WarehouseNewID)
                        )
                    ).AsQueryable();
            }

            query = query.OrderBy($"{orderby} {sort}");         
            return View("_List", query.ToPagedList(page, PageSize));
        }

        StockChangeComplex _Info = new StockChangeComplex()
        {
            StockChange = new StockChangeViewModel()
            {
                ChangeDate = DateTime.Today,
                ChangePersonID = IdentityService.GetUserData().UserID,
            },
            ChildList = new List<StockChangeDetailViewModel>()
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

        private void InitViewBag(StockChangeComplex info)
        {
            // 異動原因            
            ViewBag.ChangeReasonList =
                new SelectList(ChangeReason.GetAll(), "value", "Text", info?.StockChange?.ChangeReasonID);

            // 異動原因
            ViewBag.WarehouseList =
                new SelectList(this._GlobalService.GetWarehouseList(), "Key", "Value", null);

            // 異動原因
            ViewBag.WarehouseOldList =
                new SelectList(this._GlobalService.GetWarehouseList(), "Key", "Value", info?.StockChange?.WarehouseOldID);

            // 異動原因
            ViewBag.WarehouseNewList =
                new SelectList(this._GlobalService.GetWarehouseList(), "Key", "Value", info?.StockChange?.WarehouseOldID);

            // 使用者
            ViewBag.UserList =
                new SelectList(this._GlobalService.GetUserList(), "Key", "Value", null);
            
            // 經手人
            ViewBag.UserList =
               new SelectList(this._GlobalService.GetUserList(), "Value", "Display", info?.StockChange?.ChangePersonID);

            // 產品類別            
            ViewBag.ProductKindList =
              new SelectList(this._GlobalService.GetProductKindList(), "Value", "Text");

        }

        private void InitChildViewBag(StockChangeDetailViewModel info)
        {

        }

        private void CheckBusinessRules(StockChangeComplex info) {
            var reason = info.StockChange.ChangeReasonID;

            // 轉倉兩倉庫不能相同
            if (reason == ChangeReason.TransWarehouse.Value)
            {
                if(info.StockChange.WarehouseNewID == info.StockChange.WarehouseOldID)
                { 
                    ModelState.AddModelError("WarehouseOldID", "新倉庫不能為原倉庫。");
                }
            }
            else
            {
                if (info.StockChange.WarehouseNewID != info.StockChange.WarehouseOldID)
                {
                    ModelState.AddModelError("WarehouseOldID", "新倉庫需為原倉庫。");
                }
            }           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StockChangeComplex info)
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

                // 檢核商業邏輯
                CheckBusinessRules(info);

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
                info = this._StockChangeComplexService.Create(info);
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.CloseWindow = false;
                result.Url = Url.Action("Edit", new { id = info.StockChange.StockChangeID });
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

                if (string.IsNullOrEmpty(id))
                    return View("Error");

                var query = this._StockChangeComplexService.Get(id);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "Edit")]
        public ActionResult Edit(StockChangeComplex info)
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

                // 檢核商業邏輯
                CheckBusinessRules(info);

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
                this._StockChangeComplexService.Update(info);
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.CloseWindow = false;
                result.Url = Url.Action("Edit", new { id = info.StockChange.StockChangeID });
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
                var query = this._StockChangeComplexService.Get(id);
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
                    this._StockChangeComplexService.RemoveChild(id);
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

        [HttpGet]
        public ActionResult _Item(string id)
        {
            //child action don't share the same ViewBag with its “parents” action
            var info = new StockChangeDetailViewModel()
            {
                SeqNo = 0,                
                Qty = 1,
                IsDirty = true
            };

            InitChildViewBag(null);

            return PartialView("_Item", info);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[MultipleButton(Name = "action", Argument = "Delete")]
        //public ActionResult DeleteConfirmed(StockChangeComplex model)
        //{
        //    ResultModel result = new ResultModel();
        //    try
        //    {
        //        #region Service資料庫
        //        if (this._StockChangeComplexService.IsUsed(model))
        //        {
        //            result.Message = "MessageChaneDelete2UpdateComplete".ToLocalized();
        //            model.Sales.Activate = YesNo.No.Value;
        //            this._StockChangeComplexService.Update(model);
        //        }
        //        else
        //        {
        //            this._StockChangeComplexService.Delete(model);
        //        }
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
    }
}