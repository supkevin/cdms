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
    public class InventoryValueStatisticsController : BaseController
    {
        private readonly IStockTrackService _StockTrackService;
        private readonly IGlobalService _GlobalService;

        public InventoryValueStatisticsController(
                IStockTrackService StockTrackService,
                IGlobalService globalService
               )
        {
            this._StockTrackService = StockTrackService;
            this._GlobalService = globalService;
        }

        public ActionResult Index()
        {
            InitViewBag(null);
            return View();
        }

        [HttpPost]
        public ActionResult _List(
            DateTime? dateStart, DateTime? dateFinish, string start, string finish,
            string productKind, string wareHouse,
            string orderby = "ProductID", string sort = "desc", int page = 1)
        {            
            ViewBag.p = page < 1 ? 1 : page;

            ViewBag.dateStart = dateStart;
            ViewBag.dateFinish = dateFinish;
            ViewBag.start = start;
            ViewBag.finish = finish;
            ViewBag.productKind = productKind;
            ViewBag.wareHouse = wareHouse;
            ViewBag.orderby = orderby;
            ViewBag.sort = sort;

            var query = GeQuery(dateStart, dateFinish, start, finish, productKind, wareHouse);

            return View("_List", query.ToPagedList(page, PageSize));
        }

        private IQueryable<StockTrackViewModel>
           GeQuery(DateTime? dateStart, DateTime? dateFinish, string start, string finish,
            string productKind, string wareHouse,
            string orderby = "ProductID", string sort = "desc")
        {           
            var query =
                this._StockTrackService.GetSummary(dateStart, dateFinish, start, finish);
            query = query.OrderBy($"{orderby} {sort}");                            

            return query;
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
           string productKind="", string wareHouse="",
           string orderby = "ProductID", string sort = "desc", int page = 1)
        {
            ResultModel result = new ResultModel();

            //建立Excel         
            string template =
                System.Web.Hosting.HostingEnvironment.MapPath("~/Templates/庫存總值統計表.xlsx");

            var exportFileName = $"{ Guid.NewGuid().ToString()}.xlsx";
            var fullPath = Path.Combine(Server.MapPath("~/Temp"), exportFileName);

            XLWorkbook workbook = new ClosedXML.Excel.XLWorkbook(template);

            int rowIndex = 6;
            int rowIndexForCopy = 7;
            int columnIndex = 1;
            int rank = 0;
            try
            {
                var infos = GeQuery(dateStart, dateFinish, start, finish, orderby, sort);
                var sheet = workbook.Worksheets.First();

                //sheet.Cell(2, 2).Value = GetOrderByText(orderby); //排列方式
                sheet.Cell(3, 2).Value = GetDateRange(dateStart, dateFinish); //銷售日期
                sheet.Cell(4, 2).Value = GetCustomerRange(start, finish); //客戶編號  

                foreach (var item in infos)
                {
                    sheet.Cell(rowIndex, columnIndex++).Value = ++rank;
                    sheet.Cell(rowIndex, columnIndex++).Value = item.ProductID;
                    sheet.Cell(rowIndex, columnIndex++).Value = item.ProductName;
                    sheet.Cell(rowIndex, columnIndex++).Value = item.Purchase;
                    sheet.Cell(rowIndex, columnIndex++).Value = item.Sales;
                    sheet.Cell(rowIndex, columnIndex++).Value = item.Adjust;
                    sheet.Cell(rowIndex, columnIndex++).Value = item.Transfer;
                    sheet.Cell(rowIndex, columnIndex++).Value = item.Stock;

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

        private void InitViewBag(StockTrackViewModel info)
        {
            // 產品類別            
            ViewBag.ProductKindList =
              new SelectList(this._GlobalService.GetProductKindList(), "Value", "Text");

            // 排列方式
            HashSet<MyTextValue> sort = new HashSet<MyTextValue>();

            sort.Add(new MyTextValue { Text = "銷售量", Value = "TotalQty", Disabled = false });
            sort.Add(new MyTextValue { Text = "銷售總金額", Value = "TotalAmount", Disabled = false });
            sort.Add(new MyTextValue { Text = "毛利率(%)", Value = "GrossProfitMargin", Disabled = false });

            ViewBag.SortOrderList = new SelectList(sort, "Value", "Text", null);

            // 倉庫
            ViewBag.WareHouseList =
                new SelectList(this._GlobalService.GetWarehouseList(), "Key", "Value");
        }
    }
}