using CDMS.Language;
using CDMS.Model;
using CDMS.Model.ViewModel;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Infrastructure;
using AutoMapper;

namespace CDMS.Service
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IRepository<Model.Company> _Repository;

        private readonly UserInfo _CurrentUser = IdentityService.GetUserData();

        public CompanyService(IUnitOfWork unitofwork, IRepository<Model.Company> repository)
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
        }
     
        private Model.Company GetInfoOnCreate(Company info) {
            info.LastPerson =_CurrentUser.UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Model.Company GetInfoOnDelete(Company info)
        {
            info.Activate = YesNo.No.Value; 
            info.LastPerson = _CurrentUser.UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Model.Company GetInfoOnUpdate(Company info)
        {
            Company query = this.Get(info.CompanyID);

            // 這裡填要塞的資料   
            query.FullName = info.FullName;
            query.ShortName = info.ShortName;
            query.CompanyKindID = info.CompanyKindID;
            query.TaxID = info.TaxID;
            query.ContactPerson = info.ContactPerson;
            query.Telephone1 = info.Telephone1;
            query.Telephone2 = info.Telephone2;
            query.Fax = info.Fax;
            query.Clerk = info.Clerk;
            query.ClerkMobile = info.ClerkMobile;
            query.Address = info.Address;
            query.InvoiceAddress = info.InvoiceAddress;
            query.ShippingAddress = info.ShippingAddress;
            query.FactoryAddress = info.FactoryAddress;
            query.NextMonth = info.NextMonth;
            query.Remarks = info.Remarks;
            query.Activate = info.Activate;
            query.LastPerson =_CurrentUser.UserID;
            query.LastUpdate = DateTime.Now;
            return query;
        }

        public void Create(Company info)
        {
            #region 取資料
            info = GetInfoOnCreate(info);
            #endregion

            #region 邏輯驗證
            if (this.IsDataExists(info))
            {
                throw new Exception($"{"CompanySupplier".ToLocalized()}:{info.CompanyID} 已經存在！");
            }
            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            this._Repository.Create(info);
            this._UnitOfWork.SaveChange();
            #endregion
        }

        public void Update(Company info)
        {
            #region 取資料
            Company query = GetInfoOnUpdate(info);
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
       
        public void Delete(Company info)
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

        public Company Get(string id)
        {
            return this._Repository.Get(x => x.CompanyID == id);
        }

        public IQueryable<CompanyViewModel> GetAll()
        {
            var query = this._Repository.GetAll()
                        .Select(x => new CompanyViewModel
                        {
                            CompanyID = x.CompanyID,
                            FullName = x.FullName,
                            ShortName = x.ShortName,
                            CompanyKindID = x.CompanyKindID,
                            TaxID = x.TaxID,
                            ContactPerson = x.ContactPerson,
                            Telephone1 = x.Telephone1,
                            Telephone2 = x.Telephone2,
                            Fax = x.Fax,
                            Clerk = x.Clerk,
                            ClerkMobile = x.ClerkMobile,
                            Address = x.Address,
                            InvoiceAddress = x.InvoiceAddress,
                            ShippingAddress = x.ShippingAddress,
                            FactoryAddress = x.FactoryAddress,
                            NextMonth = x.NextMonth,
                            Remarks = x.Remarks,
                            Activate = x.Activate

                        });

            return query; 
            //return this._Repository.GetAll()
            //    .Select(x => Mapper.Map<CompanyViewModel>(x));          
        }

        public bool IsDataExists(Company info )
        {
            var query = this._Repository.Get(x => x.CompanyID == info.CompanyID);
            return (query != null);
        }

        public List<CompanyViewModel> GetForAutoComplete(string term, int count = 10 )
        {
            var query = GetAll()
                .Where(
                        x => x.Activate == YesNo.Yes.Value
                        && (x.CompanyID.Contains(term) || string.IsNullOrEmpty(term))
                      )
                .Take(count);

            return query.ToList();
        }


        public bool IsUsed(Company info)
        {
            var query = this._Repository.Get(x => x.CompanyID == info.CompanyID);
            var result = query.Product.Any();
            this._Repository.HandleDetached(query); // 
            return result;            
        }       
    }
}
