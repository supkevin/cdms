using CDMS.Language;
using CDMS.Model;
using CDMS.Model.ViewModel;
using CDMS.Service;
using CDMS.Web;
using CDMS.Web.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using CDMS.Web.ActionFilter;
using System.IO;
using System.Text;
using CDMS.Web.Utility;
using System.Data.Entity;
using AutoMapper;
using System.Web.Configuration;

namespace CDMS.Web.Controllers
{
    public class ProductImageController : BaseController
    {
        private readonly IProductImageService _ProductImageService;        
        private readonly IProductImageComplexService _ProductImageComplexService;

        public ProductImageController(
            IProductImageService productImageService,            
            IProductImageComplexService productImageComplexService)
        {
            this._ProductImageService = productImageService;            
            this._ProductImageComplexService = productImageComplexService;
        }


        public ActionResult Index(string id)
        {
            var info = this._ProductImageComplexService.Get(id);
            return View(info);
        }

        private void InitViewBag(Product info)
        {

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "Create")]
        public ActionResult Edit(ProductImageComplex info, IEnumerable<HttpPostedFileBase> file)
        {
            ResultModel result = new ResultModel() { CloseWindow = false };

            try
            {
                #region 驗證Model
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorClass.FormatToString(ModelState));
                }
                #endregion

                #region 前端資料變後端用資料ViewModel時用

                if (file != null && file.Count() > 0)
                {
                    string fileRootPath =
                        WebConfigurationManager.AppSettings["FileRootPath"];

                    string folder = $"{fileRootPath}\\{info.Product.ProductID}";                    
                    Directory.CreateDirectory(folder);

                    foreach (var item in file)
                    {
                        if (item != null)
                        {                                                                                    
                            string extension = Path.GetExtension(item.FileName);

                            var fileName = $"{ Guid.NewGuid().ToString()}{extension}";
                            string path = Path.Combine(folder, fileName);

                            info.ChildList.Add(
                            new ProductImageViewModel
                            {                                
                                ProductID = info.Product.ProductID,
                                ImagePath = path,
                                Activate = InvoiceStatus.Valid.Value
                            });

                            item.SaveAs(path);
                        }
                    }
                }
                #endregion

                #region Service資料庫
                this._ProductImageComplexService.Create(info);
                #endregion

                #region 訊息頁面設定
                result.Status = true;                
                result.Url = Url.Action("Index", "ProductImage", new { id = info.Product.ProductID });
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

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            ResultModel result = new ResultModel() { CloseWindow = false };
            try
            {
                #region 驗證Model
                if (!id.HasValue)
                    return View("Error");
                #endregion

           
                this._ProductImageComplexService.Delete(id.Value);

                #region 訊息頁面設定
                result.Status = true;
                result.Url = Url.Action("Index", "ProductImage");
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

        public ActionResult GetImage(int id)
        {
            var image = this._ProductImageService.Get(id);
                        
            try
            {
                byte[] data = System.IO.File.ReadAllBytes(image.ImagePath);
                return File(data, "image/jpeg");
            }
            catch (Exception ex)
            {
                return File(new byte[0], "image/jpeg");
            }           
        }
    }
}