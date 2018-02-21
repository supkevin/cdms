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
    public class ProductComplexService : IProductComplexService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Model.Product> _repository;
        private readonly IRepository<Model.Alternative> _alternative;

        public ProductComplexService(IUnitOfWork unitofwork,
            IRepository<Model.Product> repository,
            IRepository<Model.Alternative> alternative
            )
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;
            this._alternative = alternative;
        }

        public ProductComplex Get(string id)
        {         
            var info = new ProductComplex();
            info.Product = this._repository.Get(x => x.ProductID == id);

            var query = (
                        from u in this._alternative.GetAll()
                        where u.ProductID == id
                        join p in this._repository.GetAll() on u.AlternativeID equals p.ProductID into procuct
                        from p in procuct
                        select new AlternativeViewModel()
                        {
                            SeqNo = u.SeqNo,
                            ProductID = u.ProductID,
                            AlternativeID = u.AlternativeID,
                            AlternativeCount = u.AlternativeCount,
                            Remarks = u.Remarks,
                            AlternativeName = p.ProductName,
                            IsDirty = false 
                        }
                        );

            info.ChildList = query.ToList();
            return info;
        }

        //public IEnumerable<ProductViewModel> GetAll()
        //{
        //    var query =
        //         this._repository.GetAll()
        //         .Select(x =>
        //          new ProductViewModel
        //          {
        //              ProductID = x.ProductID,
        //              ProductName = x.ProductName,
        //              AlternativeList =
        //                     this._alternative.GetAll()
        //                     .Where(y => y.ProductID == x.ProductID)
        //                     .Select(y => new AlternativeViewModel
        //                     {
        //                         AlternativeID = y.AlternativeID
        //                     }),

        //          });

        //    return query;
        //}
    }
}
