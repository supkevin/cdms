using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using CDMS.Model;

namespace CDMS.Service
{
    public class GlobalSettings
    {
        private static MemoryCache _Cache = MemoryCache.Default;
        private static int _CacheDuration = 600; //second 

        public enum ImageType
        {
            Before = 0,
            After
        }

        public static string DATE_FORMAT = "yyyy-MM-dd";

        public static string KEY_DATE_FORMAT = "yyMMdd";

        public static int CurrentCountry = 1;  // 台灣

        public static string IT = "2200";  // 資訊部

        public static string[] Operate = new string[] { "4", "5" };  // 資訊部

        public static bool IsOperateDepartment(string department)
        {
            return GlobalSettings.Operate.Contains(department.Substring(0, 1));
        }

        // 單號流水號長度       
        public static int SequenceLength = 3;

        // 系統自動提醒消息類別
        public static string Notification = "999";

        // 自動提醒保留天數
        public static int PreserveDays = 5;

        public static string[] 
            Documents_Need_To_Delete = 
            {
                DealItem.Purchase.Text,
                DealItem.Sales.Text,
                DealItem.PurchaseInvoice.Text,
                DealItem.SalesInvoice.Text
            };
    }
}
