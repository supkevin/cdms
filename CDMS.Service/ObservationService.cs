using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public class ObservationService : IObservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.Observation> _repository;
        public ObservationService(IUnitOfWork unitofwork, IRepository<Model.Observation> repository)
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;
        }
        public void Create(Observation model)
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

        public void Update(Observation model)
        {
            #region 取資料
            Observation query = this.Get(model.ID_Observation);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            query.CX_Observation = model.CX_Observation;
            query.NQ_Sort = model.NQ_Sort;
            query.ID_Feedback = model.ID_Feedback;
            query.CX_Observation_Remarks = model.CX_Observation_Remarks;
            #endregion

            #region Models資料庫
            this._repository.Update(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public void Delete(Observation model)
        {
            #region 取資料
            Model.Observation query = this.Get(model.ID_Observation);
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
            this._repository.Delete(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public Observation Get(int id_observation)
        {
            return this._repository.Get(x => x.ID_Observation == id_observation);
        }

        public Observation GetById(int id_observation)
        {
            #region 取資料
            Observation query = this.Get(id_observation);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 回傳

            return query;
            #endregion
        }

        public IEnumerable<Observation> GetAll()
        {
            return this._repository.GetAll();
        }

        public List<Observation> GetForSelect(int id_feedback)
        {
            return this._repository.GetAll().Where(x => x.ID_Feedback == id_feedback).ToList();
        }
    }
}
