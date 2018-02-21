using CDMS.Model;
using CDMS.Model.ViewModel;
using CDMS.Service;
using ClosedXML.Excel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CDMS.Language;

namespace CDMS.Web.Controllers
{
    public class ProductSalesRankingController : BaseController
    {
        private readonly IProductSalesRankingService _Service;
        private readonly IGlobalService _GlobalService;

        // 排列方式
        private HashSet<MyTextValue> _Sort = new HashSet<MyTextValue>();

        // 產品類別 
        private HashSet<MyTextValue> _ProductKind = new HashSet<MyTextValue>();

        public ProductSalesRankingController(
                IProductSalesRankingService productSalesRankingService,
                IGlobalService globalService
               )
        {
            this._Service = productSalesRankingService;
            this._GlobalService = globalService;

            _Sort.Add(new MyTextValue { Text = "依銷售量", Value = "TotalQty", Disabled = false });
            _Sort.Add(new MyTextValue { Text = "依銷售總金額", Value = "TotalAmount", Disabled = false });
            _Sort.Add(new MyTextValue { Text = "依毛利率(%)", Value = "GrossProfitMargin", Disabled = false });

            _ProductKind = this._GlobalService.GetProductKindList();
        }

        public ActionResult Index()
        {
            InitViewBag(null);
            return View();
        }

        #region _List
        [HttpPost]
        public ActionResult _List(
            DateTime? dateStart, DateTime? dateFinish, 
            string start="0", string finish="z",
            string productKind="", 
            string orderby = "TotalQty", string sort = "desc", int page = 1)
        {            
            ViewBag.p = page < 1 ? 1 : page;

            ViewBag.dateStart = dateStart;
            ViewBag.dateFinish = dateFinish;
            ViewBag.start = start;
            ViewBag.finish = finish;
            ViewBag.productKind = productKind;            
            ViewBag.orderby = orderby;
            ViewBag.sort = sort;
           
            var query = GeQuery(dateStart, dateFinish, start, finish, productKind, orderby, sort);

            return View("_List", query.ToPagedList(page, PageSize));
        }
        #endregion _List

        private IQueryable<ProductSalesRankingViewModel>
            GeQuery(DateTime? dateStart, DateTime? dateFinish,
           string start = "0", string finish = "z",
           string productKind = "",
           string orderby = "TotalQty", string sort = "desc")
        {

            if (!dateStart.HasValue) dateStart = DateTime.MinValue;
            if (!dateFinish.HasValue) dateFinish = DateTime.MaxValue;

            var sql = " 1 = 1 ";
            List<object> obj = new List<object> { start, finish };

            if (!string.IsNullOrEmpty(start)) sql += " && ProductID >=@0 ";
            if (!string.IsNullOrEmpty(finish)) sql += " && ProductID <=@1 ";

            var query = this._Service.GetAll(dateStart.Value, dateFinish.Value)
                        .Where(sql, obj.ToArray())
                        .OrderBy($"{orderby} {sort}");

            query = query.AsEnumerable()
                       .Where(x =>
                           (productKind == "" || productKind.Split(',').ToArray().Contains(x.KindID))                           
                       ).AsQueryable();

            return query;
        }
      
        private string GetOrderByText(string orderby)
        {
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
                    $"{dateStart.Value.ToString(GlobalSettings.DATE_FORMAT)} 至 {dateStart.Value.ToString(GlobalSettings.DATE_FORMAT)}";
            return result;
        }

        private string GetProductKind(string productKind)
        {
            var result = "全部";
            if (!string.IsNullOrEmpty(productKind))
            {
                var query = _ProductKind.Where(x => productKind.Split(',').ToArray().Contains(x.Value))
                    .Select(x => x.Text)
                    .ToArray();

                result = string.Join(";", query);
            }

            return result;
        }

        private string GetProductRange(string start, string finish)
        {
            var result = "";
            result = (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(finish)) ? $"{start}~{finish}" : "";
            return result;
        }

        [HttpPost]
        public JsonResult ExportExcel(
         DateTime? dateStart, DateTime? dateFinish,
           string start = "0", string finish = "z",
           string productKind = "",
           string orderby = "TotalQty", string sort = "desc", int page = 1)
        {           
            ResultModel result = new ResultModel();

            //建立Excel         
            string template =
                System.Web.Hosting.HostingEnvironment.MapPath("~/Templates/產品銷售排行統計表.xlsx");

            var exportFileName = $"{ Guid.NewGuid().ToString()}.xlsx";
            var fullPath = Path.Combine(Server.MapPath("~/Temp"), exportFileName);

            XLWorkbook workbook = new ClosedXML.Excel.XLWorkbook(template);

            int rowIndex = 7;
            int rowIndexForCopy = 7;
            int columnIndex = 1;
            int rank = 0;

            try
            {
                var infos = GeQuery(dateStart, dateFinish, start, finish, productKind, orderby, sort);
                var sheet = workbook.Worksheets.First();

                sheet.Cell(2, 2).Value = GetOrderByText(orderby); //排列方式
                sheet.Cell(3, 2).Value = GetDateRange(dateStart, dateFinish); //銷售日期
                sheet.Cell(4, 2).Value = GetProductKind(productKind); //產品類別
                sheet.Cell(5, 2).Value = GetProductRange(start, finish); //產品編號  

                foreach (var item in infos)
                {
                    sheet.Cell(rowIndex, columnIndex++).Value = ++rank;
                    sheet.Cell(rowIndex, columnIndex++).Value = $"'{ item.ProductID}";
                    sheet.Cell(rowIndex, columnIndex++).Value = item.ProductName;
                    sheet.Cell(rowIndex, columnIndex++).Value = item.TotalQty;
                    sheet.Cell(rowIndex, columnIndex++).Value = item.TotalAmount;                    
                    sheet.Cell(rowIndex, columnIndex++).Value = item.TotalProfit;
                    sheet.Cell(rowIndex, columnIndex++).Value = item.GrossProfitMargin;

                    rowIndex++;
                    columnIndex = 1;
                    sheet.Row(rowIndexForCopy).CopyTo(sheet.Row(rowIndex));
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

        private void InitViewBag(ProductSalesRankingViewModel info)
        {
            // 產品類別            
            ViewBag.ProductKindList =
              new SelectList(_ProductKind, "Value", "Text");

            // 排列方式          
            ViewBag.OrderByList = new SelectList(_Sort, "Value", "Text", null);            
        }
    }
}