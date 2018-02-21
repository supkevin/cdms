using CDMS.Language;
using CDMS.Model;
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
namespace CDMS.Web.Controllers
{
    public class InspectionController : BaseController
    {
        private readonly IInspectionService _inspetionService;

        private readonly IOverSeaService _overseaService;
        private readonly ICountryService _countryService;
        private readonly IStoreService _storeService;
        private readonly IWorkplaceService _workplaceService;
        private readonly IFeedbackService _feedbackService;
        private readonly IObservationService _observationService;
        private readonly ITrackService _trackService;
        
        //private readonly int FileMax = 100 * 100 * 100;

        public InspectionController(IInspectionService inspetionService,
            IOverSeaService overseaService,
            ICountryService countryService,
            IStoreService storeService,
            IWorkplaceService workplaceService,
            IFeedbackService feedbackService,
            IObservationService observationService,
            ITrackService trackService
            )
        {
            this._inspetionService = inspetionService;

            this._overseaService = overseaService;
            this._countryService = countryService;
            this._storeService = storeService;
            this._workplaceService = workplaceService;
            this._feedbackService = feedbackService;
            this._observationService = observationService;
            this._trackService = trackService;            
        }


        public ActionResult Index()
        {

            ViewBag.SotreList = new SelectList(
                this._storeService.GetAll().Where(x => x.ID_Country == GlobalSettings.CurrentCountry),
                "ID_Store", "CX_Store_Name", null);

            ViewBag.StatusList = new SelectList(Status.GetAll().OrderBy(x => x.Value), "Value", "Text", null);
            return View();
        }

        [HttpPost]
        public ActionResult _List(string store, string status, string txt, string orderby, string sort, int page = 1)
        {
            #region 設定頁碼 + 傳前端資料(ViewBag)
            int CurrentPage = page < 1 ? 1 : page;

            ViewBag.p = CurrentPage;
            ViewBag.txt = txt == null ? "" : txt;
            ViewBag.orderby = sort == null ? "" : orderby;
            ViewBag.sort = sort == null ? "" : sort;
            ViewBag.store = store == null ? "" : store;
            ViewBag.status = status == null ? "" : status;
            #endregion

            #region 組出SQL + 產生資料
            string sql = " 1 = 1 ";

            // 只能查詢登入者部門
            string department = UserModel.GetUserData().DepartmentID;

            List<object> obj = new List<object> {
                department,
                txt,
                string.IsNullOrEmpty(store) ? 0 : Convert.ToInt16(store),
                string.IsNullOrEmpty(status) ? 0 : Convert.ToInt16(status),
                }; //

            sql += " && ID_Department = @0 "; // 只能查詢登入者部門

            if (!string.IsNullOrEmpty(txt))
                sql += " && (CX_Content.Contains(@1) || CX_Improve.Contains(@1) || CX_Date.Contains(@1))";

            if (!string.IsNullOrEmpty(store))
                sql += " && ID_Store == @2";

            if (!string.IsNullOrEmpty(status))
                sql += " && ID_Status == @3";
                        
            var query = this._inspetionService.GetAll().Where(sql, obj.ToArray());

            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sort))
                query = query.OrderBy(orderby + " " + sort);

            #endregion

            #region 回傳


            return View("_List", query.ToPagedList(page, PageSize));
            #endregion
        }

        public ActionResult Create()
        {
            #region 取資料
            //var info = this._inspetionService.GetSameToday(UserModel.GetUserData().UserID, DateTime.Now.ToString("yyyy-MM-dd"));

            int mID_Country = 1; // 固定台灣            
            int mID_Store = 1;
            #endregion

            #region 下拉選
        　   var Country = this._countryService.GetAll().OrderBy(x => x.NQ_Sort).ToList();
            var Store = this._storeService.GetAll().Where(x => x.ID_Country == mID_Country).OrderBy(x => x.NQ_Sort).ToList();
            ViewBag.CountryList = new SelectList(Country, "ID_Country", "CX_Country", mID_Country);
            ViewBag.SotreList = new SelectList(Store, "ID_Store", "CX_Store_Name", mID_Store);
            ViewBag.WrokPlaceList = new SelectList(this._workplaceService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Workplace", "CX_Workplace", null);
            ViewBag.FeedBackList = new SelectList(this._feedbackService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Feedback", "CX_Feedback", null);

            ViewBag.ObservationList = new SelectList(this._observationService.GetAll().Where(x => 1 == 2).OrderBy(x => x.NQ_Sort), "ID_Observation", "CX_Observation", null);

            ViewBag.TrackList = new SelectList(this._trackService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Track", "CX_Track", null);

            ViewBag.StatusList = new SelectList(Status.GetAll().OrderBy(x => x.Value), "Value", "Text", null);
            #endregion

            #region 產生資料
            Inspection result = new Inspection()
            {
                CX_Date = DateTime.Now.ToString(GlobalSettings.DATE_FORMAT),
                ID_Store = mID_Store,
                ID_Country = mID_Country,
                ID_Status = Status.Draft.Value
            };

            #endregion

            #region 回傳
            return View(result);
            #endregion
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inspection model, IEnumerable<HttpPostedFileBase> file)
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

                if (file != null && file.Count() > 0)
                {
                    foreach (var item in file)
                    {
                        if (item != null)
                        {
                            int contentLength = item.ContentLength;
                            byte[] byteImage = new byte[contentLength];
                            item.InputStream.Read(byteImage, 0, contentLength);

                            Inspection_Image image = new Inspection_Image()
                            {
                                ID_Inspection = model.ID_Inspection,
                                BI_Inspection_Image = byteImage
                            };
                            model.Inspection_Image.Add(image);
                        }
                    }
                }

                model.CX_PID = UserModel.GetUserData().StaffID;
                model.CX_Name = UserModel.GetUserData().DisplayName;
                model.ID_Department = UserModel.GetUserData().DepartmentID;
                #endregion

                #region Service資料庫
                this._inspetionService.Create(model);
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
                var query = this._inspetionService.GetById((int)id);
                #endregion

                #region ViewBag

                #endregion

                #region 顯示特殊處理

                #endregion


                #region 下拉選單
                InitViewBag(query);
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


        private void InitViewBag(Inspection info)
        {
            ViewBag.CountryList = new SelectList(
                this._countryService.GetAll().OrderBy(x => x.NQ_Sort).ToList(), 
                "ID_Country", "CX_Country", GlobalSettings.CurrentCountry);

            ViewBag.SotreList = new SelectList(
                this._storeService.GetAll().Where(x => x.ID_Country == GlobalSettings.CurrentCountry), 
                "ID_Store", "CX_Store_Name", info.ID_Store);

            ViewBag.WrokPlaceList = new SelectList(this._workplaceService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Workplace", "CX_Workplace", info.ID_Workplace);
            ViewBag.FeedBackList = new SelectList(this._feedbackService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Feedback", "CX_Feedback", info.ID_Feedback);
            ViewBag.ObservationList = new SelectList(this._observationService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Observation", "CX_Observation", info.ID_Workplace);
            ViewBag.TrackList = new SelectList(this._trackService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Track", "CX_Track", info.ID_Track);
            ViewBag.StatusList = new SelectList(Status.GetAll().OrderBy(x => x.Value), "Value", "Text", info?.ID_Status);
        }

        public ActionResult Display(int? id)
        {
            try
            {
                #region 驗證
                if (!id.HasValue)
                    return View("Error");
                #endregion

                #region 取資料
                var query = this._inspetionService.GetById((int)id);
                #endregion

                #region ViewBag

                #endregion

                #region 顯示特殊處理

                #endregion


                #region 下拉選單
                InitViewBag(query);
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
        [MultipleButton(Name = "action", Argument = "Display")]
        public ActionResult Display(Inspection model)
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

                #endregion

                #region Service資料庫
                this._inspetionService.Close(model);
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
        [MultipleButton(Name = "action", Argument = "Edit")]
        public ActionResult Edit(Inspection model, IEnumerable<HttpPostedFileBase> file)
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

                if (file != null && file.Count() > 0)
                {
                    foreach (var item in file)
                    {
                        if (item != null)
                        {
                            int contentLength = item.ContentLength;
                            byte[] byteImage = new byte[contentLength];
                            item.InputStream.Read(byteImage, 0, contentLength);

                            Inspection_Image image = new Inspection_Image()
                            {
                                ID_Inspection = model.ID_Inspection,
                                BI_Inspection_Image = byteImage,
                                FG_Type = 0,
                            };
                            model.Inspection_Image.Add(image);
                        }
                    }
                }

                model.CX_Modify = UserModel.GetUserData().StaffID;
                model.DT_Modfiy = DateTime.Now;
                #endregion

                #region Service資料庫
                this._inspetionService.Update(model);
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
        [MultipleButton(Name = "action", Argument = "Delete")]
        public ActionResult DeleteConfirmed(Inspection model)
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

                #endregion

                #region Service資料庫
                this._inspetionService.Delete(model);
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

        public ActionResult His()
        {

            #region 下拉選單

            ViewBag.CountryList = new SelectList(this._countryService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Country", "CX_Country", null);
            List<Store> SotreList = new List<Store>(0);
            ViewBag.SotreList = new SelectList(SotreList, "ID_Store", "CX_Store_Name", null);
            ViewBag.WrokPlaceList = new SelectList(this._workplaceService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Workplace", "CX_Workplace", null);
            ViewBag.FeedBackList = new SelectList(this._feedbackService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Feedback", "CX_Feedback", null);
            ViewBag.ObservationList = new SelectList(this._observationService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Observation", "CX_Observation", null);
            ViewBag.TrackList = new SelectList(this._trackService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Track", "CX_Track", null);
            #endregion

            return View();
        }

        [HttpPost]
        public ActionResult _HisList(
            string orderby,
            string sort,
            int pagesize,
            string txt,
            string date,
            int? id_country,
            int? id_store,
            int? id_workpace,
            int? id_feedback,
            int? id_observation,
            int? id_track,
            int page = 1)
        {
            int CurrentPage = page < 1 ? 1 : page;
            ViewBag.p = CurrentPage;
            ViewBag.orderby = sort == null ? "" : orderby;
            ViewBag.sort = sort == null ? "" : sort;
            ViewBag.PageSize = pagesize;

            ViewBag.txt = txt == null ? "" : txt;
            ViewBag.date = date == null ? "" : date;
            ViewBag.id_country = id_country == null ? 0 : id_country;
            ViewBag.id_store = id_store == null ? 0 : id_store;
            ViewBag.id_workpace = id_workpace == null ? 0 : id_workpace;
            ViewBag.id_feedback = id_feedback == null ? 0 : id_feedback;
            ViewBag.id_observation = id_observation == null ? 0 : id_observation;
            ViewBag.id_track = id_track == null ? 0 : id_track;

            #region 取資料
            var result = this.Get(
                //page,
                orderby,
                sort,
                //pagesize,
                txt,
                date,
              id_country,
              id_store,
              id_workpace,
              id_feedback,
              id_observation,
              id_track);
            #endregion

            #region 回傳
            return View("_HisList", result.ToPagedList(page, pagesize));
            #endregion
        }

        public ActionResult HisListExport(
            string orderby,
            string sort,
            //int pagesize,
            string txt,
            string date,
            int? id_country,
            int? id_store,
            int? id_workpace,
            int? id_feedback,
            int? id_observation,
            int? id_track
            //int page = 1
            )
        {

            string result = this.Export(
                //page,
                orderby,
                sort,
                //pagesize,
                txt,
                date,
                id_country,
                id_store,
                id_workpace,
                id_feedback,
                id_observation,
                id_track);

            //https://stackoverflow.com/questions/28628233/html-file-download-using-mvc-fileresult-looks-different-when-downloading-from-se
            byte[] b = System.Text.Encoding.UTF8.GetBytes(result);
            return File(b, "text/html", date + ".html");
        }

        /// <summary>
        /// 匯出html
        /// </summary>
        /// <param name="orderby"></param>
        /// <param name="sort"></param>
        /// <param name="txt"></param>
        /// <param name="date"></param>
        /// <param name="id_country"></param>
        /// <param name="id_store"></param>
        /// <param name="id_workpace"></param>
        /// <param name="id_feedback"></param>
        /// <param name="id_observation"></param>
        /// <param name="id_track"></param>
        /// <returns></returns>
        private string Export(
            //int page,
            string orderby,
            string sort,
            //int pagesize,
            string txt,
            string date,
            int? id_country,
            int? id_store,
            int? id_workpace,
            int? id_feedback,
            int? id_observation,
            int? id_track)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            #region 取資料
            var query = this.Get(
                //page,
                orderby,
                sort,
                //pagesize,
                txt,
                date,
                id_country,
                id_store,
                id_workpace,
                id_feedback,
                id_observation,
                id_track).ToList();

            var queryCountry = this._countryService.GetById(Convert.ToInt32(id_country));

            var queryWork = this._workplaceService.GetAll().OrderBy(x => x.NQ_Sort);
            #endregion

            #region 產生前端資料
            string mFontFamily = "DFKai-sb";

            #region Html Head
            result.Append("<!DOCTYPE html>");
            result.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            result.Append("<head>");
            result.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/>");
            result.Append("<title></title>");
            //result.Append("<link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css\" integrity=\"sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u\" crossorigin=\"anonymous\">");
            // result.Append("<link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap-theme.min.css\" integrity=\"sha384-rHyoN1iRsVXV4nD0JutlnGaslCJuC7uwjduW9SVrLvRYooPp2bWYgmgJQIXwl/Sp\" crossorigin=\"anonymous\">");
            result.Append("</head><body>");
            #endregion

            #region 日期(單日)
            result.AppendFormat("<h4 style='font-family:{2};'>{0}:{1}</h4>", "CX_Date".ToLocalized(), date, mFontFamily);
            #endregion

            #region 國家(單個)
            result.AppendFormat("<h4 style='font-family:{2};'>{0}:{1}</h4>", "CX_Country".ToLocalized(), queryCountry.CX_Country, mFontFamily);
            #endregion

            #region  門市(多筆)
            var querydistinctstore = query.GroupBy(x => x.Store.CX_Store_Name).Select(g => g.First().Store.CX_Store_Name).ToList();

            StringBuilder sbstore = new StringBuilder();
            foreach (var item in querydistinctstore)
                sbstore.Append(item + "、");

            if (sbstore.Length > 0)
                sbstore.Length = sbstore.Length - 1;

            result.AppendFormat("<h4 style='font-family:{2};'>{0}:{1}</h4>", "CX_Store_Name".ToLocalized(), sbstore.ToString(), mFontFamily);
            #endregion

            #region 巡店人員(多位)

            //var querydistinctdept = info.GroupBy(x => x.OverSea.CX_Dept_Name_Short).Select(g => g.First().OverSea.CX_Dept_Name_Short).ToList();

            //StringBuilder sbpeople = new StringBuilder();
            //sbpeople.AppendFormat("<h4 style='font-family:{1};'>{0}:", "TextInspectionPeople".ToLocalized(), mFontFamily);
            //sbpeople.Append("<ul>");
            //foreach (var item in querydistinctdept)
            //{
            //    sbpeople.Append("<li>");
            //    var querydistinctpeople = info.OrderByDescending(x => x.OverSea.CX_From_Date).Where(x => x.OverSea.CX_Dept_Name_Short.Equals(item)).GroupBy(x => x.OverSea.CX_Name).Select(g => g.First().CX_Name).ToList();

            //    StringBuilder tmp = new StringBuilder();

            //    //組別
            //    if (querydistinctpeople.Count > 0)
            //        sbpeople.Append(item + ":");

            //    //人員
            //    foreach (var itemD in querydistinctpeople)
            //        tmp.Append(itemD + "、");

            //    if (tmp.Length > 0)
            //        tmp.Length = tmp.Length - 1;

            //    sbpeople.Append(tmp.ToString());

            //    sbpeople.Append("</li>");
            //}
            //sbpeople.Append("</ul>");
            //sbpeople.Append("</h4>");
            //result.AppendFormat(sbpeople.ToString());

            #endregion

            #region 報告大綱(抓工作崗位)(有超連結點到下面)

            StringBuilder sbtitle = new StringBuilder();
            sbtitle.AppendFormat("<h4 style='font-family:{1};' id='top'>{0}:", "TextReportTitle".ToLocalized(), mFontFamily);
            sbtitle.Append("<ul>");
            foreach (var item in queryWork)
            {
                sbtitle.AppendFormat("<li><a href='#{0}'>{1}</a></li>", item.ID_Workplace, item.CX_Workplace);
            }

            sbtitle.Append("</ul>");
            sbtitle.Append("</h4>");
            result.AppendFormat(sbtitle.ToString());
            #endregion

            #region  記錄內容(依報告大綱分類)

            result.AppendFormat("<h4 style='font-family:{1};'>{0}:</h4>", "CX_Content".ToLocalized(), mFontFamily);

            foreach (var item in queryWork)
            {
                StringBuilder sbcontent = new StringBuilder();
                sbcontent.AppendFormat("<h4 id='{0}' style='font-family:{2};'>{1}</h4>", item.ID_Workplace, item.CX_Workplace, mFontFamily);

                var queryinspection = query.Where(x => x.ID_Workplace == item.ID_Workplace).OrderBy(x => x.Feedback.NQ_Sort).ThenBy(x => x.Observation.NQ_Sort);

                #region Table
                if (queryinspection.Count() > 0)
                {
                    sbcontent.AppendFormat("<table board='1' style='font-family:{0};'>", mFontFamily);// class='table table-bordered
                    //thead
                    sbcontent.Append("<thead>");
                    sbcontent.Append("<tr align='center' style='background-color:#e0e0e0;'>");
                    sbcontent.AppendFormat(@"
<td width='5%' style='word-break:break-all;'><strong>{0}</strong></td>
<td width='8%' style='word-break:break-all;'><strong>{1}</strong></td>
<td width='10%' style='word-break:break-all;'><strong>{2}</strong></td>
<td width='32%' style='word-break:break-all;'><strong>{3}</strong></td>
<td width='32%' style='word-break:break-all;'><strong>{4}</strong></td>
<td width='8%' style='word-break:break-all;'><strong>{5}</strong></td>
<td width='5%' style='word-break:break-all;'><strong>{6}</strong></td>",
                        "TextiCount".ToLocalized(),
                        "CX_Feedback".ToLocalized(),
                        "CX_Observation".ToLocalized(),
                        "CX_Content".ToLocalized(),
                        "Inspection_Image".ToLocalized(),
                        "CX_Track".ToLocalized(),
                        "CX_Name".ToLocalized());
                    sbcontent.Append("</tr>");
                    sbcontent.Append("</thead>");

                    //tbody
                    sbcontent.Append("<tbody>");

                    int i = 1;
                    foreach (var itemI in queryinspection)
                    {
                        sbcontent.AppendFormat("<tr style='background-color:{0};'>", itemI.Workplace.CX_Color);
                        sbcontent.AppendFormat("<td align='center' style='word-break:break-all;'>{0}</td>", i.ToString());
                        sbcontent.AppendFormat("<td style='word-break:break-all;'>{0}</td>", itemI.Feedback.CX_Feedback);
                        sbcontent.AppendFormat("<td style='word-break:break-all;'>{0}</td>", itemI.Observation.CX_Observation);
                        sbcontent.AppendFormat("<td style='word-break:break-all;'>{0}</td>", itemI.CX_Content);

                        //圖片
                        StringBuilder tmp = new StringBuilder();
                        foreach (var itemP in itemI.Inspection_Image)
                        {
                            string src = ImageClass.GetImage64FromByte(itemP.BI_Inspection_Image);
                            tmp.AppendFormat("<a href='{1}' target='_blank'><img style='max-width: {0}px; height: auto;' src='{1}' ></a>", 200, src);
                        }

                        sbcontent.AppendFormat("<td style='word-break:break-all;'>{0}</td>", tmp.ToString());
                        sbcontent.AppendFormat("<td style='word-break:break-all;'>{0}</td>", itemI.Track.CX_Track);
                        sbcontent.AppendFormat("<td style='word-break:break-all;'>{0}</td>", itemI.CX_Name);
                        sbcontent.Append("</tr>");
                        i++;
                    }
                    sbcontent.Append("</tbody>");
                    sbcontent.Append("</table>");
                }
                else
                {
                    sbcontent.AppendFormat("<h5 style='font-family:{1};'>{0}</h5>", "TextNoData".ToLocalized(), mFontFamily);
                }
                #endregion

                result.AppendFormat(sbcontent.ToString());
                result.AppendFormat("<a href='#top'  style='font-family:{1};'>{0}</a>", "TextGoTitle".ToLocalized(), mFontFamily);
            }


            #endregion

            #region Html End
            //result.Append("<script  src=\"https://code.jquery.com/jquery-1.12.4.min.js\"  integrity=\"sha256-ZosEbRLbNQzLpnKIkEdrPv7lOy9C27hHQ+Xp8a4MxAQ=\"  crossorigin=\"anonymous\"></script>");
            //result.Append("<script src=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js\" integrity=\"sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa\" crossorigin=\"anonymous\"></script>");
            result.Append("</body></html>");
            #endregion

            #endregion

            #region 回傳
            return result.ToString();
            #endregion
        }

        /// <summary>
        /// 取得巡檢資料
        /// </summary>
        /// <param name="orderby"></param>
        /// <param name="sort"></param>
        /// <param name="txt"></param>
        /// <param name="date"></param>
        /// <param name="id_country"></param>
        /// <param name="id_store"></param>
        /// <param name="id_workpace"></param>
        /// <param name="id_feedback"></param>
        /// <param name="id_observation"></param>
        /// <param name="id_track"></param>
        /// <returns></returns>
        private IEnumerable<Inspection> Get(
            //int page,
            string orderby,
            string sort,
            //int pagesize,
            string txt,
            string date,
            int? id_country,
            int? id_store,
            int? id_workpace,
            int? id_feedback,
            int? id_observation,
            int? id_track)
        {
            #region 組出SQL + 產生資料
            string Sql = " 1 = 1 ";
            List<object> obj = new List<object> { txt, date, id_country, id_store, id_workpace, id_feedback, id_observation, id_track };

            if (!string.IsNullOrEmpty(txt))
                Sql += " && (CX_PID.Contains(@0) || CX_Name.Contains(@0))";

            if (!string.IsNullOrEmpty(date))
                Sql += " &&  CX_Date =  @1 ";

            if (id_country.HasValue && id_country != 0)
                Sql += " && ID_Country = @2 ";

            if (id_store.HasValue && id_store != 0)
                Sql += " && ID_Store = @3 ";

            if (id_workpace.HasValue && id_workpace != 0)
                Sql += " && ID_Workplace = @4 ";

            if (id_feedback.HasValue && id_feedback != 0)
                Sql += " && ID_Feedback = @5 ";

            if (id_observation.HasValue && id_observation != 0)
                Sql += " && ID_Observation = @6 ";

            if (id_track.HasValue && id_track != 0)
                Sql += " && ID_Track = @7 ";

            var query = this._inspetionService.GetAll().Where(Sql, obj.ToArray());

            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sort))
                query = query.OrderBy(orderby + " " + sort);

            #endregion

            #region 回傳
            return query;
            #endregion
        }
    }
}
