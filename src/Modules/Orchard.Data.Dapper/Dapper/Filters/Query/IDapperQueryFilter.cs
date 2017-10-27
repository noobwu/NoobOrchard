using System;
using System.Linq.Expressions;
using Orchard.Domain.Entities;

using DapperExtensions;

namespace Orchard.Data.Dapper.Filters.Query
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDapperQueryFilter : ITransientDependency
    {
        /// <summary>
        /// 
        /// </summary>
        string FilterName { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsEnabled { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <returns></returns>
        IFieldPredicate ExecuteFilter<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Expression<Func<TEntity, bool>> ExecuteFilter<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<TPrimaryKey>;
    }
}
