using System;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using EntityFramework.Extensions;

namespace Com.BaseLibrary.Service
{
    public static class BatchExtensionsAdapter
    {
        public static int Delete<TEntity>(this IDbSet<TEntity> source, Expression<Func<TEntity, bool>> filterExpression) where TEntity : class
        {
            return BatchExtensions.Delete(source, filterExpression);
        }





        public static int Delete<TEntity>(this ObjectSet<TEntity> source, Expression<Func<TEntity, bool>> filterExpression) where TEntity : class
        {
            return BatchExtensions.Delete(source, filterExpression);
        }


    
        public static int Update<TEntity>(this IDbSet<TEntity> source,
            Expression<Func<TEntity, bool>> filterExpression,
            Expression<Func<TEntity, TEntity>> updateExpression) where TEntity : class
        {
            return BatchExtensions.Update(source, filterExpression, updateExpression);
        }

        public static int Update<TEntity>(this ObjectSet<TEntity> source,
          Expression<Func<TEntity, bool>> filterExpression,
          Expression<Func<TEntity, TEntity>> updateExpression) where TEntity : class
        {
            return BatchExtensions.Update(source, filterExpression, updateExpression);
        }


        //[Obsolete("The API was refactored to no longer need this extension method. Use query.Where(expression).Delete() syntax instead.")]
        //public static int Update<TEntity>(this IDbSet<TEntity> source,
        //     IQueryable<TEntity> query,
        //     Expression<Func<TEntity, TEntity>> updateExpression) where TEntity : class
        //{
        //    return BatchExtensions.Update(source, query, updateExpression);
        //}


        //[Obsolete("The API was refactored to no longer need this extension method. Use query.Where(expression).Delete() syntax instead.")]
        //public static int Delete<TEntity>(this IDbSet<TEntity> source, IQueryable<TEntity> query) where TEntity : class
        //{
        //    return BatchExtensions.Delete(source, query);
        //}

        //[Obsolete("The API was refactored to no longer need this extension method. Use query.Where(expression).Delete() syntax instead.")]
        //public static int Delete<TEntity>(this ObjectSet<TEntity> source, IQueryable<TEntity> query) where TEntity : class
        //{
        //    return BatchExtensions.Delete(source, query);
        //}


        //[Obsolete("The API was refactored to no longer need this extension method. Use query.Where(expression).Delete() syntax instead.")]
        //public static int Update<TEntity>(this ObjectSet<TEntity> source,
        //     IQueryable<TEntity> query,
        //     Expression<Func<TEntity, TEntity>> updateExpression) where TEntity : class
        //{
        //    return BatchExtensions.Update(source, query, updateExpression);
        //}
    }
}
