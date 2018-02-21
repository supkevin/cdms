using CDMS.Language;
using CDMS.Model;
using CDMS.Service;
using CDMS.Model.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Linq;
using AutoMapper;

namespace CDMS.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _UserService;
        private readonly IGlobalService _GlobalService;

        public UserController(
            IUserService userService,
            IGlobalService globalService)
        {
            this._UserService = userService;
            this._GlobalService = globalService;
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
                Sql += " && (UserName.Contains(@0) || Remarks.Contains(@0))";

            var query = this._UserService.GetListView().Where(Sql, obj.ToArray());            

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

        private void InitViewBag(User info)
        {          
            // 職稱
            var titleList = this._GlobalService.GetTitleList();
            ViewBag.TitleList =
              new SelectList(titleList, "Value", "Text",
                            info?.TitleID as object,
                            titleList.Where(x => x.Disabled == true).Select(x => x.Value));
            // 權限
            var quotationLevelList = this._GlobalService.GetQuotationLevelList();
            ViewBag.QuotationLevelList = 
                new SelectList(quotationLevelList, "Value", "Text",
                            info?.QuotationLevelID as object,
                            quotationLevelList.Where(x => x.Disabled == true).Select(x => x.Value));

            // 權限
            ViewBag.PermissionList =            
                new SelectList(Permission.GetAll(), "value", "Text", info?.PermissionID);

            var departmentList = this._GlobalService.GetDepartmentList();

            ViewBag.DepartmentList =
                new SelectList(departmentList, "Value", "Text",
                            info?.DepartmentID as object,
                            departmentList.Where(x => x.Disabled == true).Select(x => x.Value).ToList());

            ViewBag.YseNoList =
               new SelectList(YesNo.GetAll(), "value", "Text", info?.Activate);

   
            var userList = this._GlobalService.GetUserList()
                .Where(x=>x.Value != info?.UserID);

            ViewBag.UserList =
              new SelectList(userList, "Value", "Text",
                            info?.SupervisorID as object,
                            userList.Where(x => x.Disabled == true).Select(x => x.Value));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserViewModel model)
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

                var info = Mapper.Map<User>(model);

                if (this._UserService.IsDataExists(info))
                {
                    ModelState.AddModelError("UserID", $"{"UserID".ToLocalized()}:{info.UserID} 已經存在！");
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }

                #region 前端資料變後端用資料ViewModel時用

                #endregion

                #region Service資料庫
                this._UserService.Create(info);
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
                if (string.IsNullOrEmpty(id))
                    return View("Error");
                
                var query = this._UserService.Get(id);

                InitViewBag(query);

                var info = Mapper.Map<UserViewModel>(query);           
                return View(info);                
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
        public ActionResult Edit(UserViewModel model)
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
                User info = Mapper.Map<User>(model);
                #endregion

                #region Service資料庫
                this._UserService.Update(info);
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
                var query = this._UserService.Get(id);
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
        public ActionResult DeleteConfirmed(UserViewModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                var info = Mapper.Map<User>(model);

                #region Service資料庫
                if (this._UserService.IsUsed(info))
                {
                    result.Message = "MessageChaneDelete2UpdateComplete".ToLocalized();

                    info.Activate = YesNo.No.Value;
                    this._UserService.Update(info);
                }
                else
                {
                    this._UserService.Delete(info);
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
        
        public ActionResult GetForAutocomplete(string term)
        {
            var query =
                this._UserService.GetForAutoComplete(term).ToList()
                 .Select(x => new CodeName { Label = x.UserName, Value = x.UserID, Source = x })
                .ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
    }
}