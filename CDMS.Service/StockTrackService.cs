using CDMS.Language;
using CDMS.Model;
using CDMS.Model.ViewModel;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace CDMS.Service
{
    public class StockTrackService : IStockTrackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.Stock> _Repository;
        private readonly IRepository<Model.Product> _Product;
        private readonly IRepository<Model.Code> _Code;

        private readonly IRepository<Model.v_StockTrack> _StockTrack;
        private readonly IRepository<Model.v_Stock> _Stock;

        public StockTrackService(IUnitOfWork unitofwork,
            IRepository<Model.Stock> repository,
            IRepository<Model.Product> product,
            IRepository<Model.Code> code,
            IRepository<Model.v_StockTrack> stockTrack,
            IRepository<Model.v_Stock> stock
            )
        {
            this._unitOfWork = unitofwork;
            this._Repository = repository;
            this._Product = product;
            this._Code = code;
            this._StockTrack = stockTrack;
            this._Stock = stock;
        }

        public IQueryable<StockTrackViewModel> GetSummary(
            DateTime? changeDateStart, DateTime? changeDateFinish)
        {
            return GetSummary(changeDateStart, changeDateFinish, "0", "Z");
        }

        public IQueryable<StockTrackViewModel> GetSummary(
            DateTime? changeDateStart, DateTime? changeDateFinish,
            string productStart = "0", string productFinish = "Z",
            string[] productKind = null, string[] warehouse = null
            )
        {
            string sql = " 1 = 1 ";
            List<object> obj =
                new List<object> { changeDateStart, changeDateFinish, productStart, productFinish };

            if (changeDateStart.HasValue)
                sql += " && (ChangeDate >= @0)";

            if (changeDateFinish.HasValue)
                sql += " && (ChangeDate <= @1)";

            if (!string.IsNullOrEmpty(productStart))
                sql += " && (ProductID >= @2)";

            if (!string.IsNullOrEmpty(productFinish))
                sql += " && (ProductID <= @3)";

            var temp =
                this.GetAll().Where(sql, obj.ToArray()).OrderBy("ProductID DESC");

            // 篩選想要看的產品類別
            temp = temp.AsEnumerable()
                .Where(x =>
                        (productKind == null || productKind.Contains(x.KindID))
                      ).AsQueryable();

            // 篩選想要看倉庫
            var temp2 = from u in temp
                          group u by new { u.ProductID , u.ProductName , u.ChangeType, u.WarehouseID } into  g
                          select new {
                              ProductID = g.Key.ProductID,
                              ProductName = g.Key.ProductName,
                              ChangeType = g.Key.ChangeType,
                              WarehouseID = g.Key.WarehouseID,
                              Qty = g.Sum(u => u.StockQty) };

            var summary = temp2.AsEnumerable()
                .Where(x => warehouse == null  || warehouse.Contains(x.WarehouseID))
                .GroupBy(x=> new { x.ProductID, x.ProductName, x.ChangeType })
                .Select(x=> 
                new {
                    ProductID = x.Key.ProductID,
                    ProductName = x.Key.ProductName,
                    ChangeType = x.Key.ChangeType,
                    Qty = x.Sum(y=>y.Qty)
                }).AsQueryable();

            var stock = this._Stock.GetAll().AsEnumerable()
                        .Where(x => warehouse == null || warehouse.Contains(x.WarehouseID))
                        .GroupBy(x => new { x.ProductID, x.ProductName})
                        .Select(x =>
                        new {
                            ProductID = x.Key.ProductID,
                            ProductName = x.Key.ProductName,                            
                            Qty = x.Sum(y => y.StockQty)
                        }).AsQueryable();

            var query = from u in summary
                        group u by new { u.ProductID, u.ProductName } into g
                        select new StockTrackViewModel
                        {
                            ProductID = g.Key.ProductID,
                            ProductName = g.Key.ProductName,
                            Purchase = (
                                            from a in summary
                                            where (a.ProductID == g.Key.ProductID && a.ChangeType == "進貨")
                                            select a.Qty
                                          ).FirstOrDefault(),
                            Sales = (
                                            from a in summary
                                            where (a.ProductID == g.Key.ProductID && a.ChangeType == "銷貨")
                                            select a.Qty
                                          ).FirstOrDefault(),
                            Adjust = (
                                            from a in summary
                                            where (a.ProductID == g.Key.ProductID && a.ChangeType == "調整")
                                            select a.Qty
                                          ).FirstOrDefault(),
                            Transfer = (
                                            from a in summary
                                            where (a.ProductID == g.Key.ProductID && a.ChangeType == "調貨")
                                            select a.Qty
                                        ).FirstOrDefault(),
                            Stock = (
                                            from a in stock
                                            where (a.ProductID == g.Key.ProductID)
                                            select a.Qty
                                        ).FirstOrDefault(),
                            ChangeStartDate = changeDateStart,
                            ChangeFinishDate = changeDateFinish
                        };

            //var linqStament = from p in pList
            //                  group p by new { p.Code, p.Name } into g
            //                  select new { Code = g.Key.Code, Name = g.Key.Name, QTY = g.Sum(p => p.QTY) };
            return query;
        }
        public IQueryable<StockTrackDetailViewModel> GetDetails(
            DateTime? changeDateStart, DateTime? changeDateFinish, string productID)
        {

            string sql = " 1 = 1 ";
            List<object> obj =
                new List<object> { changeDateStart, changeDateFinish, productID };

            if (changeDateStart.HasValue)
                sql += " && (ChangeDate >= @0)";

            if (changeDateFinish.HasValue)
                sql += " && (ChangeDate <= @1)";
               
            if (!string.IsNullOrEmpty(productID))
                sql += " && (ProductID == @2)";

            var query =
                this.GetAll().Where(sql, obj.ToArray()).OrderBy("ProductID DESC");

            return query;
        }

        public IQueryable<StockTrackDetailViewModel> GetAll()
        {         
            var query = (
                       from u in this._StockTrack.GetAll()
                       select new StockTrackDetailViewModel
                       {
                           ChangeType = u.ChangeType,
                           ChangeReason = u.ChangeReason,
                           ProductID = u.ProductID,
                           KindID = u.KindID,
                           ChangeDate = u.ChangeDate,
                           SourceID = u.SourceID,
                           CompanySupplier = u.CompanyID,
                           WarehouseID = u.WarehouseID,
                           Qty = u.Qty ?? 0,
                           StockQty　= u.StockQty ?? 0,
                           Price = u.Price ?? 0,
                           ProductName = u.ProductName,
                           CompanyName = u.CompanyName                           
                       });

            return query;
        }
    }
}
