using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
namespace CDMS.Service
{
    public class InspectionService : IInspectionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.Inspection> _repository;

        private readonly IOverSeaService _overseaService;
        private readonly IInspectionImageService _inspectionimageService;
        public InspectionService(IUnitOfWork unitofwork, IRepository<Model.Inspection> repository, IOverSeaService overseaService, IInspectionImageService inspectionimageService)
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;

            this._overseaService = overseaService;
            this._inspectionimageService = inspectionimageService;
        }

        private Inspection GetTargetOnUpdate(Inspection model) {
            #region 取資料
            Inspection query = this.Get(model.ID_Inspection);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion
                        
            query.ID_Country = model.ID_Country;
            query.ID_Store = model.ID_Store;
            query.ID_Workplace = model.ID_Workplace;
            query.ID_Feedback = model.ID_Feedback;
            query.ID_Observation = model.ID_Observation;
            query.ID_Track = model.ID_Track;    
            query.CX_Content = model.CX_Content;
            query.CX_Date = model.CX_Date;

            query.ID_Status = model.ID_Status;
            query.CX_Modify = IdentityService.GetUserData().UserID;
            query.DT_Modfiy = DateTime.Now;

            query.Inspection_Image = model.Inspection_Image;

            return query;
        }
        private Inspection GetTargetOnClose(Inspection model)
        {
            #region 取資料
            Inspection query = this.Get(model.ID_Inspection);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            query.ID_Status = model.ID_Status;
            query.CX_Modify = IdentityService.GetUserData().UserID;
            query.DT_Modfiy = DateTime.Now;
            
            // 狀態是結案寫入結案日期
            if (model.ID_Status == Status.Close.Value)
            {
                query.CX_Close = IdentityService.GetUserData().UserID;
                query.DT_Close = DateTime.Now;
            }
            return query;
        }
        private Inspection GetTargetOnCreate(Inspection model)
        {
            model.DT_Create = DateTime.Now;
            model.CX_Create = IdentityService.GetUserData().UserID;          
            return model;
        }

        public void Create(Inspection model)
        {
            #region 取資料
            model = GetTargetOnCreate(model);
            #endregion

            #region 邏輯驗證
            //依日期判斷他是那一次出差的抓給他ID_OverSea
            //ID_OverSea
            //var queryoversea = this._overseaService.GetForInspection(model.CX_PID, model.CX_Date);
            //if (queryoversea == null)
            //    throw new Exception(string.Format("MessageOverSeaNotYourData".ToLocalized(), model.CX_PID, model.CX_Date));

            //if (model.ID_Feedback.Equals(2) && model.ID_Track <= 1)//如果是建議時 要填寫後續追蹤狀況
            //    throw new Exception(string.Format("MessageFeedback2Error".ToLocalized(), model.CX_PID, model.CX_Date)); //不可是0 或1無

            #endregion

            #region 變為Models需要之型別及邏輯資料
            //model.ID_OverSea = queryoversea.ID_OverSea;

            //由cookie抓前端作
            //CX_PID
            //CX_Name

            #endregion

            #region Models資料庫
            this._repository.Create(model);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public void Update(Inspection model)
        {
            #region 取資料
            Inspection query = GetTargetOnUpdate(model);
            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            this._repository.Update(query);
            this._unitOfWork.SaveChange();
            #endregion
        }
        public void Close(Inspection model)
        {
            #region 取資料
            Inspection query = GetTargetOnClose(model);
            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            this._repository.Update(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public void Delete(Inspection model)
        {
            #region 取資料
            Model.Inspection query = this.Get(model.ID_Inspection);
            //var queryoverseastaff = this._overseaService.GetForOverType(query.ID_OverType);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());

            //驗證
            //if (queryoverseastaff == null)//沒有資料
            //    throw new Exception("MessageDataHasLinking".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            //刪圖片
            this._inspectionimageService.DeleteWithOutSaveChange(model.ID_Inspection);

            this._repository.Delete(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public Inspection Get(int id_inspection)
        {
            return this._repository.Get(x => x.ID_Inspection == id_inspection);
        }

        public Inspection GetById(int id_inspection)
        {
            #region 取資料
            Inspection query = this.Get(id_inspection);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 回傳

            return query;
            #endregion
        }

        public IEnumerable<Inspection> GetAll()
        {
            return this._repository.GetAll().IncludeMultiple(
                x => x.Inspection_Image
                );
        }

        public Inspection GetSameToday(string cx_pid, string cx_date)
        {
            return this._repository.GetAll().OrderByDescending(x=>x.ID_Inspection).Where(x => x.CX_PID.Equals(cx_pid) && x.CX_Date.Equals(cx_date)).FirstOrDefault();
        }
    }
}
