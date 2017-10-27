using System;
using System.Linq.Expressions;
using DapperExtensions;
using Orchard.Domain.Entities;
using Orchard.Data.Dapper.Expressions;
using Orchard.Data.Dapper.Filters.Query;

namespace Orchard.Data.Dapper.Expressions
{
    internal static class DapperExpressionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IPredicate ToPredicateGroup<TEntity, TPrimaryKey>(this Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity<TPrimaryKey>
        {
            var dev = new DapperExpressionVisitor<TEntity, TPrimaryKey>();
            IPredicate pg = dev.Process(expression);
            return pg;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="expression"></param>
        /// <param name="dapperQueryFilterExecuter"></param>
        /// <returns></returns>
        public static IPredicate ToPredicateGroup<TEntity, TPrimaryKey>(this Expression<Func<TEntity, bool>> expression, IDapperQueryFilterExecuter dapperQueryFilterExecuter) where TEntity : class, IEntity<TPrimaryKey>
        {
            IPredicate predicate= dapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(expression);
            return predicate;
        }
    }
}
