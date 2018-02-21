using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using CDMS.Model.ViewModel;
using System.Data.Entity; // nclude 用

namespace CDMS.Service
{
    public class AlternativeViewModelService : IAlternativeViewModelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.Alternative> _repository;
        private readonly IRepository<Model.Product> _product;

        public AlternativeViewModelService(IUnitOfWork unitofwork, 
            IRepository<Model.Alternative> repository,
            IRepository<Model.Product> product
            )
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;
            this._product = product;
        }

        public IEnumerable<AlternativeViewModel> GetAll()
        {
           //var query = (
           //            from u in this._Repository.GetAll()
           //            join p in this._product.GetAll()
           //            on u.AlternativeID equals p.ProductID into pp 
           //            from p in pp.DefaultIfEmpty()
           //            select new AlternativeViewModel() {
           //                ProductID = u == null ? "" : u.ProductID,
           //                AlternativeID = u.AlternativeID,
           //                AlternativeName = p.ProductName,
           //                IsDeleted = false 
           //            });
           // return query; 
           var query = (
                       from u in this._repository.GetAll()
                       join p in this._product.GetAll()
                       on u.AlternativeID equals p.ProductID into pp 
                       from p in pp.DefaultIfEmpty()
                       select new AlternativeViewModel() {
                           ProductID = u == null ? "" : u.ProductID,
                           AlternativeID = u.AlternativeID,
                           AlternativeName = p.ProductName                           
                       });
            return query; 

        }
    }
}
