using CDMS.Model.ViewModel;
using CDMS.Model;
using CDMS.Service;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System;
using ClosedXML.Excel;
using CDMS.Language;
using System.IO;
using CDMS.Web.ActionFilter;

namespace CDMS.Web.Controllers
{
    public class CustomerSalesRankingController : BaseController
    {
        private readonly ICustomerSalesRankingService _Service;
        private readonly IGlobalService _GlobalService;
        private HashSet<MyTextValue> _Sort = new HashSet<MyTextValue>();

        public CustomerSalesRankingController(
                ICustomerSalesRankingService CustomerSalesRankingService,
                IGlobalService globalService
               )
        {
            this._Service = CustomerSalesRankingService;
            this._GlobalService = globalService;

            _Sort.Add(new MyTextValue { Text = "銷售總金額", Value = "TotalAmount", Disabled = false });
            _Sort.Add(new MyTextValue { Text = "依毛利率", Value = "GrossProfitMargin", Disabled = false });
            _Sort.Add(new MyTextValue { Text = "依客戶編號", Value = "CustomerID", Disabled = false });
        }

        public ActionResult Index()
        {
            InitViewBag(null);
            return View();
        }

        [HttpPost]
        public ActionResult _List(
            DateTime? dateStart, DateTime? dateFinish, 
            string start="0", string finish="z",            
            string orderby = "TotalAmount", string sort = "desc", int page = 1)
        {            
            ViewBag.p = page < 1 ? 1 : page;

            ViewBag.dateStart = dateStart;
            ViewBag.dateFinish = dateFinish;
            ViewBag.start = start;
            ViewBag.finish = finish;
            ViewBag.orderby = orderby;
            ViewBag.sort = sort;

            if (!dateStart.HasValue) dateStart = DateTime.MinValue;
            if (!dateFinish.HasValue) dateFinish = DateTime.MaxValue;

            var sql = " 1 = 1 ";
            List<object> obj = new List<object> { start, finish };

            if (!string.IsNullOrEmpty(start)) sql += " && CustomerID >=@0 ";
            if (!string.IsNullOrEmpty(finish)) sql += " && CustomerID <=@1 ";

            var query = GeQuery(dateStart, dateFinish, start, finish, orderby, sort);

            return View("_List", query.ToPagedList(page, PageSize));
        }
      
        #region InitViewBag
        private void InitViewBag(CustomerSalesRankingViewModel info)
        {
            // 排列方式          
            ViewBag.OrderByList = new SelectList(_Sort, "Value", "Text", null);            
        }
        #endregion


        private IQueryable<CustomerSalesRankingViewModel>
            GeQuery(
            DateTime? dateStart, DateTime? dateFinish,
            string start = "0", string finish = "z",
            string orderby = "TotalAmount", string sort = "desc")
        {

            if (!dateStart.HasValue) dateStart = DateTime.MinValue;
            if (!dateFinish.HasValue) dateFinish = DateTime.MaxValue;

            var sql = " 1 = 1 ";
            List<object> obj = new List<object> { start, finish };

            if (!string.IsNullOrEmpty(start)) sql += " && CustomerID >=@0 ";
            if (!string.IsNullOrEmpty(finish)) sql += " && CustomerID <=@1 ";

            var query = this._Service.GetAll(dateStart.Value, dateFinish.Value)
                        .Where(sql, obj.ToArray())
                        .OrderBy($"{orderby} {sort}");

            return query;
        }

        private string GetOrderByText(string orderby) {
            var result = "";
            var query = _Sort.Where(x => x.Value == orderby).Select(x => x).SingleOrDefault();

            if (null != query)
            {
                result = query.Text;
            }
            return result; 
        }

        private string GetDateRange(DateTime? dateStart, DateTime? dateFinish)
        {
            var result = "";
            if (dateStart.HasValue && dateFinish.HasValue)
                result =
                    $"{dateStart.Value.ToString(GlobalSettings.DATE_FORMAT)}~{dateStart.Value.ToString(GlobalSettings.DATE_FORMAT)}";         
            return result;
        }

        private string GetCustomerRange(string start, string finish)
        {
            var result = "";
            result = (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(finish)) ? $"{start}~{finish}" : ""; 
            return result;
        }

        [HttpPost]
        public JsonResult ExportExcel(
            DateTime? dateStart, DateTime? dateFinish,
            string start = "0", string finish = "z",
            string orderby = "TotalAmount", string sort = "desc", int page = 1)
        {
            //https://www.codeproject.com/Tips/1156485/How-to-Create-and-Download-File-with-Ajax-in-ASP-N

            ResultModel result = new ResultModel();

            //建立Excel         
            string template =
                System.Web.Hosting.HostingEnvironment.MapPath("~/Templates/客戶銷售排行統計表.xlsx");

            var exportFileName = $"{ Guid.NewGuid().ToString()}.xlsx";
            var fullPath = Path.Combine(Server.MapPath("~/Temp"), exportFileName);

            XLWorkbook workbook = new ClosedXML.Excel.XLWorkbook(template);
          
            int rowIndex = 6;
            int columnIndex = 1;
            int rank = 0;
            try
            {
                var infos = GeQuery(dateStart, dateFinish, start, finish, orderby, sort);
                var sheet = workbook.Worksheets.First();

                sheet.Cell(2, 2).Value = GetOrderByText(orderby); //排列方式
                sheet.Cell(3, 2).Value = GetDateRange(dateStart, dateFinish); //銷售日期
                sheet.Cell(4, 2).Value = GetCustomerRange(start, finish); //客戶編號  

                foreach (var item in infos)
                {                                 
                    sheet.Cell(rowIndex, columnIndex++).Value = ++rank;
                    sheet.Cell(rowIndex, columnIndex++).Value = item.CustomerID;
                    sheet.Cell(rowIndex, columnIndex++).Value = item.CustomerName;                    
                    sheet.Cell(rowIndex, columnIndex++).Value = item.TotalAmount;
                    sheet.Cell(rowIndex, columnIndex++).Value = item.TotalSalesCost;
                    sheet.Cell(rowIndex, columnIndex++).Value = item.TotalProfit;
                    sheet.Cell(rowIndex, columnIndex++).Value = item.GrossProfitMargin;

                    rowIndex++;
                    columnIndex = 1;
                    sheet.Row(6).CopyTo(sheet.Row(rowIndex));
                }

                sheet.Row(rowIndex).Delete();

                workbook.SaveAs(fullPath);

                result.Status = true;
                result.CloseWindow = false;
                result.Url = exportFileName;
                result.Message = "MessageComplete".ToLocalized();
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message.ToString();
            }
            return Json(result);
        }
    }
}