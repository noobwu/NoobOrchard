using System;
using System.Linq.Expressions;

using Orchard.Domain.Entities;

using DapperExtensions;

namespace Orchard.Data.Dapper.Filters.Query
{
    /// <summary>
    /// 
    /// </summary>
    public class NullDapperQueryFilterExecuter : IDapperQueryFilterExecuter
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly NullDapperQueryFilterExecuter Instance = new NullDapperQueryFilterExecuter();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IPredicate ExecuteFilter<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<TPrimaryKey>
        {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <returns></returns>
        public PredicateGroup ExecuteFilter<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return null;
        }
    }
}
