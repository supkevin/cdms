using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public class CodeService : ICodeService
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IRepository<Model.Code> _Repository;
        private readonly IRepository<Model.v_CodeUsed> _CodeUsed;

        private readonly UserInfo _CurrentUser = IdentityService.GetUserData();

        public CodeService(IUnitOfWork unitofwork, 
            IRepository<Model.Code> repository,
            IRepository<Model.v_CodeUsed> codeUsed)
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
            this._CodeUsed = codeUsed;
        }
     
        private Model.Code GetInfoOnCreate(Code info) {
            info.LastPerson = _CurrentUser.UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Code GetInfoOnDelete(Code info)
        {
            info.Activate = YesNo.No.Value;
            info.LastPerson = _CurrentUser.UserID; ;
            info.LastUpdate = DateTime.Now;
            return info;
        }
        private Model.Code GetInfoOnUpdate(Code info)
        {
            Code query = this.Get(info.CodeID);

            // 這裡填要塞的資料   
            //query.CodeType = info.CodeType;   // 不能改
            //query.CodeValue = info.CodeValue; // 不能改
            query.CodeName = info.CodeName;
            query.Activate = info.Activate;
            query.SortOrder = info.SortOrder;
            query.LastPerson = _CurrentUser.UserID;
            query.LastUpdate = DateTime.Now;
            return query;
        }

        public void Create(Code model)
        {
            #region 取資料
            model = GetInfoOnCreate(model);
            #endregion

            #region 邏輯驗證
            if (this.IsCodeExists(model))
            {                
                throw new Exception($"{"CodeValue".ToLocalized()}:{model.CodeValue} 已經存在！");
            }
            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            this._Repository.Create(model);
            this._UnitOfWork.SaveChange();
            #endregion
        }

        public void Update(Code info)
        {
            #region 取資料
            Code query = GetInfoOnUpdate(info);
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

        public void Delete(Code info)
        {
            #region 取資料
            Model.Code query = GetInfoOnDelete(info);            
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            if (this.IsUsed(info))
            {
                this._Repository.Update(info);
            }
            else
            {
                this._Repository.Delete(info);
            }

            this._UnitOfWork.SaveChange();
            #endregion
        }

        public Code Get(long id)
        {
            return this._Repository.Get(x => x.CodeID == id);
        }

        public IEnumerable<Code> GetAll()
        {
            return this._Repository.GetAll()
                .OrderByDescending(x => x.Activate == YesNo.Yes.Value)
                .ThenBy(x => x.SortOrder);
        }

        //public IQueryable<Code> GetByType(string codeType)
        //{
        //    return this._Repository.GetAll().Where(x=>x.CodeType == codeType);
        //}


        public bool IsCodeExists(Code info )
        {
            var query = this._Repository.Get(x => x.CodeType == info.CodeType && x.CodeValue == info.CodeValue);
            return (query != null);
        }

        public bool IsUsed(Code info)
        {
            var query = this._CodeUsed
                .Get(x => x.CodeType == info.CodeType && x.CodeValue == info.CodeValue);
                        
            return query != null;
        }
    }
}
