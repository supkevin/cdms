using CDMS.Language;
using CDMS.Model;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Security.Principal;

namespace CDMS.Service
{
    public class UserInfo
    {        
        public string UserName { get; set; }
        public string PermissionID { get; set; }
        public string UserID { get; set; }
        public string DepartmentID { get; set; }
    }

    public class UserPrincipal : System.Security.Principal.GenericPrincipal
    {
        public UserPrincipal(System.Security.Principal.IIdentity identity, string[] roles)
            : base(identity, roles)
        {
        }

        public UserInfo UserData { get; set; }
    }

    public class IdentityService
    {
        public string UserID = Thread.CurrentPrincipal.Identity.Name;
        public GenericPrincipal Principal = (GenericPrincipal)Thread.CurrentPrincipal;

        public static UserInfo GetUserData()
        {
            try
            {
                UserPrincipal principal = (UserPrincipal)Thread.CurrentPrincipal;
                IIdentity iden = principal.Identity;

                if (principal.Identity.IsAuthenticated)
                {
                    return principal.UserData;
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

        // 前端用把登入User轉為UserInfo
        public static UserInfo Convert(User source) {
            UserInfo result = new UserInfo()
            {
                UserID = source.UserID,
                UserName = source.UserName,
                PermissionID = source.PermissionID,
                DepartmentID = source.DepartmentID,
            };

           return result; 
        }

        public static UserPrincipal GeneratePrincipal(
            System.Security.Principal.IIdentity identity, string[] roles , UserInfo user)
        {
            var result = new UserPrincipal(identity, roles);
            result.UserData = user;
            return result; 
        }
    }
}
