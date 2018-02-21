using CDMS.Language;
using CDMS.Service;
using CDMS.Model.ViewModel;
using CDMS.Model;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq.Dynamic;
using System.Linq;

namespace CDMS.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _UserService;

        private Boolean _IsDebug = false;

        [System.Diagnostics.Conditional("DEBUG")]
        private void SetFakeUser()
        {            
            User fake = new User()
            {
                UserID = "9999",
                UserName = "測試帳號",
                PermissionID = "1",
                DepartmentID = "999"                    
            };

            WriteCookie(fake);

            _IsDebug = true;
        }

        public LoginController(IUserService userService)
        {
            this._UserService = userService;
        }

        public ActionResult Index(string ReturnUrl)
        {
            #region 測試資料用 正式時全部註解掉
            SetFakeUser();

            if (_IsDebug)
            {
                //轉頁
                return RedirectToAction("Index", "Home", null);
            }
            #endregion

            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        private void WriteCookie(User temp ) {
            UserInfo info = IdentityService.Convert(temp);

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
               1,//取得票證的版本號碼。
               info.UserID.ToString(),//取得與表單驗證票證相關聯的使用者名稱。
               DateTime.Now,
               DateTime.Now.AddHours(18),//取得表單驗證票證到期的本機日期和時間。
               true,//如果核發了持久性 Cookie (跨瀏覽器工作階段儲存的 Cookie)，則為 true，否則為 false。
               JsonClass.ObjectToJson(info),
               FormsAuthentication.FormsCookiePath);

            string encTicket = FormsAuthentication.Encrypt(ticket);
            //寫入登入資訊
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel info, string ReturnUrl)
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

                // 判斷帳號密碼及有效日期
                var query = this._UserService.GetAll()
                            .Where(x => x.UserID == info.UserID 
                                   && x.Password == info.Password
                                   && x.BeginDate <= DateTime.Today
                                   && (x.EndDate.HasValue == false || x.EndDate > DateTime.Today)
                                   )
                            .SingleOrDefault();

                if (null == query)
                {
                    throw new Exception("MessageLoginError".ToLocalized());
                }

                WriteCookie(query);

                #region 導向頁面

                result.Status = true;
                result.Message = "MessageLoginScuess".ToLocalized();
                if (!string.IsNullOrEmpty(ReturnUrl))
                {
                    result.Url = ReturnUrl;
                }
                else
                {
                    result.Url = Url.Action("Index", "Home");
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region 有錯誤時錯誤訊息
                result.Status = false;
                result.Message = ex.Message;
                #endregion
            }

            #region 回傳
            return Json(result);
            #endregion
        }

        //// GET: Login
        //public ActionResult Index(string qwe)
        //{

        //    #region 測試資料用 正式時全部註解掉
        //    string mName = "邱士銀";//邱士銀 蕭安呈 廖建智 李秋華 王曉雯
        //    UserModel u1 = new UserModel()
        //    {
        //        DepartmentID = "2200",
        //        DepartmentName = "資訊部",
        //        DisplayName = mName,
        //        IsApproved = true,
        //        MembershipUserName = "marco",
        //        PosStore = "0000",
        //        UserID = "6935",//6935 5164 6057 0245 8500
        //        StoreID = "2200",
        //        TitleName = "高級專員",
        //        StoreName = "資訊部"
        //    };

        //    FormsAuthenticationTicket ticket1 = new FormsAuthenticationTicket(
        //        1,//取得票證的版本號碼。
        //        mName,//取得與表單驗證票證相關聯的使用者名稱。
        //        DateTime.Now,
        //        DateTime.Now.AddHours(12),//取得表單驗證票證到期的本機日期和時間。
        //        false,//如果核發了持久性 Cookie (跨瀏覽器工作階段儲存的 Cookie)，則為 true，否則為 false。
        //        JsonClass.ObjectToJson(u1),
        //        FormsAuthentication.FormsCookiePath);

        //    string encTicket1 = FormsAuthentication.Encrypt(ticket1);
        //    //寫入登入資訊
        //    Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket1));
        //    //轉頁
        //    return RedirectToAction("Index", "Home", null);

        //    #endregion

        //    if (string.IsNullOrEmpty(qwe))
        //    {        
        //        return View();
        //    }


        //    //有參數
        //    try
        //    {
        //        // 取得使用者資料 //
        //        UserInfo info = TransferUser.DecryptUser(qwe);

        //        if (info == null)
        //            return View();

        //        //驗正過就先登出，因為上層登出不會包括這裡的，怕下一個人登時資料會是上一個人的
        //        if (User.Identity.IsAuthenticated)
        //        {
        //            //UserModel script = new UserModel();
        //            //UserModel user = script.GetUserData();
        //            string GStaffID = UserModel.GetUserData().UserID;

        //            if (!GStaffID.Equals(info.ID))
        //            {//非同一個人時先登出
        //                FormsAuthentication.SignOut();
        //            }
        //            else
        //            {
        //                return new RedirectResult(info.URL);
        //            }
        //        }

        //        StoreDeptModel scriptSD = new StoreDeptModel();
        //        StoreDeptModel storeDept = scriptSD.GetStoreDept(info.DeptID);

        //        if (storeDept == null)
        //        {
        //            ModelState.AddModelError(string.Empty, "系統 : (" + info.DeptID + ")部門對應失敗，請聯絡資訊部!!");
        //            return View();
        //        }

        //        UserModel u = new UserModel()
        //        {
        //            DepartmentID = info.DeptID,
        //            DepartmentName = info.DeptName,
        //            DisplayName = info.DisplayName,
        //            IsApproved = info.IsApproved,
        //            MembershipUserName = info.MembershipUserName,
        //            PosStore = storeDept.PosStore,
        //            UserID = info.ID,
        //            StoreID = storeDept.StoreID,
        //            TitleName = info.TitleName,
        //            StoreName = storeDept.StoreName
        //        };

        //        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
        //            1,//取得票證的版本號碼。
        //            info.DisplayName,//取得與表單驗證票證相關聯的使用者名稱。
        //            DateTime.Now,
        //            DateTime.Now.AddHours(12),//取得表單驗證票證到期的本機日期和時間。
        //            true,//如果核發了持久性 Cookie (跨瀏覽器工作階段儲存的 Cookie)，則為 true，否則為 false。
        //            JsonClass.ObjectToJson(u),
        //            FormsAuthentication.FormsCookiePath);

        //        string encTicket = FormsAuthentication.Encrypt(ticket);
        //        //寫入登入資訊
        //        Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        //        //轉頁
        //        return RedirectToAction("Index", "Home", null);

        //        // 導頁//
        //        //return RedirectToAction("Index", "Home", null);
        //        //return RedirectToAction(info.URL);
        //        //eturn new RedirectResult(info.URL);
        //    }
        //    catch (Exception ex)
        //    {
        //        string error = ex.Message;
        //    }

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            #region FormTicket登出
            FormsAuthentication.SignOut();
            #endregion

            #region 回傳 (Html.BeginForm)
            return RedirectToAction("Index", "Login", null);
            #endregion
        }
        
    }
}