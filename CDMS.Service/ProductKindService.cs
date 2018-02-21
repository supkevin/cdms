using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public class ProductKindService : IProductKindService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.Code> _repository;

        public ProductKindService(IUnitOfWork unitofwork, IRepository<Model.Code> repository)
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;
        }

        //public void Create(vProductKind model)
        //{
        //    #region 取資料

        //    #endregion

        //    #region 邏輯驗證


        //    #endregion

        //    #region 變為Models需要之型別及邏輯資料

        //    #endregion

        //    #region Models資料庫
        //    this._Repository.Create(model);
        //    this._UnitOfWork.SaveChange();
        //    #endregion
        //}

        //public void Update(Code model)
        //{
        //    #region 取資料
        //    Code query = this.Get(model.CodeValue);
        //    #endregion

        //    #region 邏輯驗證
        //    if (query == null)//沒有資料
        //        throw new Exception("MessageNoData".ToLocalized());
        //    #endregion

        //    #region 變為Models需要之型別及邏輯資料
        //    //query.CX_Observation = model.CX_Observation;
        //    //query.NQ_Sort = model.NQ_Sort;
        //    //query.ID_Feedback = model.ID_Feedback;
        //    //query.CX_Observation_Remarks = model.CX_Observation_Remarks;
        //    #endregion

        //    #region Models資料庫
        //    this._Repository.Update(query);
        //    this._UnitOfWork.SaveChange();
        //    #endregion
        //}

        //public void Delete(Code model)
        //{
        //    #region 取資料
        //    Model.Code query = this.Get(model.CodeValue);
        //    //var queryoverseastaff = this._overseaService.GetForOverType(query.ID_OverType);
        //    #endregion

        //    #region 邏輯驗證
        //    if (query == null)//沒有資料
        //        throw new Exception("MessageNoData".ToLocalized());

        //    //驗證
        //    //if (queryoverseastaff == null)//沒有資料
        //    //    throw new Exception("MessageDataHasLinking".ToLocalized());
        //    #endregion

        //    #region 變為Models需要之型別及邏輯資料

        //    #endregion

        //    #region Models資料庫
        //    this._Repository.Delete(query);
        //    this._UnitOfWork.SaveChange();
        //    #endregion
        //}

        public Code Get(string id)
        {
            return this._repository.Get(x => x.CodeValue == id);
        }

        public IEnumerable<Code> GetAll()
        {
            return this._repository.GetAll();
        }

        public List<Code> GetForSelect(string id)
        {
            return this._repository.GetAll().Where(x => x.CodeValue == id).ToList();
        }
    }
}
