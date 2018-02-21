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
    public class BrandController : BaseController
    {
        private readonly IBrandService _BrandService;
                
        public BrandController(IBrandService brandService)
        {
            this._BrandService = brandService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult _List(string txt, string orderby, string sort, int page = 1)
        {
            InitViewBag(null);

            #region 設定頁碼 + 傳前端資料(ViewBag)
            int CurrentPage = page < 1 ? 1 : page;

            ViewBag.p = CurrentPage;
            ViewBag.txt = txt == null ? "" : txt;
            ViewBag.orderby = sort == null ? "" : orderby;
            ViewBag.sort = sort == null ? "" : sort;
            #endregion

            #region 組出SQL + 產生資料
            string Sql = " 1 = 1 ";
            List<object> obj = new List<object> { txt };

            if (!string.IsNullOrEmpty(txt))
                Sql += " && (BrandName.Contains(@0) || Remarks.Contains(@0))";

            var query = this._BrandService.GetAll().Where(Sql, obj.ToArray());            

            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sort))
                query = query.OrderBy(orderby + " " + sort);

            #endregion
           
            #region 回傳
            return View("_List", query.ToPagedList(page, PageSize));
            #endregion
        }

        public ActionResult Create()
        {
            InitViewBag(null);
            return View();
        }

        private void InitViewBag(Brand info)
        {
            ViewBag.YseNoList =
               new SelectList(YesNo.GetAll(), "value", "Text", info?.Activate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BrandViewModel model)
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

                var info = Mapper.Map<Brand>(model);

                if (this._BrandService.IsCodeExists(info))
                {
                    ModelState.AddModelError("BrandID", $"{"BrandID".ToLocalized()}:{info.BrandID} 已經存在！");
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }

                #region 前端資料變後端用資料ViewModel時用

                #endregion

                #region Service資料庫
                this._BrandService.Create(info);
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


                var query = this._BrandService.Get(id);


                InitViewBag(query);

                query = Mapper.Map<BrandViewModel>(query);

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
        public ActionResult Edit(BrandViewModel model)
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

                Brand info = Mapper.Map<Brand>(model);

                #region Service資料庫
                this._BrandService.Update(info);
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
                var query = this._BrandService.Get(id);
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
        public ActionResult DeleteConfirmed(Brand model)
        {
            ResultModel result = new ResultModel();
            try
            {
                #region Service資料庫
                if (this._BrandService.IsUsed(model))
                {
                    result.Message = "MessageChaneDelete2UpdateComplete".ToLocalized();

                    model.Activate = YesNo.No.Value;
                    this._BrandService.Update(model);
                }
                else
                {
                    this._BrandService.Delete(model);
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
            List<Brand> result = new List<Brand>(0);
            var query = this._BrandService.GetForSelect(id);

            foreach (var item in query)
            {
                Brand Create = new Brand()
                {
                    BrandID = item.BrandID,
                    BrandName = item.BrandName
                };

                result.Add(Create);
            }
            return Json(result);
        }
    }
}