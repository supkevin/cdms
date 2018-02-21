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
    public class AlternativeService : IAlternativeService
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IRepository<Model.Alternative> _Repository;
        private readonly IRepository<Model.Product> _Product;

        public AlternativeService(IUnitOfWork unitofwork, 
            IRepository<Model.Alternative> repository,
            IRepository<Model.Product> product
            )
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
            this._Product = product;
        }
     
        private Model.Alternative GetInfoOnCreate(Alternative info) {            
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Model.Alternative GetInfoOnUpdate(Alternative info)
        {
            Alternative query = this.Get(info.SeqNo);                                
            // 這裡填要塞的資料                        
            query.AlternativeID = info.AlternativeID; 
            query.Remarks = info.Remarks;
            query.AlternativeCount = info.AlternativeCount;
            query.LastPerson = IdentityService.GetUserData().UserID;
            query.LastUpdate = DateTime.Now;
            return query;
        }

        public void Create(Alternative model)
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

        public void Update(Alternative info)
        {
            #region 取資料
            Alternative query = GetInfoOnUpdate(info);
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

        public void Delete(Alternative info)
        {
            #region 取資料
            Model.Alternative query = this.Get(info.SeqNo);            
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

        public Alternative Get(long id)
        {
            return this._Repository.Get(x => x.SeqNo == id);
        }

        public IEnumerable<Alternative> GetAll()
        {
            return this._Repository.GetAll();
        }

        public IQueryable<AlternativeListViewModel> GetListView()
        {
            var query = from u in this._Repository.GetAll()
                        join p in this._Product.GetAll() on u.ProductID equals p.ProductID into pp
                        from p in pp.DefaultIfEmpty()
                        join t in this._Product.GetAll() on u.AlternativeID equals t.ProductID into tt
                        from t in tt.DefaultIfEmpty()
                        select new AlternativeListViewModel
                        {
                            ProductID = u.ProductID,
                            AlternativeID = u.AlternativeID,
                            AlternativeCount = u.AlternativeCount,
                            Remarks = u.Remarks,
                            ProductName = (p == null) ? "" : p.ProductName,
                            AlternativeName = (t == null) ? "" : t.ProductName
                        };

            return query;

        }
    }
}
