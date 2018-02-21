using CDMS.Language;
using CDMS.Model;
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
    public class ObservationController : BaseController
    {
        private readonly IObservationService _observationService;
        private readonly IFeedbackService _feedbackService;


        public ObservationController(IObservationService observationService, IFeedbackService feedbackService)
        {
            this._observationService = observationService;
            this._feedbackService = feedbackService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult _List(string txt, string orderby, string sort, int page = 1)
        {
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
                Sql += " && (CX_Observation.Contains(@0) || CX_Observation_Remarks.Contains(@0))";

            var query = this._observationService.GetAll().Where(Sql, obj.ToArray());

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

        private void InitViewBag(Observation info) {
            ViewBag.FeedBackList = 
                new SelectList(this._feedbackService.GetAll().OrderBy(x => x.NQ_Sort), 
                "ID_Feedback", "CX_Feedback", info?.ID_Feedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Observation model)
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
                this._observationService.Create(model);
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
                var query = this._observationService.GetById((int)id);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Observation model)
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
                this._observationService.Update(model);
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

        public ActionResult Delete(int? id)
        {
            try
            {
                #region 驗證
                if (!id.HasValue)
                    return View("Error");
                #endregion

                #region 取資料
                var query = this._observationService.GetById((int)id);
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
        public ActionResult DeleteConfirmed(Observation model)
        {
            ResultModel result = new ResultModel();
            try
            {
                #region Service資料庫
                this._observationService.Delete(model);
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
        public ActionResult GetForSelect(int id_feedback)
        {
            List<Observation> result = new List<Observation>(0);
            var query = this._observationService.GetForSelect(id_feedback);

            foreach (var item in query)
            {
                Observation Create = new Observation()
                {
                    ID_Observation = item.ID_Observation,
                    CX_Observation = item.CX_Observation
                };
                result.Add(Create);
            }
            return Json(result);
        }
    }
}