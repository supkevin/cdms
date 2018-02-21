using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CDMS.Web.Utility
{
    public class ImageClass
    {
        /// <summary>
        /// 二進位圖片，轉base64圖檔
        /// </summary>
        /// <param name="pPhoto"></param>
        /// <returns></returns>
        public static string GetImage64FromByte(byte[] pPhoto)
        {
            string imageSrc = string.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                string imageBase64 = string.Empty;
                Image xImage = (Bitmap)((new ImageConverter()).ConvertFrom(pPhoto));
                if (ImageFormat.Bmp.Equals(xImage.RawFormat))
                {
                    // strip out 78 byte OLE header (don't need to do this for normal images)
                    ms.Write(pPhoto, 78, pPhoto.Length - 78);
                }
                else
                {
                    ms.Write(pPhoto, 0, pPhoto.Length);
                }
                imageBase64 = Convert.ToBase64String(ms.ToArray());
                imageSrc = string.Format("data:image/jpeg;base64,{0}", imageBase64);
            }
            return imageSrc;
        }
    }
}