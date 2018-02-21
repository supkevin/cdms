using CDMS.Model.ViewModel;
using CDMS.Service;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace CDMS.Web.Controllers
{
    public class StockShortageQueryController : BaseController
    {
        private readonly IStockQueryService _StockQueryService;
        private readonly IGlobalService _GlobalService;

        public StockShortageQueryController(
                IStockQueryService StockQueryService,
                IGlobalService globalService
               )
        {
            this._StockQueryService = StockQueryService;
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

            #region 設定頁碼 + 傳前端資料(ViewBag)
            ViewBag.p = page < 1 ? 1 : page;

            ViewBag.start = start;
            ViewBag.finish = finish;
            ViewBag.productKind = productKind;
            ViewBag.txt = txt;
            ViewBag.orderby = orderby;
            ViewBag.sort = sort;
            #endregion

            string sql = " 1 = 1 ";
            List<object> obj = new List<object> { start, finish, txt };

            if (!string.IsNullOrEmpty(start))
                sql += " && (ProductID >= @0)";

            if (!string.IsNullOrEmpty(finish))
                sql += " && (ProductID <= @1)";

            if (!string.IsNullOrEmpty(txt))
                sql += " && (ProductName.Contains(@2))";

            var query = this._StockQueryService.GetAll()
                        .Where(sql, obj.ToArray())
                        .OrderBy($"{ orderby } {sort}");

            if (!string.IsNullOrEmpty(productKind))
            {
                var kind = productKind.Split(',');

                query = query.AsEnumerable()
                    .Where(x =>kind.Contains(x.KindID))
                    .AsQueryable();
            }
            
            #region 回傳
            return View("_List", query.ToPagedList(page, PageSize));
            #endregion
        }
  
        private void InitViewBag(StockQueryViewModel info)
        {
            // 產品類別            
            ViewBag.ProductKindList = 
                new SelectList(this._GlobalService.GetProductKindList(), "Value", "Text", null);

        }
    }
}