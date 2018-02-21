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
    public class CustomerSalesRankingService : ICustomerSalesRankingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.Sales> _Repository;
        private readonly IRepository<Model.SalesDetail> _SalesDetail;
        private readonly IRepository<Model.Company> _Company;

        public CustomerSalesRankingService(IUnitOfWork unitofwork,
            IRepository<Model.Sales> repository,
            IRepository<Model.SalesDetail> salesDetail,
            IRepository<Model.Company> company
            )
        {
            this._unitOfWork = unitofwork;
            this._Repository = repository;
            this._SalesDetail = salesDetail;
            this._Company = company;
        }
          
        public IQueryable<CustomerSalesRankingViewModel> 
            GetAll(DateTime start, DateTime finish)
        {
            var temp = from u in _Repository.GetAll()
                        .Where(x => x.SalesDate >= start && x.SalesDate <= finish)
                        join d in _SalesDetail.GetAll() on u.SalesID equals d.SalesID into dd
                        from d in dd.DefaultIfEmpty()
                        group d by u.CustomerID into g                        
                        select new 
                        {
                            CustomerID = g.Key,
                            TotalAmount= g.Sum(x=>x.Amount),
                            TotalSalesCost = g.Sum(x => x.SalesCost),
                            TotalQty = g.Sum(x => x.Qty),
                            TotalProfit = g.Sum(x => (x.Price - x.SalesCost) * x.Qty),
                            GrossProfitMargin = 
                                Math.Round((Double)(g.Sum(x => x.Price - x.SalesCost) / g.Sum(x=>x.SalesCost) * 100), 0 )
                        };

            var query = from u in temp
                        join p in this._Company.GetAll() on u.CustomerID equals p.CompanyID into pp
                        from p in pp.DefaultIfEmpty()
                        select new CustomerSalesRankingViewModel
                        {
                            CustomerID = u.CustomerID,                            
                            TotalQty = u.TotalQty ?? 0,
                            TotalAmount = (decimal)u.TotalAmount,
                            TotalSalesCost = (decimal)u.TotalSalesCost,
                            TotalProfit = (decimal)u.TotalProfit,
                            GrossProfitMargin = u.GrossProfitMargin,
                            CustomerName = p == null ? "" : p.ShortName,                            
                        };

            return query;
        }
    }
}
