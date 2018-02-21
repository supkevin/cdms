using Autofac;
using Autofac.Integration.Mvc;

using CDMS.Model.DbContextFactory;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;

namespace CDMS.Web.App_Start
{
    public class AutofacConfig
    {
        /// <summary>
        /// 註冊DI注入物件資料
        /// https://dotblogs.com.tw/mantou1201/2014/06/13/145527
        /// http://kevintsengtw.blogspot.tw/2013/09/aspnet-mvc-autofac.html
        /// https://dotblogs.com.tw/mantou1201/2014/07/07/145845
        /// http://blog.darkthread.net/post-2013-11-02-autofac-notes-2.aspx
        /// </summary>
        public static void Register()
        {
            //容器建立者
            ContainerBuilder builder = new ContainerBuilder();

            //註冊Controller
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            //builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            //註冊DbContextFactory
            string connectionString = ConfigurationManager.ConnectionStrings["CDMSEntities"].ConnectionString;

            builder.RegisterType<DbContextFactory>()
                          .WithParameter("connectionString", connectionString)
                          .As<IDbContextFactory>()
                          .InstancePerHttpRequest();


            // 註冊 Repository UnitOfWork
            builder.RegisterGeneric(typeof(GenericRepository<>)).As(typeof(IRepository<>));
            builder.RegisterType(typeof(UnitOfWork)).As(typeof(IUnitOfWork));

            //註冊Model ActionFilter會用到的
            //builder.RegisterType<PermissionModel>().As<PermissionModel>();
            //builder.RegisterType<PermissionActionFilter>().PropertiesAutowired();//.SingleInstance();

            //註冊Service
            var services = Assembly.Load("CDMS.Service");
            builder.RegisterAssemblyTypes(services).AsImplementedInterfaces();

            // 目前沒使用
            // https://mirkomaggioni.com/2016/10/15/register-a-singleton-service-with-autofac/
            // SingletonToken
            builder.RegisterType<CDMS.Service.SingletonTokenService>()
             .AsSelf()
             .AsImplementedInterfaces()
             .SingleInstance();

            builder.RegisterFilterProvider();

            //建立容器
            IContainer container = builder.Build();
            
            //建立相依解析器
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}