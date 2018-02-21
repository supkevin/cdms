using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;

namespace CDMS.Service
{
    public class OverTypeService : IOverTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.OverType> _repository;

        private readonly IOverSeaService _overseaService;

        public OverTypeService(IUnitOfWork unitofwork, IRepository<Model.OverType> repository,IOverSeaService overseaService)
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;

            this._overseaService = overseaService;
        }

        public void Create(Model.OverType model)
        {
            #region 取資料

            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            this._repository.Create(model);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public void Update(Model.OverType model)
        {
            #region 取資料
            OverType query = this.Get(model.ID_OverType);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            query.CX_OverType = model.CX_OverType;
            query.NQ_Sort = model.NQ_Sort;            
            #endregion

            #region Models資料庫
            this._repository.Update(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public void Delete(Model.OverType model)
        {
            #region 取資料
            Model.OverType query = this.Get(model.ID_OverType);
            var queryoverseastaff = this._overseaService.GetForOverType(query.ID_OverType);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());

            //驗證
            if (queryoverseastaff == null)//沒有資料
                throw new Exception("MessageDataHasLinking".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            this._repository.Delete(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public Model.OverType Get(int id_overseatype)
        {
            return this._repository.Get(x => x.ID_OverType == id_overseatype);
        }

        public Model.OverType GetById(int id_overseatype)
        {
            #region 取資料
            OverType query = this.Get(id_overseatype);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 回傳

            return query;
            #endregion
        }

        public IEnumerable<Model.OverType> GetAll()
        {
            return this._repository.GetAll();
        }





    }
}
