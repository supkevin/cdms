using System;
using System.Web;
using System.Web.Security;
namespace CDMS.Web
{
    public class UserModel
    {
        public string StoreID { get; set; }

        public string PosStore { get; set; }

        public string StoreName { get; set; }

        public string StaffID { get; set; }

        public string DisplayName { get; set; }

        public string DepartmentID { get; set; }

        public string TitleName { get; set; }

        public string DepartmentName { get; set; }

        public bool IsApproved { get; set; }

        public string MembershipUserName { get; set; }

        public static UserModel GetUserData()
        {
            try
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    FormsIdentity u = (FormsIdentity)HttpContext.Current.User.Identity;
                    object obj = JsonClass.JsonToObject<UserModel>(u.Ticket.UserData);
                    UserModel t = (UserModel)obj;
                    return t;
                }
                else
                {
                    throw new Exception("無法取得登入資料");
                }
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }
    }
}