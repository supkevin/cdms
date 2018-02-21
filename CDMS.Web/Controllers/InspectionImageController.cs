using CDMS.Language;
using CDMS.Model;
using CDMS.Service;
using CDMS.Web;
using CDMS.Web.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace CDMS.Web.Controllers
{
    public class InspectionImageController : BaseController
    {
       private readonly IInspectionImageService _inspectionimageService;

       public InspectionImageController(IInspectionImageService inspectionimageService)
        {
            this._inspectionimageService = inspectionimageService;
        }

        [HttpPost, ActionName("Delete")]       
        public ActionResult DeleteConfirmed(int? id)
        {
            ResultModel result = new ResultModel();
            try
            {
                #region 驗證Model
                if (!id.HasValue)
                    return View("Error");
                #endregion

                #region 前端資料變後端用資料ViewModel時用

                #endregion

                #region Service資料庫
                Inspection_Image inspection_image = new Inspection_Image()
                {
                    ID_Inspection_Image = (int)id
                };

                this._inspectionimageService.Delete(inspection_image);
                #endregion

                #region 訊息頁面設定
                result.Status = true;
                result.Message = "MessageComplete".ToLocalized();
                #endregion
            }
            catch (Exception ex)
            {
                #region 有錯誤時錯誤訊息
                result.Status = false;
                result.Message = ex.Message.ToString();
                #endregion
            }
            return Json(result);
        }

        public ActionResult GetImage(int id_inspection_image)
        {
            var image = this._inspectionimageService.Get(id_inspection_image);

            return File(image.BI_Inspection_Image, "image/jpeg");
        }
    }
}