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
    public class ProcessController : BaseController
    {
        class UserPermission
        {

            public List<Store> AllowStore;
            public List<Workplace> AllowWorkplace;
            public List<Status> AllowStatus;

            public UserPermission(IStoreService storeService, IWorkplaceService workplaceService)
            {
                string department = UserModel.GetUserData().DepartmentID;
                string store = UserModel.GetUserData().PosStore;

                List<Store> storeList = storeService.GetAll().ToList();
                List<Workplace> workplaceList = workplaceService.GetAll().ToList();
           
                int flag = GetDepartmentFlag(department);

                if (department != GlobalSettings.IT)
                {
                    storeList = storeList.Where(x => x.CX_StoreNo == store).ToList();
                    workplaceList = workplaceList.Where(x => x.FG_Department == flag).ToList();
                }

                this.AllowStore = storeList;
                this.AllowWorkplace = workplaceList;
                this.AllowStatus = Status.GetProcessStatus();
            }

            private int GetDepartmentFlag(string department)
            {
                int result = -1;

                // 是不是營運部門
                if(GlobalSettings.IsOperateDepartment(department))
                {
                    result = Convert.ToInt16(department.Substring(2, 1));
                }

                return result;
            }
        }

        private readonly IInspectionService _inspetionService;
        private readonly IProcessService _processService;

        private readonly IOverSeaService _overseaService;
        private readonly ICountryService _countryService;
        private readonly IStoreService _storeService;
        private readonly IWorkplaceService _workplaceService;
        private readonly IFeedbackService _feedbackService;
        private readonly IObservationService _observationService;
        private readonly ITrackService _trackService;

        //private readonly int FileMax = 100 * 100 * 100;
        private UserPermission _UserPermission; 

        public ProcessController(IInspectionService inspetionService,
            IProcessService processService,
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
            this._processService = processService;

            this._overseaService = overseaService;
            this._countryService = countryService;
            this._storeService = storeService;
            this._workplaceService = workplaceService;
            this._feedbackService = feedbackService;
            this._observationService = observationService;
            this._trackService = trackService;

            // 目前登入者可用權限
            this._UserPermission = new UserPermission(storeService, workplaceService);
        }

        private void InitViewBag(Inspection info)
        {
            var Country = this._countryService.GetAll().OrderBy(x => x.NQ_Sort).ToList();
            var Store = this._storeService.GetAll().Where(x => x.ID_Country == info.ID_Country).OrderBy(x => x.NQ_Sort).ToList();

            ViewBag.CountryList = new SelectList(Country, "ID_Country", "CX_Country", info.ID_Country);
            ViewBag.SotreList = new SelectList(Store, "ID_Store", "CX_Store_Name", info.ID_Store);

            ViewBag.WrokPlaceList = new SelectList(this._workplaceService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Workplace", "CX_Workplace", info.ID_Workplace);
            ViewBag.FeedBackList = new SelectList(this._feedbackService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Feedback", "CX_Feedback", info.ID_Feedback);

            ViewBag.ObservationList = new SelectList(this._observationService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Observation", "CX_Observation", info.ID_Observation);

            ViewBag.TrackList = new SelectList(this._trackService.GetAll().OrderBy(x => x.NQ_Sort), "ID_Track", "CX_Track", info.ID_Track);

            ViewBag.StatusList = new SelectList(Status.GetAll().OrderBy(x => x.Value), "Value", "Text", info?.ID_Status);
        }

        public ActionResult Index()
        {
            ViewBag.StoreList = new SelectList(
                this._UserPermission.AllowStore.OrderBy(x => x.NQ_Sort), "ID_Store", "CX_Store_Name", null);

            ViewBag.WrokPlaceList = new SelectList(
                this._UserPermission.AllowWorkplace.OrderBy(x => x.NQ_Sort), "ID_Workplace", "CX_Workplace", null);
            
            ViewBag.StatusList = new SelectList(this._UserPermission.AllowStatus.OrderBy(x => x.Value), "Value", "Text", Status.Wait.Value);

            return View();
        }

        

        [HttpPost]
        public ActionResult _List(
            string store, string workplace, string status, string txt, string orderby, string sort, int page = 1)
        {
            #region 設定頁碼 + 傳前端資料(ViewBag)
            int CurrentPage = page < 1 ? 1 : page;

            ViewBag.p = CurrentPage;
            ViewBag.txt = txt == null ? "" : txt;
            ViewBag.orderby = sort == null ? "" : orderby;
            ViewBag.sort = sort == null ? "" : sort;
            #endregion

            #region 組出SQL + 產生資料
            
           
            List<object> obj = new List<object> { txt ,
            string.IsNullOrEmpty(store) ? 0 : Convert.ToInt16(store),
            string.IsNullOrEmpty(workplace) ? 0 : Convert.ToInt16(workplace),
            string.IsNullOrEmpty(status) ? 0 : Convert.ToInt16(status),
            };

            // return Content("qqqqq");
            // 關鍵字

            string sql = " 1 = 1 ";
            if (!string.IsNullOrEmpty(txt))
                sql += " && (CX_Content.Contains(@0) || CX_Improve.Contains(@0) || CX_Date.Contains(@0))";

            if (!string.IsNullOrEmpty(store))
                sql += " && ID_Store == @1";

            if(!string.IsNullOrEmpty(workplace))
                sql += " && ID_Workplace == @2";

            if (!string.IsNullOrEmpty(status))
                sql += " && ID_Status == @3";

            int[] allowStore = this._UserPermission.AllowStore.Select(x => x.ID_Store).ToArray();
            int[] allowWorkplace = this._UserPermission.AllowWorkplace.Select(x => x.ID_Workplace).ToArray();
            int[] allowStatus = this._UserPermission.AllowStatus.Select(x => x.Value).ToArray();

            // 先對登入者的權限做篩選
            var query = this._inspetionService.GetAll()
                    .Where(x =>
                        allowStore.Contains(x.ID_Store)
                        && allowWorkplace.Contains(x.ID_Workplace)
                        && allowStatus.Contains(x.ID_Status)); 

            query = query.Where(sql, obj.ToArray());

            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sort))
                query = query.OrderBy(orderby + " " + sort);

            #endregion

            #region 回傳


            return View("_List", query.ToPagedList(page, PageSize));
            #endregion
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
                                FG_Type = 1,
                            };
                            model.Inspection_Image.Add(image);
                        }
                    }
                }
                #endregion

                #region Service資料庫
                this._processService.Update(model);
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

        [HttpGet]
        public ActionResult AddItem()
        {
            return PartialView("_Item", new Object());
        }
    }
}
