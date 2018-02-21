using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
namespace CDMS.Service
{
    public class ProductImageService : IProductImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ProductImage> _repository;

        public ProductImageService(
            IUnitOfWork unitofwork, IRepository<ProductImage> repository)
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;
        }

        public ProductImage Get(int imageID)
        {
            return this._repository.Get(x => x.ImageID == imageID);
        }

        public void Delete(ProductImage model)
        {
            #region 取資料
            ProductImage query = this.Get(model.ImageID);

            #endregion

            #region 邏輯驗證
            if (query == null)//沒有資料
                throw new Exception("MessageNoData".ToLocalized());

            #endregion

            #region 變為Models需要之型別及邏輯資料

            #endregion

            #region Models資料庫
            this._repository.Delete(query);
            this._unitOfWork.SaveChange();
            #endregion
        }

        public void DeleteWithOutSaveChange(string productID)
        {
            var query = this.GetForInspection(productID);

            foreach (var item in query)
            {
                #region 取資料
                ProductImage inspectionimage = this.Get(item.ImageID);
                #endregion

                #region 邏輯驗證
                if (inspectionimage == null)//沒有資料
                    throw new Exception("MessageNoData".ToLocalized());

                #endregion

                #region 變為Models需要之型別及邏輯資料

                #endregion

                #region Models資料庫
                this._repository.Delete(inspectionimage);
                #endregion
            }
        }

        public IEnumerable<ProductImage> GetForInspection(string productID)
        {
            return this._repository.GetAll().Where(x => x.ProductID == productID);
        }
    }
}
