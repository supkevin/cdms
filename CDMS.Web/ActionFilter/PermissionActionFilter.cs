using System.Web.Mvc;
using System.Web.Routing;
//using Web.Models;
using CDMS.Language;
using CDMS.Web;

namespace CDMS.Web.ActionFilter
{
    public class PermissionActionFilter : AuthorizeAttribute
    {
        //public PermissionModel permissionModel { get; set; }//autofac註冊

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //base.OnAuthorization(filterContext);
            //if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            //{
            //    string area = string.Empty;//若沒使用則為null     
            //    if (filterContext.RouteData.DataTokens.ContainsKey("area"))
            //        area = filterContext.RouteData.DataTokens["area"].ToString();

            //    string controller = filterContext.RouteData.Values["controller"].ToString();
            //    string action = filterContext.RouteData.Values["action"].ToString();
            //    int id_role = AccountModel.GetUserInfo().ID_Role;



            //    //是否可進本頁
            //    bool IsPermission = permissionModel.GetForPermission(id_role, area, controller, action);

            //    if (!IsPermission)
            //    {
            //        filterContext.Controller.TempData.Add("Message", "MessageNoPermission".ToLocalized());
            //        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "Index",area="" }));
            //    }
            //}

        }


    }
}