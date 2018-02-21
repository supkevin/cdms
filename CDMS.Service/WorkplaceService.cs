using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
namespace CDMS.Service
{
    public class WorkplaceService : IWorkplaceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.Workplace> _repository;
        public WorkplaceService(IUnitOfWork unitofwork, IRepository<Model.Workplace> repository)
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;
        }

        public void Create(Workplace model)
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

        public void Update(Workplace model)
        {
            #region 取資料
            Workplace query = this.Get(model.ID_Workplace);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            query.CX_Workplace = model.CX_Workplace;
            query.NQ_Sort = model.NQ_Sort;
            query.CX_Workplace_Remarks = model.CX_Workplace_Remarks;
            query.CX_Color = model.CX_Color;
            #endregion

            #region Models資料庫
            this._repository.Update(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public void Delete(Workplace model)
        {
            #region 取資料
            Model.Workplace query = this.Get(model.ID_Workplace);
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

        public Workplace Get(int id_workplace)
        {
            return this._repository.Get(x => x.ID_Workplace == id_workplace);
        }

        public Workplace GetById(int id_workplace)
        {
            #region 取資料
            Workplace query = this.Get(id_workplace);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 回傳

            return query;
            #endregion
        }

        public IEnumerable<Workplace> GetAll()
        {
            return this._repository.GetAll();
        }
    }
}
