using CDMS.Language;
using CDMS.Model;
using CDMS.Model.ViewModel;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IRepository<Model.User> _Repository;
        private readonly IRepository<Model.Code> _Code;

        public UserService(IUnitOfWork unitofwork,
            IRepository<Model.User> repository, IRepository<Model.Code> code
            )
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
            this._Code = code;
        }
     
        private Model.User GetInfoOnCreate(User info) {
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Model.User GetInfoOnUpdate(User info)
        {
            User query = this.Get(info.UserID);
            // 這裡填要塞的資料   
            query.UserName = info.UserName;
            query.TitleID = info.TitleID;
            query.OnboardDate = info.OnboardDate;
            query.PermissionID = info.PermissionID;
            query.SupervisorID = info.SupervisorID;
            query.DepartmentID = info.DepartmentID;
            query.Telephone = info.Telephone;
            query.Address = info.Address;
            query.Email = info.Email;
            query.AnnualTarget = info.AnnualTarget;
            query.QuotationLevelID = info.QuotationLevelID;
            query.Password = info.Password;
            query.BeginDate = info.BeginDate;
            query.EndDate = info.EndDate;
            query.Remarks = info.Remarks;
            query.Activate = info.Activate;
            query.LastPerson = IdentityService.GetUserData().UserID;
            query.LastUpdate = DateTime.Now;
            return query;
        }

        public void Create(User info)
        {
            #region 取資料
            info = GetInfoOnCreate(info);
            #endregion

            #region 邏輯驗證
            if (this.IsDataExists(info))
            {
                throw new Exception($"{"UserID".ToLocalized()}:{info.UserID} 已經存在！");
            }
            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            this._Repository.Create(info);
            this._UnitOfWork.SaveChange();
            #endregion
        }

        public void Update(User info)
        {
            #region 取資料
            User query = GetInfoOnUpdate(info);
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

        public void Delete(User info)
        {
            #region 取資料
            Model.User query = this.Get(info.UserID);            
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

        public User Get(string id)
        {
            return this._Repository.Get(x => x.UserID == id);
        }

        public IQueryable<UserViewModel> GetAll()
        {
            //return this._Repository.GetAll();
            var query = from u in this._Repository.GetAll()                   
                        select new UserViewModel
                        {
                            UserID = u.UserID,
                            UserName = u.UserName,
                            TitleID = u.TitleID,
                            OnboardDate = u.OnboardDate,
                            PermissionID = u.PermissionID,
                            SupervisorID = u.SupervisorID,
                            DepartmentID = u.DepartmentID,
                            Telephone = u.Telephone,
                            Address = u.Address,
                            Email = u.Email,
                            AnnualTarget = u.AnnualTarget,
                            QuotationLevelID = u.QuotationLevelID,
                            Password = u.Password,
                            BeginDate = u.BeginDate,
                            EndDate = u.EndDate,
                            Remarks = u.Remarks,
                            Activate = u.Activate
                        };

            return query;
        }

        public bool IsDataExists(User info )
        {
            var query = this._Repository.Get(x => x.UserID == info.UserID);
            return (query != null);
        }

        public bool IsUsed(User info)
        {
            var query = this._Repository.Get(x => x.UserID == info.UserID);
            var result = query.Product.Any();
            this._Repository.HandleDetached(query); // 
            return result;
        }

        public List<UserViewModel> GetForAutoComplete(string term, int count = 10)
        {
            return this.GetAll()
                .Where(x => (x.EndDate == null || x.EndDate > DateTime.Today)
                            && x.UserID.Contains(term))
                .Take(count)
                .ToList();
        }

        public IQueryable<UserListViewModel> GetListView()
        {
            var query = from u in this._Repository.GetAll()
                        join t in this._Code.GetAll()
                            .Where(x=> x.CodeType == CodeType.Title.Value) 
                            on u.TitleID equals t.CodeValue into tt
                        from t in tt.DefaultIfEmpty()
                        select new UserListViewModel
                        {
                            UserID = u.UserID,
                            UserName = u.UserName,
                            TitleID = u.TitleID,
                            OnboardDate = u.OnboardDate,
                            PermissionID = u.PermissionID,
                            SupervisorID = u.SupervisorID,
                            Telephone = u.Telephone,
                            Address = u.Address,
                            Email = u.Email,
                            AnnualTarget = u.AnnualTarget,
                            QuotationLevelID = u.QuotationLevelID,
                            Password = u.Password,
                            BeginDate = u.BeginDate,
                            EndDate = u.EndDate,
                            Remarks = u.Remarks,
                            Activate = u.Activate,
                            TitleName = (t== null) ? "" : t.CodeName
                        };

            return query;

        }
    }
}
