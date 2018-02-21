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
    public class ProductImageComplexService : IProductImageComplexService
    {
        private readonly IUnitOfWork _UnitOfWork;

        private readonly IRepository<Model.ProductImage> _Repository;                        
        private readonly IRepository<Model.Product> _Product;

        public ProductImageComplexService(
            IUnitOfWork unitofwork,
            IRepository<Model.ProductImage> repository,                        
            IRepository<Model.Product> product
            )
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;                        
            this._Product = product;
        }
 
        private List<ProductImage> GetChildOnCreate(ProductImageComplex source)
        {
            List<ProductImage> infos = new List<ProductImage>(); 
            foreach (var item in source.ChildList)
            {
                ProductImage temp = Mapper.Map<ProductImage>(item);                               
                temp.LastPerson = IdentityService.GetUserData().UserID;
                temp.LastUpdate = DateTime.Now;
                infos.Add(temp);
            }
            return infos;
        }

        private ProductImage GetChildOnDelete(ProductImage info)
        {
            info.Activate = InvoiceStatus.Invalid.Value;
            info.LastPerson = IdentityService.GetUserData().UserID;
            info.LastUpdate = DateTime.Now;
            return info;
        }

        public ProductImageComplex Create(ProductImageComplex source)
        {
            #region 取資料


            #endregion

            #region 邏輯驗證


            #endregion

            #region 變為Models需要之型別及邏輯資料
            List<ProductImage> children = GetChildOnCreate(source);
            //#endregion

            //#region Models資料庫

            foreach (ProductImage item in children)
            {
                this._Repository.Create(item);
            }

            this._UnitOfWork.SaveChange();
            #endregion

            return this.Get(source.Product.ProductID);
        }
      
        public void Delete(int imageID)
        {
            #region 取資料
            ProductImage query = this._Repository.Get(x=>x.ImageID == imageID);
            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());
            #endregion

            #region 變為Models需要之型別及邏輯資料
            var info = GetChildOnDelete(query);
            #endregion

            #region Models資料庫
            this._Repository.Update(info);
            this._UnitOfWork.SaveChange();
            #endregion
        }

        public ProductImageComplex Get(string id)
        {
            ProductImageComplex info = new ProductImageComplex();
                        
            info.Product = this._Product.Get(x => x.ProductID == id);

            var query2 =
               from u in this._Repository.GetAll()            
               where (u.ProductID == id && u.Activate == InvoiceStatus.Valid.Value)
               select new ProductImageViewModel()
               {
                  ImageID = u.ImageID,
                  ProductID = u.ProductID,
                  ImagePath = u.ImagePath
               };

            info.ChildList = query2.ToList();

            return info;
        }

        public IQueryable<ProductImageComplex> GetAll()
        {
            return new List<ProductImageComplex>().AsQueryable();
        }
    }
}
