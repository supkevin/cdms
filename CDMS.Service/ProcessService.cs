using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
namespace CDMS.Service
{
    public class ProcessService : IProcessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.Inspection> _repository;

        private readonly IOverSeaService _overseaService;
        private readonly IInspectionImageService _inspectionimageService;
        public ProcessService(IUnitOfWork unitofwork, IRepository<Model.Inspection> repository,
            IOverSeaService overseaService,
            IInspectionImageService inspectionimageService)
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;

            this._overseaService = overseaService;
            this._inspectionimageService = inspectionimageService;
        }

        private Inspection GetTaget(Inspection model)
        {
            #region 取資料
            Inspection query = this.Get(model.ID_Inspection);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            query.CX_DealWith = model.CX_DealWith;
            query.ID_Status = model.ID_Status;
            query.Inspection_Image = model.Inspection_Image;

            query.CX_Process = IdentityService.GetUserData().UserID;
            query.DT_Process = DateTime.Now; 
            #endregion

            return query;
        }

        public void Update(Inspection model)
        {
            #region 取資料
            Inspection query = GetTaget(model);
            #endregion
          
            #region Models資料庫
            this._repository.Update(query);
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
    }
}
