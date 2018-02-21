using CDMS.Model.UnitOfWork;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;

namespace CDMS.Model.Repository
{
    public class GenericRepository<TEntity> : IRepository<TEntity>
      where TEntity : class
    {
        public IUnitOfWork _UnitOfWork { get; set; }

        private DbContext Context { get; set; }

        public DbSet<TEntity> DbSet { get; set; }

        public GenericRepository(IUnitOfWork unitofwork)
        {
            this._UnitOfWork = unitofwork;
            this.Context = unitofwork.Context;
            this.DbSet = this.Context.Set<TEntity>();
        }


        /// <summary>
        /// Creates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <exception cref="System.ArgumentNullException">instance</exception>
        public void Create(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            this.DbSet.Add(instance);
        }

        /// <summary>
        /// Updates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Update(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            this.Context.Entry(instance).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Delete(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            this.Context.Entry(instance).State = EntityState.Deleted;
        }

        /// <summary>
        /// Gets the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return this.DbSet.FirstOrDefault(predicate);
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }

        //http://appetere.com/post/passing-include-statements-into-a-repository
        //public IQueryable<TEntity> GetAll(
        //        params Expression<Func<TEntity, object>>[] includeExpressions)
        //{
        //    return includeExpressions
        //      .Aggregate<Expression<Func<TEntity, object>>, IQueryable<TEntity>>
        //       (this.DbSet, (current, expression) => current.Include(expression));
        //}

        //var orders = _ordersRepository.GetAll(o=>o.Customer, o=>o.LineItems);
        public IQueryable<TEntity> GetAll(
            params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            IQueryable<TEntity> set = this.DbSet;

            foreach (var includeExpression in includeExpressions)
            {
                set = set.Include(includeExpression);
            }
            return set;
        }

        public void HandleDetached(TEntity entity)
        {
            //var objectContext = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)this).ObjectContext;
            //var entitySet = objectContext.CreateObjectSet<TEntity>();
            //var entityKey = objectContext.CreateEntityKey(entitySet.EntitySet.Name, entity);
            //object foundSet;
            //bool exists = objectContext.TryGetObjectByKey(entityKey, out foundSet);
            //if (exists)
            //{
            //    objectContext.Detach(foundSet); //從上下文中移除
            //}
            //return exists;
            this.Context.Entry(entity).State = EntityState.Detached;            
        }
    }
}
