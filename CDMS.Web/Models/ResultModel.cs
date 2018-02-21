
using CDMS.Language;
 
namespace CDMS.Web
{
    public class ResultModel
    {
        public ResultModel() {
            // 預設關視窗
            this.CloseWindow = true;
            this.Status = true;
            this.Message = "MessageComplete".ToLocalized();
        }

        public bool Status { get; set; }

        public bool CloseWindow { get; set; }

        public string Url { get; set; }

        public string Message { get; set; }
    }
}