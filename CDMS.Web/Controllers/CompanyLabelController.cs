﻿using CDMS.Model.ViewModel;
using CDMS.Model;
using CDMS.Service;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System;

namespace CDMS.Web.Controllers
{
    public class CompanyLabelController : BaseController
    {
        private readonly IStockTrackService _StockTrackService;
        private readonly IGlobalService _GlobalService;

        public CompanyLabelController(
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

            var query = 
                this._StockTrackService.GetSummary(dateStart, dateFinish, start, finish);

            query = query.OrderBy($"{orderby} {sort}");

            return View("_List", query.ToPagedList(page, PageSize));
        }

        [HttpPost]
        public ActionResult _TrackList(
           DateTime? dateStart, DateTime? dateFinish, string product,
           string orderby = "ChangeDate", string sort = "desc", int page = 1)
        {
            var query =
                this._StockTrackService.GetDetails(dateStart, dateFinish, product);

            query = query.OrderBy($"{orderby} {sort}");

            return View("_TrackList", query.ToList());
        }

        private void InitViewBag(StockTrackViewModel info)
        {
            ViewBag.CompanyKindList =
              new SelectList(CompanyType.GetAll(), "Value", "Text");

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