using System;
using System.Linq.Expressions;
using System.Reflection;
using Orchard.Data.Dapper.Utils;
using Orchard.Domain.Entities;
using Orchard.Domain.Uow;

using DapperExtensions;

namespace Orchard.Data.Dapper.Filters.Query
{
    /// <summary>
    /// 
    /// </summary>
    public class SoftDeleteDapperQueryFilter : IDapperQueryFilter
    {

        public SoftDeleteDapperQueryFilter()
        {
            
        }

        public bool IsDeleted => false;

        public string FilterName { get; } = DefaultDataFilters.SoftDelete;

        public bool IsEnabled =>true;

        public IFieldPredicate ExecuteFilter<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>
        {
            IFieldPredicate predicate = null;
            if (IsFilterable<TEntity, TPrimaryKey>())
            {
                predicate = Predicates.Field<TEntity>(f => (f as ISoftDelete).DeleteFlag, Operator.Eq, IsDeleted);
            }

            return predicate;
        }

        public Expression<Func<TEntity, bool>> ExecuteFilter<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (IsFilterable<TEntity, TPrimaryKey>())
            {
                PropertyInfo propType = typeof(TEntity).GetProperty(nameof(ISoftDelete.DeleteFlag));
                if (predicate == null)
                {
                    predicate = ExpressionUtils.MakePredicate<TEntity>(nameof(ISoftDelete.DeleteFlag), IsDeleted, propType.PropertyType);
                }
                else
                {
                    ParameterExpression paramExpr = predicate.Parameters[0];
                    MemberExpression memberExpr = Expression.Property(paramExpr, nameof(ISoftDelete.DeleteFlag));
                    BinaryExpression body = Expression.AndAlso(
                        predicate.Body,
                        Expression.Equal(memberExpr, Expression.Constant(IsDeleted, propType.PropertyType)));
                    predicate = Expression.Lambda<Func<TEntity, bool>>(body, paramExpr);
                }
            }
            return predicate;
        }

        private bool IsFilterable<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)) && IsEnabled;
        }
    }
}
