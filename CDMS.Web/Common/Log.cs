using System;
using System.Reflection;
using System.IO;
using log4net;
using System.Web;
using System.Text;

namespace CDMS.Web.Common
{
    #region Log Level

    /// <summary>
    /// Log level enumeration
    /// </summary>
    public enum LogLevel
    {
        All,

        /// <summary>        
        /// <para>Fatal level</para>
        /// <para>The Fatal level designates very severe error events that will presumably lead the application to abort.</para>
        /// </summary>
        Fatal,

        /// <summary>
        /// <para>Error level</para>
        /// <para>The Error level designates error events that might still allow the application to continue running.</para>
        /// </summary>            
        Error,

        /// <summary>
        /// <para>Warn level</para>
        /// <para>The Warn level designates potentially harmful situations.</para>
        /// </summary>            
        Warn,

        /// <summary>
        /// <para>Invoice level</para>
        /// <para></para>
        /// </summary>           
        Info,

        /// <summary>
        /// <para>Debug level</para>
        /// <para>The Debug level designates fine-grained informational events that are most useful to debug an application.</para>
        /// </summary>            
        Debug,

        Off
    }

    #endregion Log Level

    /// <summary>
    /// Log class    
    /// </summary>  
    public class Log
    {   
        private static readonly ILog log = log4net.LogManager.GetLogger(typeof(Log));

        /// <summary>
        /// Constructor
        /// </summary>
        public Log()
        {
            //log4net
            string log4netPath = HttpContext.Current.Server.MapPath("~/log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(log4netPath));

            //log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
        }

        /// <summary>
        /// Write log.
        /// </summary>        
        /// <returns></returns>         
        public static void WriteLog(LogLevel level, string message, MethodBase function)
        {
            try
            {                          
                setLogInfo();

                GlobalContext.Properties["Function"] = 
                    string.Format("{0}.{1}", function.ReflectedType.Name, function.Name);

                if ((level == LogLevel.Fatal) && (log.IsFatalEnabled == true))
                {
                    log.Fatal(message);
                }
                else if ((level == LogLevel.Error) && (log.IsErrorEnabled == true))
                {
                    log.Error(message);
                }
                else if ((level == LogLevel.Warn) && (log.IsWarnEnabled == true))
                {
                    log.Warn(message);
                }
                else if ((level == LogLevel.Info) && (log.IsInfoEnabled == true))
                {
                    log.Info(message);
                }
                else if ((level == LogLevel.Debug) && (log.IsDebugEnabled == true))
                {
                    log.Debug(message);
                }

            }
            catch(Exception ex)
            {
            }
        }

        #region Private Info

        private static void setLogInfo()
        {
            GlobalContext.Properties["MachineName"]        = getMachineName();
            GlobalContext.Properties["UserHostAddress"]    = getHostAddress();
            GlobalContext.Properties["UserAgent"]          = getUserAgent();
            GlobalContext.Properties["URI"]                = getURI();
            GlobalContext.Properties["RequestForm"]        = getRequestForm();
            GlobalContext.Properties["RequestQueryString"] = getRequestQueryString();
            GlobalContext.Properties["Uid"]                = getCurrentUid();
        }

        private static string getMachineName()
        {
            return Environment.MachineName;
        }

        private static string getHostAddress()
        {
            if (HttpContext.Current == null)
                return "";
                
            return HttpContext.Current.Request.UserHostAddress;
        }

        private static string getUserAgent()
        {
            if (HttpContext.Current == null)
                return "";
                
            return HttpContext.Current.Request.UserAgent;
        }

        private static string getURI()
        {
            if (HttpContext.Current == null)
                return "";

            return HttpContext.Current.Request.Url.AbsoluteUri;            
        }


        private static string getRequestForm()
        {
            if ((HttpContext.Current == null) || (HttpContext.Current.Request == null) || (HttpContext.Current.Request.Form.Count <= 0))
                return "";
            
            StringBuilder sbuilder = new StringBuilder();
            foreach (string key in HttpContext.Current.Request.Form.AllKeys)
            {
                string value = HttpContext.Current.Request.Form[key];
                sbuilder.Append(string.Format("{0}={1},", key, value));
            }
            return sbuilder.ToString().Substring(0, sbuilder.ToString().Length - 1);
        }

        private static string getRequestQueryString()
        {
            if ((HttpContext.Current == null) || (HttpContext.Current.Request == null) || (HttpContext.Current.Request.QueryString.Count <= 0))
                return "";
                
            StringBuilder sbuilder = new StringBuilder();
            foreach (string key in HttpContext.Current.Request.QueryString.AllKeys)
            {
                string value = HttpContext.Current.Request.QueryString[key];
                sbuilder.Append(string.Format("{0}={1},", key, value));
            }
            return sbuilder.ToString().Substring(0, sbuilder.ToString().Length - 1);
        }

        private static string getCurrentUid()
        {
            //UserInfo user= Utility.GetUserInfo();
            //return user.ID; 
            return "3380";
        }
        
    #endregion Private Info
    }
}