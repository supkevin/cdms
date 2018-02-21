﻿using CDMS.Language;
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
    public class CodeController : BaseController
    {
        private readonly ICodeService _CodeService;

        public CodeController(ICodeService codeService)
        {
            this._CodeService = codeService;
        }

        public ActionResult Index()
        {
            InitViewBag(null);
            return View();
        }

        #region _List        
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
                Sql += " && (CodeType == @0)";

            var query = this._CodeService.GetAll().Where(Sql, obj.ToArray());

            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sort))
                query = query.OrderBy(orderby + " " + sort);

            #endregion

            InitViewBag(null);

            #region 回傳
            return View("_List", query.ToPagedList(page, PageSize));
            #endregion
        }
        #endregion

        public ActionResult Create(string codeType)
        {
            Code info = new Code() { CodeType = codeType };

            InitViewBag(info);
                        
            return View(Mapper.Map<CodeViewModel>(info));
        }

        private void InitViewBag(Code info)
        {

           ViewBag.CodeTypeList =
                new SelectList(CodeType.GetAll(), "value", "Text", info?.CodeType);

            ViewBag.YseNoList =
               new SelectList(YesNo.GetAll(), "value", "Text", info?.Activate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CodeViewModel model)
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

                var info = Mapper.Map<Code>(model);

                if (this._CodeService.IsCodeExists(info))
                {
                    ModelState.AddModelError("CodeValue", $"{"CodeValue".ToLocalized()}:{info.CodeValue} 已經存在！");
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }

                #region 前端資料變後端用資料ViewModel時用

                #endregion

                #region Service資料庫
                this._CodeService.Create(info);
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
                var query = this._CodeService.Get(id);
                #endregion

                #region ViewBag
                InitViewBag(query);
                #endregion

                #region 顯示特殊處理

                #endregion

                #region 下拉選單

                #endregion

                var info = Mapper.Map<CodeViewModel>(query);

                #region 回傳
                return View(info);
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
        public ActionResult Edit(CodeViewModel model)
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

                var info = Mapper.Map<Code>(model);

                #endregion

                #region Service資料庫
                this._CodeService.Update(info);
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
                var query = this._CodeService.Get(id);
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
        public ActionResult DeleteConfirmed(Code model)
        {
            ResultModel result = new ResultModel();
            try
            {
                #region Service資料庫
                if (this._CodeService.IsUsed(model))
                {
                    result.Message = "MessageChaneDelete2UpdateComplete".ToLocalized();

                    model.Activate = YesNo.No.Value;
                    this._CodeService.Update(model);
                }
                else
                {
                    this._CodeService.Delete(model);
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