using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.News> _repository;

        public NewsService(IUnitOfWork unitofwork, IRepository<Model.News> repository)
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;
        }
     
        private Model.News GetInfoOnCreate(News info) {            
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Model.News GetInfoOnUpdate(News info)
        {
            News query = this.Get(info.NewsID);
            
            // 這裡填要塞的資料                        
            query.DepartmentID = info.DepartmentID;
            query.NewsTypeID = info.NewsTypeID;
            query.ReleaseDate = info.ReleaseDate;
            query.OffDate = info.OffDate;
            query.SetTop = info.SetTop;
            query.Content = info.Content;
            query.Remarks = info.Remarks;
            query.Activate = info.Activate;
            query.LastPerson = IdentityService.GetUserData().UserID;
            query.LastUpdate = DateTime.Now;
            return query;
        }

        public void Create(News model)
        {
            #region 取資料
            model = GetInfoOnCreate(model);
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

        public void Update(News info)
        {
            #region 取資料
            News query = GetInfoOnUpdate(info);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            //query.CX_Observation = info.CX_Observation;
            //query.NQ_Sort = info.NQ_Sort;
            //query.ID_Feedback = info.ID_Feedback;
            //query.CX_Observation_Remarks = info.CX_Observation_Remarks;
            #endregion

            #region Models資料庫
            this._repository.Update(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public void Delete(News info)
        {
            #region 取資料
            Model.News query = this.Get(info.NewsID);            
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            this._repository.Delete(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public News Get(long id)
        {
            return this._repository.Get(x => x.NewsID == id);
        }

        public IQueryable<News> GetAll()
        {
            return this._repository.GetAll();
        }

        public bool IsUsed(News info)
        {
            return false; 
        }
    }
}
