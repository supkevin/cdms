using CDMS.Language;
using CDMS.Model;
using CDMS.Service;
using CDMS.Web.ActionFilter;
using CDMS.Web.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Linq;
namespace CDMS.Web.Controllers
{
    public class StoreController : BaseController
    {
        private readonly IStoreService _storeService;
        private readonly ICountryService _countryService;
        public StoreController(IStoreService storeService, ICountryService countryService)
        {
            this._storeService = storeService;
            this._countryService = countryService;
        }

        public ActionResult Create(int? id)
        {
            try
            {
                #region 驗證
                if (!id.HasValue)
                    return View("Error");
                #endregion

                #region 取資料
                var query = this._countryService.GetById((int)id);

                #endregion

                #region ViewBag

                #endregion

                #region 顯示特殊處理

                #endregion

                #region 下拉選單
                //ViewBag.HrSetEducationalList = new SelectList(this._hrseteducationalService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Educational", "CX_Educational", null);

                Store result = new Store()
                {
                    ID_Country = query.ID_Country
                };
                #endregion

                #region 回傳
                return View(result);
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
        public ActionResult Create(Store model)
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
                //model.CreateID = AccountModel.GetUserInfo().ID_Account;
                //model.CreateTime = DateTime.Now;
                #endregion

                #region Service資料庫
                this._storeService.Create(model);
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

        public ActionResult Edit(int? id)
        {
            try
            {
                #region 驗證
                if (!id.HasValue)
                    return View("Error");
                #endregion

                #region 取資料
                var query = this._storeService.GetById((int)id);
                #endregion

                #region ViewBag

                #endregion

                #region 顯示特殊處理

                #endregion

                #region 下拉選單
                //ViewBag.HrSetEducationalList = new SelectList(this._hrseteducationalService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Educational", "CX_Educational", query.ID_Educational);
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
        public ActionResult Edit(Store model)
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
                //model.ModifyID = AccountModel.GetUserInfo().ID_Account;
                //model.ModifyTime = DateTime.Now;
                #endregion

                #region Service資料庫
                this._storeService.Update(model);
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
        public ActionResult DeleteConfirmed(Store model)
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
                this._storeService.Delete(model);
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
        public ActionResult GetForSelect(int id_country)
        {
            List<Store> result = new List<Store>(0);
            var query = this._storeService.GetForSelect(id_country);
            foreach (var item in query)
            {
                Store Create = new Store()
                {
                    ID_Store = item.ID_Store,
                    CX_Store_Name = item.CX_Store_Name
                };
                result.Add(Create);
            }           
            return Json(result);
        }


     
    }
}