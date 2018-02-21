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
    public class StockQueryService : IStockQueryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.Stock> _Repository;
        private readonly IRepository<Model.Product> _Product;
        private readonly IRepository<Model.Code> _Code;
        private readonly IRepository<Model.Alternative> _Alternative;

        public StockQueryService(IUnitOfWork unitofwork,
            IRepository<Model.Stock> repository,
            IRepository<Model.Product> product,
            IRepository<Model.Code> code,
            IRepository<Model.Alternative> alternative
            )
        {
            this._unitOfWork = unitofwork;
            this._Repository = repository;
            this._Product = product;
            this._Code = code;
            this._Alternative = alternative;
        }

        public IQueryable<StockQueryViewModel> GetAll()
        {
            var query1 = (
                        from u in this._Repository.GetAll()
                        select new
                        {
                            ProductID = u.ProductID,
                            WarehouseID = u.WarehouseID,
                            Qty = u.Qty,
                        });

            var query = (
                        from p in this._Product.GetAll()
                        join u in this._Code.GetAll().Where(x => x.CodeType == CodeType.Unit.Value)
                        on p.UnitID equals u.CodeValue into unit
                        from u in unit.DefaultIfEmpty()
                        join k in this._Code.GetAll().Where(x => x.CodeType == CodeType.ProductKind.Value)
                        on p.KindID equals k.CodeValue into kind
                        from k in kind.DefaultIfEmpty()
                        select new StockQueryViewModel()
                        {
                            ProductID = p.ProductID,
                            ProductName = p.ProductName,
                            KindID = p.KindID,
                            UnitName = u == null ? "" : u.CodeName,
                            ProductKindName = k == null ? "" : k.CodeName,
                            ListPrice = p.ListPrice ?? 0,
                            SetPrice = p.SetPrice ?? 0,
                            RealPrice = p.RealPrice ?? 0,
                            BizCost = p.BizCost ?? 0,
                            SafeStock = p.SafeStock ?? 0, 
                            QtyWarehouse1 = (
                                             from a in query1
                                             where (a.ProductID == p.ProductID 
                                                && a.WarehouseID == Warehouse.One.Value)
                                             select a.Qty ?? 0
                                             ).FirstOrDefault(),
                            QtyWarehouse2 = (
                                             from b in query1
                                             where (b.ProductID == p.ProductID 
                                                && b.WarehouseID == Warehouse.Two.Value)
                                             select b.Qty ?? 0
                                             ).FirstOrDefault()
                        });

            return query;
        }

        public IQueryable<StockQueryViewModel> GetAlternativeList(string id)
        {
            var query = from p in this._Alternative.GetAll().Where(x => x.ProductID == id)
                        join u in this.GetAll()
                        on p.AlternativeID equals u.ProductID
                        select u;
            return query;                        
        }
    }
}
