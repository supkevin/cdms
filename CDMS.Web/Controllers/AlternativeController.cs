using CDMS.Language;
using CDMS.Model;
using CDMS.Model.ViewModel;
using CDMS.Service;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Linq;
using AutoMapper;

namespace CDMS.Web.Controllers
{

    public class AlternativeController : BaseController
    {
        private readonly IAlternativeService _AlternativeService;
        private readonly IProductService _ProductService;
        private readonly IProductComplexService _ProductComplexService;
        private readonly IGlobalService _GlobalService;

        public AlternativeController(
            IAlternativeService alternativeService,
            IProductService productService,
            IProductComplexService productComplexService,
            IGlobalService globalService)
        {
            this._AlternativeService = alternativeService;

            this._ProductService = productService;
            this._ProductComplexService = productComplexService;
            this._GlobalService = globalService;
        }


        public ActionResult Index(string id)
        {
            var info = this._ProductComplexService.Get(id);
            return View(info);
        }

        private void InitViewBag(Product info)
        {
            // 公司
            ViewBag.ProductList =
              new SelectList(this._GlobalService.GetProductList(), "Value", "Display");
        }

        [HttpGet]
        public ActionResult _Item(string id)
        {
            InitViewBag(null);

            var info = new AlternativeViewModel()
            {
                SeqNo = 0,
                ProductID = id,
                IsDirty = true
            };

            return PartialView("_Item", info);
        }

        [HttpPost]
        public ActionResult _List(
            string start, string finish, string txt,
            string orderby, string sort, int page = 1)
        {
            InitViewBag(null);

            #region 設定頁碼 + 傳前端資料(ViewBag)
            int CurrentPage = page < 1 ? 1 : page;

            ViewBag.p = CurrentPage;
            ViewBag.start = start == null ? "" : start;
            ViewBag.finish = finish == null ? "" : finish;

            ViewBag.txt = txt == null ? "" : txt;
            ViewBag.orderby = sort == null ? "" : orderby;
            ViewBag.sort = sort == null ? "" : sort;
            #endregion

            #region 組出SQL + 產生資料
            string Sql = " 1 = 1 ";
            List<object> obj = new List<object> { start , finish , txt };

            if (!string.IsNullOrEmpty(start))
                Sql += " && (ProductID >= @0)";

            if (!string.IsNullOrEmpty(finish))
                Sql += " && (ProductID <= @1)";

            if (!string.IsNullOrEmpty(txt))
                Sql += " && (ProductID.Contains(@2) || ProductName.Contains(@2) || Remarks.Contains(@2))";

            var query = this._AlternativeService.GetListView().Where(Sql, obj.ToArray());

            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sort))
                query = query.OrderBy(orderby + " " + sort);

            #endregion

            #region 回傳
            return View("_List", query.ToPagedList(page, PageSize));
            #endregion
        }

        public ActionResult Edit(string id)
        {
            #region 驗證
            if (string.IsNullOrEmpty(id))
                return View("Error");
            #endregion

            var info = this._ProductComplexService.Get(id);
            return View(info);
        }

        [HttpPost, ActionName("RemoveChild")]
        public ActionResult RemoveChild(int id)
        {
            ResultModel result = new ResultModel();

            try
            {
                #region Service資料庫
                if (id != 0)
                {
                    Alternative delete = new Alternative()
                    {
                        SeqNo = id,
                    };

                    this._AlternativeService.Delete(delete);
                }
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.Message = "MessageComplete".ToLocalized();
                #endregion
            }
            catch (Exception ex)
            {
                #region 有錯誤時錯誤訊息
                result.Status = false;
                result.Message = ex.Message.ToString();
                #endregion
            }
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductComplex info)
        {
            ResultModel result = new ResultModel();

            try
            {
                #region 驗證Model
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }

                var query =
                    info.ChildList
                    .GroupBy(x => x.AlternativeID)
                    .ToDictionary(x => x.Key, x => x.Count())
                    .Where(x => x.Value > 1)
                    .ToList();

                CDMS.Web.Common.DuplicateValidator validator = new CDMS.Web.Common.DuplicateValidator(query);

                if (validator.Message.Count > 0)
                {
                    foreach (var s in validator.Message)
                    {
                        ModelState.AddModelError("AlternativeID", s);
                    }
                }

                var query2 = info.ChildList
                    .Where(x => x.AlternativeID == info.Product.ProductID)
                    .ToList()
                    .Count;

                if (query2 > 0)
                {
                    ModelState.AddModelError("AlternativeID", "替代品不可與產品相同<br/>");
                }

                if (!ModelState.IsValid)
                {
                    string message = ModelStateErrorClass.FormatToString(ModelState);

                    result.Status = false;
                    result.Message = message;

                    return Json(result);
                }
                #endregion


                // 有修改的資料
                foreach (var item in info.ChildList.Where(x => x.IsDirty == true))
                {
                    Alternative target = Mapper.Map<Alternative>(item);

                    if (target.SeqNo == 0)
                    {
                        this._AlternativeService.Create(target);
                    }
                    else
                    {
                        this._AlternativeService.Update(target);
                    }
                }
                #region 訊息頁面設定
                result.Status = true;
                result.Url = Url.Action("Index", "Alternative", new { id = info.Product.ProductID });
                result.Message = "MessageComplete".ToLocalized();
                #endregion
            }
            catch (Exception ex)
            {
                #region 有錯誤時錯誤訊息
                result.Status = false;
                result.Message = ex.Message.ToString();
                #endregion
            }
            return Json(result);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            ResultModel result = new ResultModel();
            try
            {
                #region 驗證Model
                if (!id.HasValue)
                    return View("Error");
                #endregion

                if (id.Value != 0)
                {
                    Alternative delete = new Alternative()
                    {
                        SeqNo = (int)id.Value,
                    };

                    this._AlternativeService.Delete(delete);
                }

                #region 訊息頁面設定
                result.Status = true;
                result.Message = "MessageComplete".ToLocalized();
                #endregion
            }
            catch (Exception ex)
            {
                #region 有錯誤時錯誤訊息
                result.Status = false;
                result.Message = ex.Message.ToString();
                #endregion
            }
            return Json(result);
        }
    }
}