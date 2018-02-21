using System;
using System.Linq;
using System.Linq.Expressions;

namespace CDMS.Model.Repository
{
    public interface IRepository<TEntity>
      where TEntity : class
    {
        void Create(TEntity instance);

        void Update(TEntity instance);

        void Delete(TEntity instance);

        TEntity Get(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> GetAll();

        IQueryable<TEntity> GetAll(
            params Expression<Func<TEntity, object>>[] includeExpressions);

        void HandleDetached(TEntity instance);
    }
}
