using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
namespace CDMS.Service
{
    public class InspectionImageService : IInspectionImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Inspection_Image> _repository;

        public InspectionImageService(
            IUnitOfWork unitofwork, IRepository<Inspection_Image> repository)
        {
            this._unitOfWork = unitofwork;
            this._repository = repository;
        }

        public Inspection_Image Get(int id_inspection_image)
        {
            return this._repository.Get(x => x.ID_Inspection_Image == id_inspection_image);
        }

        public void Delete(Inspection_Image model)
        {
            #region 取資料
            Inspection_Image query = this.Get(model.ID_Inspection_Image);

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

        public void DeleteWithOutSaveChange(int id_inspection)
        {
            var query = this.GetForInspection(id_inspection);

            foreach (var item in query)
            {
                #region 取資料
                Inspection_Image inspectionimage = this.Get(item.ID_Inspection_Image);
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

        public IEnumerable<Inspection_Image> GetForInspection(int id_inspection)
        {
            return this._repository.GetAll().Where(x => x.ID_Inspection == id_inspection);
        }
    }
}
