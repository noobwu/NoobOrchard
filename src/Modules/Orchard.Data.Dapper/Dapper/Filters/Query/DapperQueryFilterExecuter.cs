using DapperExtensions;
using Orchard.Data.Dapper.Expressions;
using Orchard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Orchard.Data.Dapper.Filters.Query
{
    /// <summary>
    /// 
    /// </summary>
    public class DapperQueryFilterExecuter : IDapperQueryFilterExecuter, ITransientDependency
    {
        private readonly IEnumerable<IDapperQueryFilter> queryFilters;
        /// <summary>
        /// 
        /// </summary>
        public DapperQueryFilterExecuter()
        {
            queryFilters = new IDapperQueryFilter[] { new SoftDeleteDapperQueryFilter() };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IPredicate ExecuteFilter<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (queryFilters != null&&queryFilters.Count()>0)
            {
                ICollection<IDapperQueryFilter> filters = queryFilters.ToList();
                foreach (IDapperQueryFilter filter in filters)
                {
                    predicate = filter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
                }
            }
            IPredicate pg = predicate.ToPredicateGroup<TEntity, TPrimaryKey>();
            return pg;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <returns></returns>
        public PredicateGroup ExecuteFilter<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>
        {
            PredicateGroup predicateGroup=null;
            if (queryFilters != null && queryFilters.Count() > 0)
            {
                 predicateGroup = new PredicateGroup
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate>()
                };
                ICollection<IDapperQueryFilter> filters = queryFilters.ToList();
                foreach (IDapperQueryFilter filter in filters)
                {
                    IFieldPredicate predicate = filter.ExecuteFilter<TEntity, TPrimaryKey>();
                    if (predicate != null)
                    {
                        predicateGroup.Predicates.Add(predicate);
                    }
                }
            }
            return predicateGroup;
        }
    }
}
