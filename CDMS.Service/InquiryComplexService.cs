using CDMS.Language;
using CDMS.Model;
using CDMS.Model.ViewModel;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CDMS.Service.Common;

namespace CDMS.Service
{
    public class InquiryComplexService : IInquiryComplexService
    {
        private readonly IUnitOfWork _UnitOfWork;

        private readonly IRepository<Model.Inquiry> _Repository;
        private readonly IRepository<Model.InquiryDetail> _DetailRepository;
        private readonly IRepository<Model.Company> _Company;
        private readonly IRepository<Model.Product> _Product;
        private readonly IRepository<Model.News> _News;

        private readonly UserInfo _CurrentUser = IdentityService.GetUserData();

        public InquiryComplexService(
            IUnitOfWork unitofwork,
            IRepository<Model.Inquiry> repository,
            IRepository<Model.InquiryDetail> detailRepository,
            IRepository<Model.Company> company,
            IRepository<Model.Product> product,
            IRepository<Model.News> news
            )
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
            this._DetailRepository = detailRepository;
            this._Company = company;
            this._Product = product;
            this._News = news;
        }

        // 取號
        private string GenerateInquiryID(Inquiry info)
        {
            int seq = 1;
            string result = "";

            string key = $"I{DateTime.Today.ToString("yyMMdd")}";

            var current =
                this._Repository.GetAll()
                .Where(x => x.InquiryID.StartsWith(key))
                .OrderByDescending(x => x.InquiryID)
                .FirstOrDefault();

            if (current != null)
            {
                // 移除前面日期部分留下流水號
                seq = Convert.ToInt16(current.InquiryID.Replace(key, ""));
                seq += 1;
            }

            result = $"{key}{seq.ToString().PadLeft(GlobalSettings.SequenceLength, '0')}";
            return result;
        }

        private Inquiry GetInquiryOnCreate(InquiryComplex source)
        {
            Inquiry info = Mapper.Map<Inquiry>(source.Inquiry);

            // 取得詢價單號;目前由使用者自型輸入
            info.InquiryID = GenerateInquiryID(info);
            info.Activate = YesNo.Yes.Value;
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Inquiry GetInquiryOnUpdate(InquiryComplex source)
        {
            Inquiry info = Mapper.Map<Inquiry>(source.Inquiry);

            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private Inquiry GetInquiryOnDelete(InquiryComplex source)
        {
            Inquiry info = Mapper.Map<Inquiry>(source.Inquiry);
            info.Activate = YesNo.No.Value;
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        private List<InquiryDetail> GetChildOnCreate(Inquiry master, InquiryComplex source)
        {
            List<InquiryDetail> infos = new List<InquiryDetail>();
            var wanted = source.ChildList.Where(x => x.IsDirty == true);

            foreach (var item in wanted)
            {
                InquiryDetail temp = Mapper.Map<InquiryDetail>(item);
                temp.InquiryID = master.InquiryID;
                temp.LastPerson = IdentityService.GetUserData().UserID;
                temp.LastUpdate = DateTime.Now;
                infos.Add(temp);
            }
            return infos;
        }

        private void GenerateNews(Inquiry info)
        {
            //預定到貨前提醒：若不等於0天，存檔時，於「最新消息資料表」新增一筆資料，
            //發布單位：「系統自動提醒」，發布日期：「預定到貨日期 - 預定到貨前提醒天數」，
            //下架日期：「預定到貨日期 + 5天」，消息內容：「供應商名稱 + 詢價單編號 +“的訂購貨品，預定於”+預定到貨日期 +“到貨。”」

            var delete = this._News.Get(x => x.SourceID == info.InquiryID);
            //var user = IdentityService.GetUserData();

            if (delete != null)
            {
                delete.Activate = YesNo.No.Value;
                delete.LastPerson = _CurrentUser.UserID;
                delete.LastUpdate = DateTime.Now;

                this._News.Update(delete);
            }

            // 有預定到貨日期非刪除才處理
            if (!info.ScheduleDate.HasValue 
                || info.Remind == 0 
                || info.Activate != YesNo.Yes.Value) return;

            var company = this._Company.Get(x => x.CompanyID == info.CompanyID);

            if (company == null)
            {
                throw new Exception($"詢價單廠商代碼錯誤，代碼：{info.CompanyID}");
            }
            
            var news = new News
            {
                NewsTypeID = GlobalSettings.Notification,
                Content = $"{company.ShortName}-{ info.InquiryID } 的訂購貨品，預定於{string.Format("{0:yyyy-MM-dd}", info.ScheduleDate)}到貨。",
                ReleaseDate = info.ScheduleDate.Value.AddDays(-1 * info.Remind ?? 0),
                OffDate = info.ScheduleDate.Value.AddDays(GlobalSettings.PreserveDays),
                SourceID = info.InquiryID,
                SetTop = YesNo.No.Value,
                DepartmentID = _CurrentUser.DepartmentID,
                LastPerson = _CurrentUser.UserID,
                LastUpdate = DateTime.Now
            };

            this._News.Create(news);
        }

        public InquiryComplex Create(InquiryComplex source)
        {
            #region 取資料


            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料
            Inquiry main = GetInquiryOnCreate(source);

            List<InquiryDetail> children = GetChildOnCreate(main, source);
            #endregion

            #region Models資料庫

            this._Repository.Create(main);

            foreach (InquiryDetail item in children)
            {
                this._DetailRepository.Create(item);
            }

            GenerateNews(main); // 產生到貨消息

            this._UnitOfWork.SaveChange();
            #endregion

            return this.Get(main.InquiryID);
        }

        public void Update(InquiryComplex source)
        {
            #region 取資料


            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料
            Inquiry main = GetInquiryOnUpdate(source);

            List<InquiryDetail> children = GetChildOnCreate(main, source);
            #endregion

            #region Models資料庫

            this._Repository.Update(main);

            foreach (InquiryDetail item in children)
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

            GenerateNews(main); // 產生到貨消息

            this._UnitOfWork.SaveChange();
            #endregion
        }

        public void Delete(InquiryComplex model)
        {           
            #region 邏輯驗證
            if (model == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            var info = Mapper.Map<Inquiry>(model.Inquiry);
            #endregion

            #region Models資料庫 

            foreach (InquiryDetailViewModel t in model.ChildList)
            {
                var d = Mapper.Map<InquiryDetail>(t);
                _DetailRepository.Delete(d);
            }             
                  
            this._Repository.Delete(info);
            this._UnitOfWork.SaveChange();
            #endregion        
        }

        public void RemoveChild(long id)
        {
            #region 取資料
            InquiryDetail query = this._DetailRepository.Get(x => x.SeqNo == id);
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


        public InquiryComplex Get(string id)
        {
            InquiryComplex info = new InquiryComplex();
            info = this.GetAll().Where(x => x.Inquiry.InquiryID == id).Single();

            return info;
        }

        public IQueryable<InquiryComplex> GetAll()
        {
            var query1 = from u in this._Repository.GetAll()
                         join p in this._Company.GetAll() on u.CompanyID equals p.CompanyID into g
                         from p in g.DefaultIfEmpty()
                         select new InquiryViewModel()
                         {
                             InquiryID = u.InquiryID,
                             InquiryDate = u.InquiryDate,
                             ContactPerson = u.ContactPerson,
                             ContactPhone = u.ContactPhone,
                             CompanyID = u.CompanyID,
                             CurrencyID = u.CurrencyID,
                             ExchangeRate = u.ExchangeRate,
                             Remarks = u.Remarks,
                             ValidateDate = u.ValidateDate,
                             ScheduleDate = u.ScheduleDate,
                             Remind = u.Remind,
                             PurchaseID = u.PurchaseID,
                             PurchaseDate = u.PurchaseDate,
                             Activate = u.Activate,
                             CompanyName = p == null ? "" : p.ShortName
                         };

            var query2 =
              from u in this._DetailRepository.GetAll()
              join p in this._Product.GetAll() on u.ProductID equals p.ProductID into g
              from p in g.DefaultIfEmpty()
              select new InquiryDetailViewModel()
              {
                  SeqNo = u.SeqNo,
                  InquiryID = u.InquiryID,
                  ProductID = u.ProductID,
                  PriceKindID = u.PriceKindID,
                  ConditionID = u.ConditionID,
                  Discount = u.Discount,
                  ForeignPrice = u.ForeignPrice,
                  Price = u.Price,
                  Qty = u.Qty,
                  Amount = u.Amount,
                  Remarks = u.Remarks,
                  OriginalPrice = u.OriginalPrice,
                  ProductName = p == null ? "" : p.ProductName
              };

            var query =
                 from u in query1
                 select new InquiryComplex()
                 {
                     Inquiry = u,
                     ChildList = (
                                 from p in query2
                                 where p.InquiryID == u.InquiryID
                                 select p
                                 ).ToList()
                 };

            //var query =
            //    from u in query1
            //    join p in query3 on u.InquiryID equals p.Key into g 
            //    from p in g
            //    select new InquiryComplex()
            //    {
            //        Inquiry = u,
            //        ChildList = p.Items,
            //    };

            return query;
        }

        public bool IsUsed(InquiryComplex info)
        {
            //var query = this._Repository.Get(x => x.InquiryID == model.Inquiry.InquiryID);
            //var result = query.Product.Any();
            //this._Repository.HandleDetached(query); // 
            //return result;
            return true; // 目前不知關聯到哪些資料表 
        }

        public InquiryHistoryViewModel GetHistory(string productID)
        {
            var temp = from u in _DetailRepository.GetAll().Where(x => x.ProductID == productID)
                       join m in _Repository.GetAll() on u.InquiryID equals m.InquiryID into mm
                       from m in mm.DefaultIfEmpty()
                       orderby m.InquiryDate descending
                       select new
                       {
                           u.ProductID,
                           u.Price,
                           m.InquiryDate
                       };

            var procuct = _Product.Get(x => x.ProductID == productID);

            if (null == procuct) throw new NullReferenceException($"找不到產品,產品編號: { productID } !");

            var query = new InquiryHistoryViewModel()
            {
                ProductID = procuct.ProductID,
                ProductName = procuct.ProductName,
                Inquiry1 = "Inquiry1",
                Inquiry2 = "Inquiry2",
                Inquiry3 = "Inquiry3"
            };

            return query;
        }
    }
}
