using CDMS.Language;
using CDMS.Model;
using CDMS.Model.ViewModel;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public class MenuComplexService : IMenuComplexService
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IRepository<Model.Menu> _Repository;
        private readonly IRepository<Model.MenuPermission> _Permission;
        private readonly IRepository<Model.User> _User;

        public MenuComplexService(IUnitOfWork unitofwork, 
            IRepository<Model.Menu> repository, 
            IRepository<Model.MenuPermission> permission,
            IRepository<Model.User> user)
        {
            this._UnitOfWork = unitofwork;
            this._Repository = repository;
            this._Permission = permission;
            this._User = user;
        }
              
        public IQueryable<MenuComplex> Get(string permissionID)
        {
            var query = from u in this._Repository.GetAll()
                        join p in this._Permission.GetAll()
                            on u.MenuID equals p.MenuID into child
                        from p in child
                        where p.PermissionID == permissionID
                        orderby u.SortOrder
                        select u;

            var group = from u in this._Repository.GetAll()
                        where ( u.ParentMenuID ?? 0 ) == 0 
                        select new MenuComplex
                        {
                            Group = u,
                            ChildList = (
                                 from p in query
                                 where p.ParentMenuID == u.MenuID
                                 select p
                                 ).ToList()
                        };
                                                                         
            return group;
        }
    }
}
