using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;

namespace CDMS.Service
{
    public class TrackService : ITrackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.Track> _repository;
        public TrackService(IUnitOfWork unitofwork, IRepository<Model.Track> repository)
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;
        }


        public void Create(Track model)
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

        public void Update(Track model)
        {
            #region 取資料
            Track query = this.Get(model.ID_Track);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            query.CX_Track = model.CX_Track;
            query.NQ_Sort = model.NQ_Sort;
            query.CX_Track_Remarks = model.CX_Track_Remarks;
            #endregion

            #region Models資料庫
            this._repository.Update(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public void Delete(Track model)
        {
            #region 取資料
            Model.Track query = this.Get(model.ID_Track);
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

        public Track Get(int id_track)
        {
            return this._repository.Get(x => x.ID_Track == id_track);
        }

        public Track GetById(int id_track)
        {
            #region 取資料
            Track query = this.Get(id_track);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 回傳

            return query;
            #endregion
        }

        public IEnumerable<Track> GetAll()
        {
            return this._repository.GetAll();
        }
    }
}
