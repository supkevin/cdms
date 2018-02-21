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
    public class ProductSalesRankingService : IProductSalesRankingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.Sales> _Repository;
        private readonly IRepository<Model.SalesDetail> _SalesDetail;
        private readonly IRepository<Model.Product> _Product;

        public ProductSalesRankingService(IUnitOfWork unitofwork,
            IRepository<Model.Sales> repository,
            IRepository<Model.SalesDetail> salesDetail,
            IRepository<Model.Product> product
            )
        {
            this._unitOfWork = unitofwork;
            this._Repository = repository;
            this._SalesDetail = salesDetail;
            this._Product = product;
        }
          
        public IQueryable<ProductSalesRankingViewModel> 
            GetAll(DateTime start, DateTime finish)
        {
            var temp = from u in _Repository.GetAll()
                        .Where(x => x.SalesDate >= start && x.SalesDate <= finish)
                        join d in _SalesDetail.GetAll() on u.SalesID equals d.SalesID into dd
                        from d in dd.DefaultIfEmpty()
                        group d by d.ProductID into g                        
                        select new 
                        {
                            ProductID = g.Key,
                            TotalAmount= g.Sum(x=>x.Amount),
                            TotalSalesCost = g.Sum(x => x.SalesCost),
                            TotalQty = g.Sum(x => x.Qty),
                            TotalProfit = g.Sum(x => (x.Price - x.SalesCost) * x.Qty),
                            GrossProfitMargin = 
                                Math.Round((Double)(g.Sum(x => x.Price - x.SalesCost) / g.Sum(x=>x.SalesCost) * 100), 0 )
                        };

            var query = from u in temp
                        join p in this._Product.GetAll() on u.ProductID equals p.ProductID into pp
                        from p in pp.DefaultIfEmpty()
                        select new ProductSalesRankingViewModel
                        {
                            ProductID = u.ProductID,                            
                            TotalQty = u.TotalQty ?? 0,
                            TotalAmount = (decimal)u.TotalAmount,
                            TotalSalesCost = (decimal)u.TotalSalesCost,
                            TotalProfit = (decimal)u.TotalProfit,
                            GrossProfitMargin = u.GrossProfitMargin,
                            ProductName = p == null ? "" : p.ProductName,
                            KindID = p == null ? "" : p.KindID,
                        };

            return query;
        }
    }
}
