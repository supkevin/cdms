using CDMS.Language;
using CDMS.Model;
using CDMS.Service;
using CDMS.Web.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace CDMS.Web.Controllers
{
    public class OverSeaController : BaseController
    {
        private readonly IOverSeaService _overseaService;
        private readonly IOverTypeService _overtypeService;
        private readonly ICountryService _countryService;

        public OverSeaController(IOverSeaService overseaService, IOverTypeService overtypeService, ICountryService countryService)
        {
            this._overseaService = overseaService;
            this._overtypeService = overtypeService;
            this._countryService = countryService;
        }

        public ActionResult Index()
        {
            #region 下拉選單
            ViewBag.OverTypeList = new SelectList(this._overtypeService.GetAll().OrderBy(x => x.NQ_Sort), "ID_OverType", "CX_OverType", null);
            ViewBag.CountryList = new SelectList(this._countryService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Country", "CX_Country", null);
            #endregion

            return View();
        }

        [HttpPost]
        public ActionResult _List(string txt, string orderby, string sort, int? id_overtype, int? id_country, string goesfrom, string goesto, int page = 1)
        {
            #region 設定頁碼 + 傳前端資料(ViewBag)
            int CurrentPage = page < 1 ? 1 : page;

            ViewBag.PageSize = PageSize;
            ViewBag.p = CurrentPage;
            ViewBag.txt = txt == null ? "" : txt;
            ViewBag.orderby = sort == null ? "" : orderby;
            ViewBag.sort = sort == null ? "" : sort;
            ViewBag.id_overtype = id_overtype == null ? 0 : id_overtype;//給0不做
            ViewBag.id_country = id_country == null ? 0 : id_country;//給0不做
            ViewBag.goesfrom = goesfrom == null ? "" : goesfrom;
            ViewBag.goesto = goesto == null ? "" : goesto;
            #endregion

            #region 組出SQL + 產生資料
            string Sql = " 1 = 1 ";
            List<object> obj = new List<object> { txt, id_country, id_overtype, goesfrom, goesto };

            if (!string.IsNullOrEmpty(txt))
                Sql += " && (CX_PID.Contains(@0) || CX_Name.Contains(@0))";

            if (id_country.HasValue && id_country != 0)
                Sql += " && ID_Country = @1 ";

            if (id_overtype.HasValue && id_overtype != 0)
                Sql += " && ID_Country = @2 ";

            if (!string.IsNullOrEmpty(goesfrom))
                Sql += " && CX_From_Date >= @3 ";

            if (!string.IsNullOrEmpty(goesto))
                Sql += " && CX_From_Date <= @4 ";

            var query = this._overseaService.GetAll().Where(Sql, obj.ToArray());

            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sort))
                query = query.OrderBy(orderby + " " + sort);

            #endregion

            #region 回傳
            return View("_List", query.ToPagedList(page, PageSize));
            #endregion
        }

        public ActionResult His()
        {
            #region 下拉選單
            ViewBag.OverTypeList = new SelectList(this._overtypeService.GetAll().OrderBy(x => x.NQ_Sort), "ID_OverType", "CX_OverType", null);
            ViewBag.CountryList = new SelectList(this._countryService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Country", "CX_Country", null);
            #endregion
            return View();
        }

        [HttpPost]
        public ActionResult _HisList(string txt, string goesfrom, string goesto, string orderby, string sort, int? id_overtype, int? id_country, int page = 1)
        {
            #region 設定頁碼 + 傳前端資料(ViewBag)
            int CurrentPage = page < 1 ? 1 : page;

            ViewBag.PageSize = PageSize;
            ViewBag.p = CurrentPage;
            ViewBag.txt = txt == null ? "" : txt;
            ViewBag.orderby = sort == null ? "" : orderby;
            ViewBag.sort = sort == null ? "" : sort;
            ViewBag.id_overtype = id_overtype == null ? 0 : id_overtype;//給0不做
            ViewBag.id_country = id_country == null ? 0 : id_country;//給0不做
            ViewBag.goesfrom = goesfrom == null ? "" : goesfrom;
            ViewBag.goesto = goesto == null ? "" : goesto;
            #endregion

            #region 組出SQL + 產生資料
            string Sql = " 1 = 1 ";
            List<object> obj = new List<object> { txt, id_country, id_overtype, goesfrom, goesto };

            if (!string.IsNullOrEmpty(txt))
                Sql += " && (CX_PID.Contains(@0) || CX_Name.Contains(@0))";

            if (id_country.HasValue && id_country != 0)
                Sql += " && ID_Country = @1 ";

            if (id_overtype.HasValue && id_overtype != 0)
                Sql += " && ID_Country = @2 ";

            if (!string.IsNullOrEmpty(goesfrom))
                Sql += " && CX_From_Date >= @3 ";

            if (!string.IsNullOrEmpty(goesto))
                Sql += " && CX_From_Date <= @4 ";

            var query = this._overseaService.GetAll().Where(Sql, obj.ToArray());

            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sort))
                query = query.OrderBy(orderby + " " + sort);

            #endregion

            #region 回傳
            return View("_HisList", query.ToPagedList(page, PageSize));
            #endregion
        }

        public ActionResult Birthday()
        {
            return View();
        }

        [HttpPost]
        public ActionResult _BirthdayList(int kind)
        {
            #region 回傳的
            List<OverSea> oversealist = new List<OverSea>();
            #endregion

            #region 計算區間

            DateTime mFrom = DateTime.Now; //生日區間的日期起
            DateTime mTo = mFrom;//生日區間的日期迄
            switch (kind)
            {
                case 1:
                    mTo = mFrom.AddDays(62);//1 兩個月
                    break;
                case 2:
                    mTo = mFrom.AddDays(8);//　2一周
                    break;
            }

            string mFromS = mFrom.ToString("yyyy-MM-dd");
            string mToS = mTo.ToString("yyyy-MM-dd");
            #endregion

            #region 抓出會遇到生日的人員

            //會有生日的 且返台日其比今天後面
            var query = this._overseaService.GetAll().Where(x => x.NQ_MeetBirthday > 0 && x.CX_To_Date.CompareTo(mFromS) >= 0);//&& x.CX_From_Date.CompareTo(mFromS) <= 0 && x.CX_To_Date.CompareTo(mToS) >= 0
            #endregion

            #region 產生資料回傳

            foreach (var item in query)
            {             
                //看這段期間是生日的
                int mBirthdayCount = DateHelper.GetBetweenBirthdayCount(
                    mFrom,
                    mTo,
                    DateTime.Parse(item.CX_Birthday));

                if (mBirthdayCount > 0)
                {
                    oversealist.Add(item);
                }
            }
            return View(oversealist);
            #endregion
        }


        public ActionResult Display()
        {
            return View();
        }

        public ActionResult _DisplayList(int kind)
        {
            #region 回傳
            List<OverSea> oversealist = this.GetDisplayList(kind);

            return View(this.GetDisplayList(kind));
            #endregion
        }

        public ActionResult DisplayListExport()
        {
            #region 抓資料
            List<OverSea> list1 = this.GetDisplayList(1);
            List<OverSea> list2 = this.GetDisplayList(2);
            #endregion

            #region 產生前端資料
            List<DisplayExportViewModel> result = new List<DisplayExportViewModel>();

            int i = 1;
            foreach (var item in list1)
            {
                DisplayExportViewModel Create = new DisplayExportViewModel()
                {
                    序號 = i,
                    工號 = item.CX_PID,
                    姓名 = item.CX_Name,
                    出差國家 = item.Country.CX_Country,
                    出發日期 = item.CX_From_Date,
                    返台日期 = item.CX_To_Date,
                    種類 = item.OverType.CX_OverType,
                    單位 = item.CX_Dept_Name,
                    職稱 = item.CX_Title,
                    到職日 = item.CX_OnBoard_Date,
                    工作簽 = StringClass.GetTrueOrFalse(item.FG_IsWorkCard),
                    狀態 = "TextIsOut".ToLocalized()

                };
                result.Add(Create);
                i++;
            }

            i = 0;
            foreach (var item in list2)
            {
                DisplayExportViewModel Create = new DisplayExportViewModel()
                {
                    序號 = i,
                    工號 = item.CX_PID,
                    姓名 = item.CX_Name,
                    出差國家 = item.Country.CX_Country,
                    出發日期 = item.CX_From_Date,
                    返台日期 = item.CX_To_Date,
                    種類 = item.OverType.CX_OverType,
                    單位 = item.CX_Dept_Name,
                    職稱 = item.CX_Title,
                    到職日 = item.CX_OnBoard_Date,
                    工作簽 = StringClass.GetTrueOrFalse(item.FG_IsWorkCard),
                    狀態 = "TextIsNotGo".ToLocalized()

                };
                result.Add(Create);
                i++;
            }

            MemoryStream ms = NpoiClass.RenderListToExcel<DisplayExportViewModel>(result) as MemoryStream;
            #endregion

            #region 回傳
            return File(ms.ToArray(), "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            #endregion

        }


        public List<OverSea> GetDisplayList(int kind)
        {
            #region 回傳的
            List<OverSea> oversealist = new List<OverSea>();
            #endregion

            #region 產生資料回傳
            string mNow = DateTime.Now.ToString("yyyy-MM-dd");
            switch (kind)
            {
                case 1:
                    //正在國外的
                    oversealist = this._overseaService.GetAll().Where(x => x.CX_From_Date.CompareTo(mNow) <= 0 && x.CX_To_Date.CompareTo(mNow) >= 0).ToList();
                    break;
                case 2:
                    //尚未在國外的
                    oversealist = this._overseaService.GetAll().Where(x => x.CX_From_Date.CompareTo(mNow) > 0).ToList();
                    break;
            }
            return oversealist.OrderBy(x => x.Country.NQ_Sort).ThenBy(x => x.CX_From_Date).ToList();
            #endregion
        }


        public ActionResult NowOverSea()
        {
            #region 回傳的
            List<NowOverSeaViewModel> result = new List<NowOverSeaViewModel>();
            #endregion

            #region 產生資料回傳
            string mNow = DateTime.Now.ToString("yyyy-MM-dd");
            var query = this._overseaService.GetAll().Where(x => x.CX_From_Date.CompareTo(mNow) <= 0 && x.CX_To_Date.CompareTo(mNow) >= 0).ToList();

            var gCountry = (from u in query
                            group u by new { u.ID_Country, u.Country.CX_Country } into g
                            select new
                            {
                                ID_Country = g.Key.ID_Country,
                                CX_Country = g.Key.CX_Country
                            }).ToList();

            var gGroup = (from u in query
                          group u by new { u.CX_Dept_Name_Short } into g
                          select new
                          {
                              CX_Dept_Name_Short = g.Key.CX_Dept_Name_Short
                          }).ToList();

            foreach (var itemCountry in gCountry)
            {
                //總筆數
                int mCountryCount = query.Where(x => x.ID_Country == itemCountry.ID_Country).Count();

                foreach (var itemGroup in gGroup)
                {   //子筆數
                    var queryGroupCount = query.Where(x => x.CX_Dept_Name_Short == itemGroup.CX_Dept_Name_Short && x.ID_Country == itemCountry.ID_Country);

                    //算地區比數
                    var gGroupPlace = (from u in queryGroupCount
                                       group u by new { u.CX_Place_Remark } into g
                                       select new
                                       {
                                           CX_Place_Remark = g.Key.CX_Place_Remark
                                       }).ToList();

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    foreach (var itemGroupPlace in gGroupPlace)
                    {
                        if (!string.IsNullOrEmpty(itemGroupPlace.CX_Place_Remark))
                        {　 //地區人數
                            var queryPlaceX = queryGroupCount.Where(x => x.CX_Place_Remark == itemGroupPlace.CX_Place_Remark).ToList();
                            //工作簽人數
                            var queryPlaceXX = queryPlaceX.Where(x => x.FG_IsWorkCard == true).ToList();
                            string tempString = queryPlaceXX.Count() > 0 ? "(" + "FG_IsWorkCard".ToLocalized() + ":" + queryPlaceXX.Count.ToString() + "TextMing".ToLocalized() + ")" : "";

                            //全部文字
                            sb.AppendFormat(
                                itemGroupPlace.CX_Place_Remark + ":" + queryPlaceX.Count.ToString() + "TextMing".ToLocalized() + tempString + "\r\n");
                        }
                    }


                    if (queryGroupCount.Count() > 0)//要有子筆數才算
                    {
                        NowOverSeaViewModel newoversea = new NowOverSeaViewModel()
                        {
                            NQ_Sub_People = queryGroupCount.Count(),
                            NQ_Total_People = mCountryCount,
                            CX_Country = itemCountry.CX_Country,
                            CX_Group_Name = itemGroup.CX_Dept_Name_Short,
                            CX_Remark = sb.ToString()
                        };
                        result.Add(newoversea);
                    }
                }
            }

            return View(result);
            #endregion
        }

        public ActionResult Create()
        {
            try
            {
                #region 下拉選單
                ViewBag.OverTypeList = new SelectList(this._overtypeService.GetAll().OrderBy(x => x.NQ_Sort), "ID_OverType", "CX_OverType", null);
                ViewBag.CountryList = new SelectList(this._countryService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Country", "CX_Country", null);
                #endregion

                #region 回傳
                return View();
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
        public ActionResult Create(OverSea model)
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

                //算遇到幾天
                model.NQ_MeetBirthday = DateHelper.GetBetweenBirthdayCount(
                    DateTime.Parse(model.CX_From_Date),
                    DateTime.Parse(model.CX_To_Date),
                    DateTime.Parse(model.CX_Birthday));

                model.CX_Create = UserModel.GetUserData().StaffID;
                model.DT_Create = DateTime.Now;
                #endregion

                #region Service資料庫
                this._overseaService.Create(model);
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

        public ActionResult Edit(int? id)
        {
            try
            {
                #region 驗證
                if (!id.HasValue)
                    return View("Error");
                #endregion

                #region 取資料
                var query = this._overseaService.GetById((int)id);
                #endregion

                #region ViewBag

                #endregion

                #region 顯示特殊處理

                #endregion

                #region 下拉選單
                ViewBag.OverTypeList = new SelectList(this._overtypeService.GetAll().OrderBy(x => x.NQ_Sort), "ID_OverType", "CX_OverType", query.ID_OverType);
                ViewBag.CountryList = new SelectList(this._countryService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Country", "CX_Country", query.ID_Country);
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
        public ActionResult Edit(OverSea model)
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

                //算遇到幾天
                model.NQ_MeetBirthday = DateHelper.GetBetweenBirthdayCount(
                    DateTime.Parse(model.CX_From_Date),
                    DateTime.Parse(model.CX_To_Date),
                    DateTime.Parse(model.CX_Birthday));

                model.CX_Modify = UserModel.GetUserData().StaffID;
                model.DT_Modify = DateTime.Now;
                #endregion

                #region Service資料庫
                this._overseaService.Update(model);
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

        public ActionResult Delete(int? id)
        {
            try
            {
                #region 驗證
                if (!id.HasValue)
                    return View("Error");
                #endregion

                #region 取資料
                var query = this._overseaService.GetById((int)id);
                #endregion

                #region ViewBag

                #endregion

                #region 顯示特殊處理

                #endregion

                #region 下拉選單
                ViewBag.OverTypeList = new SelectList(this._overtypeService.GetAll().OrderBy(x => x.NQ_Sort), "ID_OverType", "CX_OverType", query.ID_OverType);
                ViewBag.CountryList = new SelectList(this._countryService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Country", "CX_Country", query.ID_Country);
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(OverSea model)
        {
            ResultModel result = new ResultModel();
            try
            {
                #region Service資料庫
                this._overseaService.Delete(model);
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

        public ActionResult BatchCreate(int count = 10)
        {
            #region 下拉選單
            ViewBag.OverTypeList = new SelectList(this._overtypeService.GetAll().OrderBy(x => x.NQ_Sort), "ID_OverType", "CX_OverType", null);
            ViewBag.CountryList = new SelectList(this._countryService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Country", "CX_Country", null);
            #endregion

            #region 產生前端資料
            OverSeaBatchCreateViewModel result = new OverSeaBatchCreateViewModel();
            OverSeaBatchCreateMasterViewModel master = new OverSeaBatchCreateMasterViewModel();
            List<OverSeaBatchCreateDetailViewModel> detail = new List<OverSeaBatchCreateDetailViewModel>();

            for (int i = 0; i < count; i++)
            {
                OverSeaBatchCreateDetailViewModel overseabatchcreatedetail = new OverSeaBatchCreateDetailViewModel()
                {
                    CX_PID = "",
                    FG_IsWorkCard = false
                };
                detail.Add(overseabatchcreatedetail);
            }

            result.Master = master;
            result.Detail = detail;
            #endregion

            #region 回傳
            return View(result);
            #endregion
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BatchCreate(OverSeaBatchCreateViewModel model)
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
                //驗證工號是否都正確
                EmployeeModel script = new EmployeeModel();
                List<EmployeeModel> employeelist = script.GetEmpDataByStaffID("", "0", "0");

                string mCX_Create = UserModel.GetUserData().StaffID;
                DateTime mNow = DateTime.Now;

                List<OverSea> oversealist = new List<OverSea>(0);

                foreach (var item in model.Detail)
                {
                    if (!string.IsNullOrEmpty(item.CX_PID))
                    {
                        var employee = employeelist.Where(x => x.eID == item.CX_PID).FirstOrDefault();

                        if (employee == null)
                            throw new Exception(item.CX_PID + "MessageEmployeeSearchError".ToLocalized());

                        OverSea oversea = new OverSea()
                        {
                            CX_PID = item.CX_PID,
                            CX_Name = employee.eName,
                            ID_Country = model.Master.ID_Country,
                            ID_OverType = model.Master.ID_OverType,
                            CX_From_Date = model.Master.CX_From_Date,
                            CX_To_Date = model.Master.CX_To_Date,
                            CX_Dept_Name = employee.DeptName,
                            CX_Title = employee.TitleName,
                            CX_OnBoard_Date = employee.OnBoard,
                            CX_Birthday = employee.OBirthday,
                            CX_Create = mCX_Create,
                            DT_Create = mNow,
                            CX_OverSea_Remark = model.Master.CX_OverSea_Remark,
                            CX_Place_Remark = model.Master.CX_Place_Remark,
                            FG_IsWorkCard = item.FG_IsWorkCard
                        };

                        //算遇到幾天
                        oversea.NQ_MeetBirthday = DateHelper.GetBetweenBirthdayCount(
                            DateTime.Parse(oversea.CX_From_Date),
                            DateTime.Parse(oversea.CX_To_Date),
                            DateTime.Parse(oversea.CX_Birthday));

                        oversealist.Add(oversea);
                    }
                }


                #endregion

                #region Service資料庫
                this._overseaService.BatchCreate(oversealist);
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
    }
}