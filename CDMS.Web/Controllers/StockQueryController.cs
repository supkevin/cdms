using CDMS.Model.ViewModel;
using CDMS.Service;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace CDMS.Web.Controllers
{
    public class StockQueryController : BaseController
    {
        private readonly IStockQueryService _StockQueryService;
        private readonly IProductComplexService _ProductComplexService;
        private readonly IGlobalService _GlobalService;

        public StockQueryController(
                IStockQueryService StockQueryService,
                IProductComplexService productComplexService,
                IGlobalService globalService
               )
        {
            this._StockQueryService = StockQueryService;
            this._ProductComplexService = productComplexService;
            this._GlobalService = globalService;
        }

        public ActionResult Index()
        {
            InitViewBag(null);
            return View();
        }

        [HttpPost]
        public ActionResult _List(
            string start, string finish, string txt, string productKind,
            string orderby = "ProductID", string sort = "desc", int page = 1)
        {
            InitViewBag(null);
                                    
            ViewBag.p = page < 1 ? 1 : page;

            ViewBag.start = start;
            ViewBag.finish = finish;
            ViewBag.productKind = productKind;
            ViewBag.txt = txt;
            ViewBag.orderby = orderby;
            ViewBag.sort = sort;
            
            
            string sql = " 1 = 1 ";
            List<object> obj = new List<object> { start, finish, txt };

            if (!string.IsNullOrEmpty(start))
                sql += " && (ProductID >= @0)";

            if (!string.IsNullOrEmpty(finish))
                sql += " && (ProductID <= @1)";

            if (!string.IsNullOrEmpty(txt))
                sql += " && (ProductName.Contains(@2))";

            var query = 
                this._StockQueryService.GetAll()
                .Where(sql, obj.ToArray())
                .OrderBy($"{orderby} {sort}");
                        
            if (!string.IsNullOrEmpty(productKind))
            {
                var kind = productKind.Split(',');

                // IQuerable不能用string[].Contains用這種方式
                query = query.AsEnumerable()
                        .Where(x =>
                            (
                             productKind == "" || kind.Contains(x.KindID)
                            )
                        ).AsQueryable();
            }
                                    
            return View("_List", query.ToPagedList(page, PageSize));            
        }

        public ActionResult Show(string id,
             string orderby = "ProductID", string sort = "desc", int page = 1)
        {
            InitViewBag(null);
            var product = this._StockQueryService.GetAll().Where(x => x.ProductID == id).SingleOrDefault();

            if (null == product)
            {
                TempData["Message"] = $"產品編號錯誤，產品編號:{id}！";
                return View("Error");
            }

            ViewBag.ProductID = product.ProductID;
            ViewBag.ProductName = product.ProductName;

            var query = this._StockQueryService.GetAlternativeList(id);
            query = query.OrderBy($"{ orderby } {sort}");

            return View("Show", query.ToList());
        }

        private void InitViewBag(StockQueryViewModel info)
        {
            // 產品
            ViewBag.ProductList =
                new SelectList(this._GlobalService.GetProductList(), "Key", "Value", null);

            // 使用者
            ViewBag.UserList =
                new SelectList(this._GlobalService.GetUserList(), "Key", "Value", null);

            // 產品類別
            var productKindList = this._GlobalService.GetProductKindList();
            ViewBag.ProductKindList = new SelectList(productKindList, "Value", "Text", null);

            ViewBag.MultiWareHouseList =
             new SelectList(this._GlobalService.GetWarehouseList(), "Key", "Value", null);
        }
    }
}