using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IRepository<Model.BankAccount> _Repository;

        public BankAccountService(IUnitOfWork unitofwork, IRepository<Model.BankAccount> repository)
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
        }
     
        private Model.BankAccount GetInfoOnCreate(BankAccount info) {
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Model.BankAccount GetInfoOnUpdate(BankAccount info)
        {
            BankAccount query = this.Get(info.SeqNo);

            // 這裡填要塞的資料            
            query.BankID = info.BankID;
            query.AccountID = info.AccountID;
            query.BankName = info.BankName;
            query.AccountName = info.AccountName;
            query.InitialAmount = info.InitialAmount;
            query.InitialDate = info.InitialDate;
            query.LastNumber = info.LastNumber;
            query.LastDate = info.LastDate;
            query.Remarks = info.Remarks;
            query.LastPerson = IdentityService.GetUserData().UserID;
            query.LastUpdate = DateTime.Now;
            return query;
        }

        public void Create(BankAccount info)
        {
            #region 取資料
            info = GetInfoOnCreate(info);
            #endregion

            #region 邏輯驗證
            if (this.IsDataExists(info))
            {                
                throw new Exception($"{"BankID".ToLocalized()}:{info.BankID}-{info.AccountID} 已經存在！");
            }
            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            this._Repository.Create(info);
            this._UnitOfWork.SaveChange();
            #endregion
        }

        public void Update(BankAccount info)
        {
            #region 取資料
            BankAccount query = GetInfoOnUpdate(info);
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

        public void Delete(BankAccount info)
        {
            #region 取資料
            Model.BankAccount query = this.Get(info.SeqNo);            
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            this._Repository.Delete(query);
            this._UnitOfWork.SaveChange();
            #endregion
        }

        public BankAccount Get(long id)
        {
            return this._Repository.Get(x => x.SeqNo == id);
        }

        public IEnumerable<BankAccount> GetAll()
        {
            return this._Repository.GetAll();
        }

        public bool IsDataExists(BankAccount info )
        {
            var query = this._Repository
                .Get(x => x.BankID == info.BankID && x.AccountID == info.AccountID);
            return (query != null);
        }

        public bool IsUsed(BankAccount info)
        {
            //var query = this._Repository.Get(x => x.SeqNo == info.SeqNo);
            //var result = query.Product.Any();
            //this._Repository.HandleDetached(query); // 
            //return result;
            return true;
        }
    }
}
