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
    public class NewsController : BaseController
    {
        private readonly INewsService _NewsService;
        private readonly IGlobalService _GlobalService;

        public NewsController(
            INewsService newsService,
            IGlobalService globalService)
        {
            this._NewsService = newsService;            
            this._GlobalService = globalService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Display()
        {
            return View();
        }

        [HttpPost]
        public ActionResult _Display(int page = 1)
        {
            InitViewBag(null);

            #region 設定頁碼 + 傳前端資料(ViewBag)
            int CurrentPage = page < 1 ? 1 : page;

            ViewBag.p = CurrentPage;
            #endregion

            #region 組出SQL + 產生資料            
            var query = this._NewsService.GetAll()
                        .Where(x=> x.Activate == YesNo.Yes.Value 
                            && x.ReleaseDate <= DateTime.Now 
                            && x.OffDate > DateTime.Now)
                        .OrderByDescending(x=>x.SetTop)
                        .ThenByDescending(x => x.ReleaseDate);
                        
            #endregion

            #region 回傳
            return View("_Display", query.ToPagedList(page, PageSize));
            #endregion
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
               Sql += " && (Content.Contains(@0))";

            var query = this._NewsService.GetAll().Where(Sql, obj.ToArray());
            
            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sort))
               query = query.OrderBy(orderby + " " + sort);
          
            #endregion

            #region 回傳
            return View("_List", query.ToPagedList(page, PageSize));
            #endregion
        }

        public ActionResult Create()
        {
            var info = new NewsViewModel() {
                ReleaseDate = DateTime.Today,
                OffDate = DateTime.Today.AddDays(7),
            };

            InitViewBag(null);
            return View(info);
        }

        private void InitViewBag(News info) {

            var newsTypeList = this._GlobalService.GetNewsTypeList();

            ViewBag.NewsTypeList =
              new SelectList(newsTypeList, "Value", "Text",
                            info?.NewsTypeID as object,
                            newsTypeList.Where(x => x.Disabled == true).Select(x => x.Value));


            var departmentList = this._GlobalService.GetDepartmentList();

            ViewBag.DepartmentList = 
                new SelectList(departmentList, "Value", "Text",
                            info?.DepartmentID as object,
                            departmentList.Where(x => x.Disabled == true).Select(x => x.Value).ToList());

            ViewBag.YseNoList =
                new SelectList(YesNo.GetAll(), "value", "Text", info?.SetTop);
        }

        private News GetInfo(News model) {

            // 不在model內的欄位可這樣處理
            var txtReleaseTime = Request.Form["txtReleaseTime"];
            var txtOffTime = Request.Form["txtOffTime"];

            string releaseTime = model.ReleaseDate.Value.Date.ToString(GlobalSettings.DATE_FORMAT);
            releaseTime = string.Format("{0} {1}", releaseTime, txtReleaseTime);

            string offTime = model.OffDate.Value.Date.ToString(GlobalSettings.DATE_FORMAT);
            offTime = string.Format("{0} {1}", offTime, txtOffTime);

            try
            {
                model.ReleaseDate = Convert.ToDateTime(releaseTime);
                model.OffDate = Convert.ToDateTime(offTime);
            }
            catch (Exception ex)
            {
                throw new Exception("MessageDateTimeError".ToString());
            }

            if (model.ReleaseDate > model.OffDate)
            {

                ModelState.AddModelError("ReleaseDate", "發佈日期不可大於下架日期");
            }
            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewsViewModel model)
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
                var info = Mapper.Map<News>(model);

                info = GetInfo(info);

                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }
                #endregion


                #region Service資料庫
                this._NewsService.Create(info);
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

        public ActionResult Edit(long id)
        {
            try
            {                
                #region 驗證
                //if (string.IsNullOrEmpty(id)) 
                //    return View("Error");
                #endregion

                #region 取資料
                var query = this._NewsService.Get(id);
                #endregion

                #region ViewBag
                InitViewBag(query);
                #endregion

                #region 顯示特殊處理

                #endregion

                #region 下拉選單

                #endregion

                var info = Mapper.Map<NewsViewModel>(query);

                #region 回傳
                return View(info);
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
        public ActionResult Edit(NewsViewModel model)
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

                var info = Mapper.Map<News>(model);

                info = GetInfo(info);

                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }
                #endregion

                #region Service資料庫
                this._NewsService.Update(info);
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

        public ActionResult Delete(long id)
        {
            try
            {
                #region 驗證
                //if (string.IsNullOrEmpty(id))
                //    return View("Error");
                #endregion

                #region 取資料
                var query = this._NewsService.Get(id);
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
        public ActionResult DeleteConfirmed(News model)
        {
            ResultModel result = new ResultModel();
            try
            {
                #region Service資料庫
                if (this._NewsService.IsUsed(model))
                {
                    result.Message = "MessageChaneDelete2UpdateComplete".ToLocalized();

                    model.Activate = YesNo.No.Value;
                    this._NewsService.Update(model);
                }
                else
                {
                    this._NewsService.Delete(model);
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