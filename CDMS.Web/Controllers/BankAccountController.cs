using CDMS.Language;
using CDMS.Model;
using CDMS.Model.ViewModel;
using CDMS.Service;
using CDMS.Web;
using CDMS.Web.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using CDMS.Web.ActionFilter;
using System.IO;
using System.Text;
using CDMS.Web.Utility;
using AutoMapper;

namespace CDMS.Web.Controllers
{
    public class BankAccountController : BaseController
    {
        private readonly IBankAccountService _BankAccountService;
                
        public BankAccountController(IBankAccountService bankAccountService)
        {
            this._BankAccountService = bankAccountService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult _List(string txt, string orderby, string sort, int page = 1)
        {
            #region 設定頁碼 + 傳前端資料(ViewBag)
            int CurrentPage = page < 1 ? 1 : page;

            ViewBag.p = CurrentPage;
            ViewBag.txt = txt == null ? "" : txt;
            ViewBag.orderby = sort == null ? "" : orderby;
            ViewBag.sort = sort == null ? "" : sort;
            #endregion

            #region 組出SQL + 產生資料
            string Sql = " 1 = 1 ";
            List<object> obj = new List<object> { txt };

            if (!string.IsNullOrEmpty(txt))
            {
                Sql += " && (BankID.Contains(@0) || BankName.Contains(@0) ";
                Sql += " || AccountID.Contains(@0)|| AccountName.Contains(@0) || Remarks.Contains(@0))";
            }

            var query = this._BankAccountService.GetAll().Where(Sql, obj.ToArray());            

            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sort))
                query = query.OrderBy(orderby + " " + sort);

            #endregion

            #region 回傳
            return View("_List", query.ToPagedList(page, PageSize));
            #endregion
        }

        public ActionResult Create()
        {
             InitViewBag(null);
            return View();
        }

        private void InitViewBag(BankAccount info)
        {
            //ViewBag.YseNoList =
            //   new SelectList(YesNo.GetAll(), "value", "Text", info?.Activate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BankAccountViewModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                #region 驗證Model
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }
                #endregion

                var info = Mapper.Map<BankAccount>(model);

                if (this._BankAccountService.IsDataExists(info))
                {
                    ModelState.AddModelError("BankID", $"{"BankID".ToLocalized()}:{info.BankID} 已經存在！");
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }

                #region 前端資料變後端用資料ViewModel時用

                #endregion

                #region Service資料庫
                this._BankAccountService.Create(info);
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

        public ActionResult Edit(long id)
        {
            try
            {
                #region 驗證
                //if (string.IsNullOrEmpty(id))
                //    return View("Error");
                #endregion

                #region 取資料
                var query = this._BankAccountService.Get(id);
                #endregion

                #region ViewBag
                InitViewBag(query);
                #endregion

                #region 顯示特殊處理

                #endregion

                #region 下拉選單

                #endregion
                 
                #region 回傳
                return View(Mapper.Map<BankAccountViewModel>(query));
                #endregion
            }
            catch (Exception ex)
            {
                #region 有錯誤時錯誤訊息
                TempData["Message"] = ex.Message;
                return View("Error");
                #endregion
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "Edit")]
        public ActionResult Edit(BankAccountViewModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                #region 驗證Model
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }
                #endregion

                #region 前端資料變後端用資料ViewModel時用

                var info = Mapper.Map<BankAccount>(model);

                #endregion

                #region Service資料庫
                this._BankAccountService.Update(info);
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

        public ActionResult Delete(long id)
        {
            try
            {
                #region 驗證
                //if (string.IsNullOrEmpty(id))
                //    return View("Error");
                #endregion

                #region 取資料
                var query = this._BankAccountService.Get(id);
                #endregion

                #region ViewBag

                #endregion

                #region 顯示特殊處理

                #endregion

                #region 下拉選單

                #endregion

                #region 回傳
                return View(query);
                #endregion
            }
            catch (Exception ex)
            {
                #region 有錯誤時錯誤訊息
                TempData["Message"] = ex.Message;
                return View("Error");
                #endregion
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "Delete")]
        public ActionResult DeleteConfirmed(BankAccount model)
        {
            ResultModel result = new ResultModel();
            try
            {             
                #region Service資料庫
                if (this._BankAccountService.IsUsed(model))
                {
                    result.Message = "MessageChaneDelete2UpdateComplete".ToLocalized();

                    model.Activate = YesNo.No.Value;
                    this._BankAccountService.Update(model);
                }
                else
                {
                    this._BankAccountService.Delete(model);
                }
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