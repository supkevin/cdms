using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IRepository<Model.Purchase> _Repository;

        public PurchaseService(IUnitOfWork unitofwork, IRepository<Model.Purchase> repository)
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
        }

        private string GenerateInquiryID(Purchase info)
        {            
            int seq = 1;
            string result = "";

            string key = $"P{DateTime.Today.ToString("yyMMdd")}";

            var current =
                this._Repository.GetAll()
                .Where(x => x.PurchaseID.StartsWith(key))
                .OrderByDescending(x => x.PurchaseID)
                .FirstOrDefault();

            if (current != null)
            {
                // 移除前面日期部分留下流水號
                seq = Convert.ToInt16(current.PurchaseID.Replace(key, ""));
                seq += 1;
            }

            result = $"{key}{seq.ToString().PadLeft(GlobalSettings.SequenceLength, '0')}";
            return result;
        }

        private Model.Purchase GetInfoOnCreate(Purchase info)
        {
            // 取得產品號;目前由使用者自型輸入
            info.PurchaseID = GenerateInquiryID(info);
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Model.Purchase GetInfoOnUpdate(Purchase info)
        {
            Purchase query = this.Get(info.PurchaseID);

            query.LastPerson = IdentityService.GetUserData().UserID;
            query.LastUpdate = DateTime.Now;
            return query;
        }

        public void Create(Purchase model)
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

        public void Update(Purchase info)
        {
            #region 取資料
            Purchase query = GetInfoOnUpdate(info);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            //query.CX_Observation = model.CX_Observation;
            //query.NQ_Sort = model.NQ_Sort;
            //query.ID_Feedback = model.ID_Feedback;
            //query.CX_Observation_Remarks = model.CX_Observation_Remarks;
            #endregion

            #region Models資料庫
            this._Repository.Update(query);
            this._UnitOfWork.SaveChange();
            #endregion
        }

        public void Delete(Purchase model)
        {
            #region 取資料
            Model.Purchase query = this.Get(model.PurchaseID);
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
            this._Repository.Delete(query);
            this._UnitOfWork.SaveChange();
            #endregion
        }

        public Purchase Get(string id)
        {
            return this._Repository.Get(x => x.PurchaseID == id);
        }

        public IQueryable<Purchase> GetAll()
        {
            return this._Repository.GetAll();
        }
    }
}
