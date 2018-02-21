using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IRepository<Model.Brand> _Repository;

        private readonly UserInfo _CurrentUser = IdentityService.GetUserData();

        public BrandService(IUnitOfWork unitofwork, IRepository<Model.Brand> repository)
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
        }
     
        private Model.Brand GetInfoOnCreate(Brand info) {
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Model.Brand GetInfoOnDelete(Brand info)
        {
            info.Activate = YesNo.No.Value;
            info.LastPerson = _CurrentUser.UserID; ;
            info.LastUpdate = DateTime.Now;

            return info;
        }

        private Model.Brand GetInfoOnUpdate(Brand info)
        {
            Brand query = this.Get(info.BrandID);

            // 這裡填要塞的資料
            query.BrandName = info.BrandName;
            query.Activate = info.Activate;
            query.LastPerson = IdentityService.GetUserData().UserID;
            query.LastUpdate = DateTime.Now;
            return query;
        }

        public void Create(Brand model)
        {
            #region 取資料
            model = GetInfoOnCreate(model);
            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料
                        
            #endregion

            #region Models資料庫
            this._Repository.Create(model);
            this._UnitOfWork.SaveChange();
            #endregion
        }

        public void Update(Brand info)
        {
            #region 取資料
            Brand query = GetInfoOnUpdate(info);
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
            this._Repository.Update(query);
            this._UnitOfWork.SaveChange();
            #endregion
        }

        public void Delete(Brand info)
        {
            #region 取資料            
            #endregion

            #region 邏輯驗證
            if (info == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫                    
            this._Repository.Delete(info);
            this._UnitOfWork.SaveChange();
            #endregion        
        }

        public Brand Get(string id)
        {
            return this._Repository.Get(x => x.BrandID == id);
        }

        public IEnumerable<Brand> GetAll()
        {
            return this._Repository.GetAll();
        }

        public List<Brand> GetForSelect(string id)
        {
            return this._Repository.GetAll().Where(x => x.BrandID == id).ToList();
        }

        public bool IsCodeExists(Brand info )
        {
            var query = this._Repository.Get(x => x.BrandID == info.BrandID);
            return (query != null);
        }

        public bool IsUsed(Brand info)
        {
            var query = this._Repository.Get(x => x.BrandID == info.BrandID);
            var result = query.Product.Any();
            this._Repository.HandleDetached(query); // 
            return result;
        }
    }
}
