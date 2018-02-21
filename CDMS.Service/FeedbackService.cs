using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;

namespace CDMS.Service
{
    public class FeedbackService : IFeedbackService
    {
         private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.Feedback> _repository;
        public FeedbackService(IUnitOfWork unitofwork, IRepository<Model.Feedback> repository)
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;
        }

        public void Create(Feedback model)
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

        public void Update(Feedback model)
        {
            #region 取資料
            Feedback query = this.Get(model.ID_Feedback);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            query.CX_Feedback = model.CX_Feedback;
            query.NQ_Sort = model.NQ_Sort;
            query.CX_Feeback_Remarks = model.CX_Feeback_Remarks;
            #endregion

            #region Models資料庫
            this._repository.Update(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public void Delete(Feedback model)
        {
            #region 取資料
            Model.Feedback query = this.Get(model.ID_Feedback);
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

        public Feedback Get(int id_feedback)
        {
            return this._repository.Get(x => x.ID_Feedback == id_feedback);
        }

        public Feedback GetById(int id_feedback)
        {
            #region 取資料
            Feedback query = this.Get(id_feedback);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 回傳

            return query;
            #endregion
        }

        public IEnumerable<Feedback> GetAll()
        {
            return this._repository.GetAll();
        }
    }
}
