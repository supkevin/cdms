using CDMS.Language;
using CDMS.Model;
using CDMS.Model.ViewModel;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace CDMS.Service
{
    public class StockChangeComplexService : IStockChangeComplexService
    {
        private readonly IUnitOfWork _UnitOfWork;

        private readonly IRepository<Model.StockChange> _Repository;        
        private readonly IRepository<Model.StockChangeDetail> _DetailRepository;
        private readonly IRepository<Model.Product> _Product;
        private readonly IRepository<Model.User> _User;

        public StockChangeComplexService(
            IUnitOfWork unitofwork,
            IRepository<Model.StockChange> repository,
            IRepository<Model.StockChangeDetail> detailRepository,            
            IRepository<Model.Product> product,
            IRepository<Model.User> user
            )
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
            this._DetailRepository = detailRepository;            
            this._Product = product;
            this._User= user;
        }

        private string GenerateKey(StockChange info)
        {
            int seq = 1;
            string result = "";

            string key = $"C{DateTime.Today.ToString(GlobalSettings.KEY_DATE_FORMAT)}";

            var current =
                this._Repository.GetAll()
                .Where(x => x.StockChangeID.StartsWith(key))
                .OrderByDescending(x => x.StockChangeID)
                .FirstOrDefault();

            if (current != null)
            {
                // 移除前面日期部分留下流水號
                seq = Convert.ToInt16(current.StockChangeID.Replace(key, ""));
                seq += 1;
            }

            result = $"{key}{seq.ToString().PadLeft(GlobalSettings.SequenceLength, '0')}";
            return result;
        }

        private StockChange GetInfoOnCreate(StockChangeComplex source)
        {
            StockChange info = Mapper.Map<StockChange>(source.StockChange);

            // 取得詢價單號;目前由使用者自型輸入
            info.StockChangeID = GenerateKey(info);
            //info.PostingTime = DateTime.Today; 
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private StockChange GetInfoOnUpdate(StockChangeComplex source)
        {
            StockChange info = Mapper.Map<StockChange>(source.StockChange);

            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private List<StockChangeDetail> GetChildOnCreate(StockChange master, StockChangeComplex source)
        {
            List<StockChangeDetail> infos = new List<StockChangeDetail>();
            var wanted = source.ChildList.Where(x => x.IsDirty == true);

            foreach (var item in wanted)
            {
                StockChangeDetail temp = Mapper.Map<StockChangeDetail>(item);
                temp.StockChangeID = master.StockChangeID;
                temp.LastPerson = IdentityService.GetUserData().UserID;
                temp.LastUpdate = DateTime.Now;
                infos.Add(temp);
            }
            return infos;
        }

        public StockChangeComplex Create(StockChangeComplex source)
        {        
            #region 變為Models需要之型別及邏輯資料
            StockChange main = GetInfoOnCreate(source);

            List<StockChangeDetail> children = GetChildOnCreate(main, source);
            #endregion

            #region Models資料庫

            this._Repository.Create(main);

            foreach (StockChangeDetail item in children)
            {
                this._DetailRepository.Create(item);
            }

            this._UnitOfWork.SaveChange();
            #endregion

            return this.Get(main.StockChangeID);
        }

        public void Update(StockChangeComplex source)
        {
            #region 取資料


            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料
            StockChange main = GetInfoOnUpdate(source);

            List<StockChangeDetail> children = GetChildOnCreate(main, source);
            #endregion

            #region Models資料庫

            this._Repository.Update(main);

            foreach (StockChangeDetail item in children)
            {
                if (item.SeqNo == 0)
                {
                    this._DetailRepository.Create(item);
                }
                else
                {
                    this._DetailRepository.Update(item);
                }
            }

            this._UnitOfWork.SaveChange();
            #endregion
        }

        public void Delete(StockChangeComplex model)
        {
            #region 邏輯驗證
            if (model == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            var info = Mapper.Map<StockChange>(model.StockChange);
            #endregion

            #region Models資料庫 

            foreach (StockChangeDetailViewModel t in model.ChildList)
            {
                var d = Mapper.Map<StockChangeDetail>(t);
                _DetailRepository.Delete(d);
            }

            this._Repository.Delete(info);
            this._UnitOfWork.SaveChange();
            #endregion        
        }

        public void RemoveChild(long id)
        {
            #region 取資料
            StockChangeDetail query = this._DetailRepository.Get(x => x.SeqNo == id);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());

            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            this._DetailRepository.Delete(query);
            this._UnitOfWork.SaveChange();
            #endregion
        }

        public StockChangeComplex Get(string id)
        {
            StockChangeComplex info = new StockChangeComplex();

            info = this.GetAll().Where(x => x.StockChange.StockChangeID == id).Single();
            return info;
        }

        public IQueryable<StockChangeComplex> GetAll()
        {
            var query1 = from u in this._Repository.GetAll()
                         join p in this._User.GetAll() on u.ChangePersonID equals p.UserID into g
                         from p in g.DefaultIfEmpty()
                         select new StockChangeViewModel()
                         {
                             StockChangeID = u.StockChangeID,
                             ChangeReasonID = u.ChangeReasonID,
                             WarehouseOldID = u.WarehouseOldID,
                             WarehouseNewID = u.WarehouseNewID,
                             ChangePersonID = u.ChangePersonID,
                             ChangeDate = u.ChangeDate,
                             Remarks = u.Remarks,
                             ChangePersonName = p.UserName
                         };

            var query2 =
              from u in this._DetailRepository.GetAll()
              join p in this._Product.GetAll() on u.ProductID equals p.ProductID into g
              from p in g.DefaultIfEmpty()
              select new StockChangeDetailViewModel()
              {
                  SeqNo = u.SeqNo,
                  StockChangeID = u.StockChangeID,
                  ProductID = u.ProductID,               
                  Qty = u.Qty,                  
                  Remarks = u.Remarks,                  
                  ProductName = (p == null) ? "" : p.ProductName
              };

            var query =
                 from u in query1
                 select new StockChangeComplex()
                 {
                     StockChange = u,
                     ChildList = (
                                 from p in query2
                                 where p.StockChangeID == u.StockChangeID
                                 select p
                                 ).ToList()
                 };

            return query;
        }


        public IQueryable<StockChangeListViewModel> GetListView()
        {
            var query = from u in this._Repository.GetAll()
                        join t in this._DetailRepository.GetAll() on u.StockChangeID equals t.StockChangeID into tt
                        from t in tt
                        join p in this._Product.GetAll() on t.ProductID equals p.ProductID into pp
                        from p in pp.DefaultIfEmpty()
                        join v in this._User.GetAll() on u.ChangePersonID equals v.UserID into vv
                        from v in vv.DefaultIfEmpty()
                        select new StockChangeListViewModel
                        {
                            ChangeDate = u.ChangeDate,
                            StockChangeID = u.StockChangeID,
                            ChangeReasonID = u.ChangeReasonID,
                            ChangePersonID = u.ChangePersonID,
                            WarehouseOldID = u.WarehouseOldID,
                            WarehouseNewID = u.WarehouseNewID,
                            ProductID = t == null ? "" : t.ProductID,
                            ProductName = p == null ? "" : p.ProductName,
                            Qty = t == null ? 0 : t.Qty ?? 0,
                            ChangePersonName = v== null ? "" : v.UserName,
                            Remarks = u.Remarks
                        };

            return query;

        }

        public bool IsUsed(StockChangeComplex info)
        {
            //var query = this._Repository.Get(x => x.InquiryID == model.Inquiry.InquiryID);
            //var result = query.Product.Any();
            //this._Repository.HandleDetached(query); // 
            //return result;
            return true; // 目前不知關聯到哪些資料表 
        }
    }
}
