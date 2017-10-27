using System;
using System.Linq.Expressions;

using Orchard.Domain.Entities;

using DapperExtensions;

namespace Orchard.Data.Dapper.Filters.Query
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDapperQueryFilterExecuter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IPredicate ExecuteFilter<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<TPrimaryKey>;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <returns></returns>
        PredicateGroup ExecuteFilter<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>;
    }
}
