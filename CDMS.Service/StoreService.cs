using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
namespace CDMS.Service
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.Store> _repository;
        public StoreService(IUnitOfWork unitofwork, IRepository<Model.Store> repository)
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;
        }

        public void Create(Model.Store model)
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

        public void Update(Model.Store model)
        {
            #region 取資料
            Store query = this.Get(model.ID_Store);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            query.CX_Store_Name = model.CX_Store_Name;
            query.NQ_Sort = model.NQ_Sort;
            query.CX_Store_Remarks = model.CX_Store_Remarks;
            #endregion

            #region Models資料庫
            this._repository.Update(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public void Delete(Model.Store model)
        {
            #region 取資料
            Model.Store query = this.Get(model.ID_Store);
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

        public Model.Store Get(int id_store)
        {
            return this._repository.Get(x => x.ID_Store == id_store);
        }

        public Model.Store GetById(int id_store)
        {
            #region 取資料
            Store query = this.Get(id_store);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 回傳

            return query;
            #endregion
        }

        public IEnumerable<Model.Store> GetAll()
        {
            return this._repository.GetAll();
        }


        public List<Store> GetForSelect(int id_country)
        {
            return this._repository.GetAll().Where(x=>x.ID_Country== id_country).ToList();
        }
    }
}
