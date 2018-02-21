using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace CDMS.Service
{
    public static class LinqExtension
    {
        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes) where T : class
        {
            if (includes != null)
            {
                query = includes.Aggregate(query,
                          (current, include) => current.Include(include));
            }

            return query;
        }

        public static IQueryable<T> OrderByField<T>(this IQueryable<T> query, string SortField, bool Ascending)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, SortField);
            var exp = Expression.Lambda(prop, param);
            string method = Ascending ? "OrderBy" : "OrderByDescending";
            Type[] types = new Type[] { query.ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, query.Expression, exp);
            return query.Provider.CreateQuery<T>(mce);
        }
    }
}
