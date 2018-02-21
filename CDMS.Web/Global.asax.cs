using CDMS.Language;
using CDMS.Web.App_Start;
using CDMS.Model;
using CDMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Globalization;

namespace CDMS.Web
{
    //https://haacked.com/archive/2011/03/19/fixing-binding-to-decimals.aspx/
    // 前端帶有傳回帶有格式的數字移除格式轉回純數字
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                actualValue = Convert.ToDecimal(valueResult.AttemptedValue,
                    CultureInfo.CurrentCulture);

                //actualValue =
                //    Convert.ToDecimal(string.Format("{0:0.00}", valueResult.AttemptedValue));
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AutoMapperConfig.Configure();  //註冊AutoMapper

            AreaRegistration.RegisterAllAreas();
            
            //decimal 顯示
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //可崁在iframe裡
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
            //多國語言Language
            ResourceStringProvider rp = new ResourceStringProvider(Resources.ResourceManager);
            ModelMetadataProviders.Current = new LocalizedModelMetadataProvider(rp);
            //DI
            AutofacConfig.Register();

            //json
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();            
        }

        void Application_EndRequest(object sender, EventArgs e)
        {
            // 防止Cache
            HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
        }

        // 應該是通過驗證才會進入 
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            // 轉換 HttpContext.Current.User 為 Thread.CurrentPrincipal
            SetThreadPrincipal();
        }
        
        private void SetThreadPrincipal() {

            var context = HttpContext.Current;
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                return;
            }

            var formsIdentity = context.User.Identity as FormsIdentity;
            if (formsIdentity == null)
            {
                return;
            }

            var identity = formsIdentity;
            var ticket = identity.Ticket;
            var userData = ticket.UserData; // Get the stored user-data, in this case, our roles
            //UserModel user = JsonClass.JsonToObject<UserModel>(userData) as UserModel;
            UserInfo user = JsonClass.JsonToObject<UserInfo>(userData) as UserInfo;

            if (null == user)
            {
                return; 
            }

            // 目前登入沒有記錄角色         
            System.Threading.Thread.CurrentPrincipal = Context.User
                = IdentityService.GeneratePrincipal(formsIdentity, null, user);
        }        
    }
}
