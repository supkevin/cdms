using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;

namespace CDMS.Service
{
    public class CountryService : ICountryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Country> _repository;

        private readonly IOverSeaService _overseaService;

        public CountryService(IUnitOfWork unitofwork, IRepository<Country> repository, IOverSeaService overseaService)
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;

            this._overseaService = overseaService;
        }

        public void Create(Country model)
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

        public void Update(Country model)
        {
            #region 取資料
            Country query = this.Get(model.ID_Country);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            query.CX_Country = model.CX_Country;
            query.NQ_Sort = model.NQ_Sort;
            query.CX_Country_Remarks = model.CX_Country_Remarks;

            query.CX_Send_To_Mail = model.CX_Send_To_Mail;
            #endregion

            #region Models資料庫
            this._repository.Update(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public void Delete(Country model)
        {
            #region 取資料
            Country query = this.Get(model.ID_Country);
            var queryoverseastaff = this._overseaService.GetForCountry(query.ID_Country);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());

            //驗證
            if (queryoverseastaff == null)//沒有資料
                throw new Exception("MessageDataHasLinking".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            this._repository.Delete(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public Country Get(int id_country)
        {
            return this._repository.Get(x => x.ID_Country == id_country);
        }

        public Country GetById(int id_country)
        {
            #region 取資料
            Country query = this.Get(id_country);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 回傳

            return query;
            #endregion
        }

        public IEnumerable<Country> GetAll()
        {
            return this._repository.GetAll().IncludeMultiple(x => x.Store, x => x.OverSea);            
        }



    }
}
