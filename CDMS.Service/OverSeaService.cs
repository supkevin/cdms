using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public class OverSeaService : IOverSeaService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.OverSea> _repository;

        public OverSeaService(IUnitOfWork unitofwork, IRepository<Model.OverSea> repository)
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;
        }

        public void Create(Model.OverSea model)
        {
            #region 取資料

            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料

            if (model.CX_Dept_Name.Trim().Equals("總經理室"))
            {
                model.CX_Dept_Name_Short = model.CX_Dept_Name;
            }
            else
            {
                model.CX_Dept_Name_Short = model.CX_Dept_Name.Substring(0, 2) + "TextTeam".ToLocalized();//變成料理組 點心組 餐飲組
            }


            #endregion

            #region Models資料庫
            this._repository.Create(model);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public void BatchCreate(List<OverSea> model)
        {
            #region 取資料

            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料
            foreach (var item in model)
            {
                item.CX_Dept_Name_Short = item.CX_Dept_Name.Substring(0, 2) + "TextTeam".ToLocalized();//變成料理組 點心組 餐飲組
            }
            #endregion

            #region Models資料庫
            foreach (var item in model)
            {
                this._repository.Create(item);
            }


            this._unitOfWork.SaveChange();
            #endregion
        }

        public void Update(Model.OverSea model)
        {
            #region 取資料
            OverSea query = this.Get(model.ID_OverSea);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            query.ID_Country = model.ID_Country;
            query.CX_From_Date = model.CX_From_Date;
            query.CX_To_Date = model.CX_To_Date;
            query.ID_OverType = model.ID_OverType;
            query.CX_OverSea_Remark = model.CX_OverSea_Remark;
            query.CX_Place_Remark = model.CX_Place_Remark;
            query.FG_IsWorkCard = model.FG_IsWorkCard;
            query.NQ_MeetBirthday = model.NQ_MeetBirthday;
            query.CX_Modify = model.CX_Modify;
            query.DT_Modify = model.DT_Modify;
            //2017-09-11 編輯時也要
            query.NQ_MeetBirthday = model.NQ_MeetBirthday;
            #endregion

            #region Models資料庫
            this._repository.Update(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public void Delete(Model.OverSea model)
        {
            #region 取資料
            Model.OverSea query = this.Get(model.ID_OverSea);
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

        public Model.OverSea Get(int id_oversea)
        {
            return this._repository.Get(x => x.ID_OverSea == id_oversea);
        }

        public Model.OverSea GetById(int id_oversea)
        {
            #region 取資料
            OverSea query = this.Get(id_oversea);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 回傳

            return query;
            #endregion
        }

        public IEnumerable<Model.OverSea> GetAll()
        {
            return this._repository.GetAll().IncludeMultiple(
                x => x.Country,
                x => x.OverType);
        }


        public IEnumerable<OverSea> GetForOverType(int id_overtype)
        {
            return this._repository.GetAll().Where(x => x.ID_OverType == id_overtype);
        }

        public IEnumerable<OverSea> GetForCountry(int id_country)
        {
            return this._repository.GetAll().Where(x => x.ID_Country == id_country);
        }


        public OverSea GetForInspection(string cx_pid, string cx_date)
        {
            return this._repository.GetAll().Where(
                    x => x.CX_PID.Equals(cx_pid) &&
                    x.CX_From_Date.CompareTo(cx_date) <= 0 &&
                    x.CX_To_Date.CompareTo(cx_date) >= 0).FirstOrDefault();
        }
    }
}
