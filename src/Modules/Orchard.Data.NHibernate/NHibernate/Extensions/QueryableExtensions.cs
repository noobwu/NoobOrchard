using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Caching;
using NHibernate;
using NHibernate.AdoNet;
using NHibernate.Impl;
using NHibernate.Linq;
using NHibernate.Engine;

namespace Orchard.Data.NHibernate.Extensions
{
	public static class QueryableExtensions
	{
		#region Private static readonly fields
		private static readonly PropertyInfo sessionProperty = typeof(DefaultQueryProvider).GetProperty("Session", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
		private static readonly FieldInfo batcherInterceptorField = typeof(AbstractBatcher).GetField("_interceptor", BindingFlags.NonPublic | BindingFlags.Instance);
		private static readonly FieldInfo sessionImplInterceptorField = typeof(SessionImpl).GetField("interceptor", BindingFlags.NonPublic | BindingFlags.Instance);
		#endregion
		
		#region AsStateless
		public static IOrderedQueryable<T> AsStateless<T>(this IOrderedQueryable<T> queryable)
		{
			return ((IOrderedQueryable<T>)AsStateless((IQueryable<T>)queryable));
		}

		public static IQueryable<T> AsStateless<T>(this IQueryable<T> queryable)
		{
			if (!(queryable is NhQueryable<T>))
			{
				return (queryable);
			}
			var session = sessionProperty.GetValue(queryable.Provider, null) as ISession;
			return (new ActionQueryable<T>(queryable, x => session.Evict(x)));
		}
		#endregion

		#region AsReadOnly
		public static IOrderedQueryable<T> AsReadOnly<T>(this IOrderedQueryable<T> queryable)
		{
			return ((IOrderedQueryable<T>)AsReadOnly((IQueryable<T>)queryable));
		}

		public static IQueryable<T> AsReadOnly<T>(this IQueryable<T> queryable)
		{
			if (!(queryable is NhQueryable<T>))
			{
				return (queryable);
			}
			var session = sessionProperty.GetValue(queryable.Provider, null) as ISession;
			var currentDefaultReadOnly = session.DefaultReadOnly;
			return (new ActionQueryable<T>(queryable, null, () => { session.DefaultReadOnly = true; }, () => { session.DefaultReadOnly = currentDefaultReadOnly; }));
		}
		#endregion
		#region AsCacheable
		public static IQueryable<T> AsCacheable<T>(this IQueryable<T> query, TimeSpan duration)
		{
			return (AsCacheable(query, (Int32) duration.TotalSeconds));
		}

		public static IQueryable<T> AsCacheable<T>(this IQueryable<T> query, Int32 seconds)
		{
			ObjectCache cache = null;

			if (ObjectCache.Host != null)
			{
				cache = ObjectCache.Host.GetService(typeof(ObjectCache)) as ObjectCache;
			}
			else
			{
				cache = MemoryCache.Default;
			}

			if (cache.Contains(query.Expression.ToString()) == true)
			{
				return ((cache[query.Expression.ToString()] as IQueryable<T>).ToList().AsQueryable());
			}
			else
			{
				cache.Add(query.Expression.ToString(), query, DateTimeOffset.Now.AddSeconds(seconds));
				return (query);
			}
		}
		#endregion

		#region ThenBy
		public static IOrderedQueryable<TSource> ThenBy<TSource>(this IQueryable<TSource> query, String propertyName, Boolean ascending)
		{
			Type type = typeof(TSource);
			ParameterExpression parameter = Expression.Parameter(type, "p");
			MemberExpression propertyReference = Expression.Property(parameter, propertyName);
			MethodCallExpression sortExpression = Expression.Call(typeof(Queryable), (ascending == true ? "ThenBy" : "ThenByDescending"), new Type[] { type }, null, Expression.Lambda<Func<TSource, Boolean>>(propertyReference, new ParameterExpression[] { parameter }));
			return (query.Provider.CreateQuery<TSource>(sortExpression) as IOrderedQueryable<TSource>);
		}

		public static IOrderedQueryable<TSource> ThenBy<TSource>(this IOrderedQueryable<TSource> query, String propertyName)
		{
			return (ThenBy<TSource>(query, propertyName, true));
		}

		public static IOrderedQueryable<TSource> ThenByDescending<TSource>(this IOrderedQueryable<TSource> query, String propertyName)
		{
			return (ThenBy<TSource>(query, propertyName, false));
		}
		#endregion

		#region OrderBy
		public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> query, String propertyName, Boolean ascending)
		{
			Type type = typeof(TSource);
			ParameterExpression parameter = Expression.Parameter(type, "p");
			MemberExpression propertyReference = Expression.Property(parameter, propertyName);
			MethodCallExpression sortExpression = Expression.Call(typeof(Queryable), (ascending == true ? "OrderBy" : "OrderByDescending"), new Type[] { type }, null, Expression.Lambda<Func<TSource, Boolean>>(propertyReference, new ParameterExpression[] { parameter }));
			return (query.Provider.CreateQuery<TSource>(sortExpression) as IOrderedQueryable<TSource>);
		}

		public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> query, String propertyName)
		{
			return (OrderBy<TSource>(query, propertyName, true));
		}

		public static IOrderedQueryable<TSource> OrderByDescending<TSource>(this IQueryable<TSource> query, String propertyName)
		{
			return (OrderBy<TSource>(query, propertyName, false));
		}
		#endregion

		#region Compare
		public enum Operand
		{
			Equal,
			NotEqual,
			GreaterThan,
			GreaterThanOrEqual,
			LessThan,
			LessThanOrEqual,
			TypeIs,
			TypeIsNot,
		}

		public static IQueryable<TSource> Compare<TSource>(this IQueryable<TSource> query, Operand op, String propertyName, Object value = null)
		{
			Type type = typeof(TSource);
			ParameterExpression pe = Expression.Parameter(type, "p");
			MemberExpression propertyReference = Expression.Property(pe, propertyName);
			ConstantExpression constantReference = Expression.Constant(value);

			switch (op)
			{
				case Operand.Equal:
					return (query.Where(Expression.Lambda<Func<TSource, Boolean>>(Expression.Equal(propertyReference, constantReference), new ParameterExpression[] { pe })));

				case Operand.NotEqual:
					return (query.Where(Expression.Lambda<Func<TSource, Boolean>>(Expression.NotEqual(propertyReference, constantReference), new ParameterExpression[] { pe })));

				case Operand.GreaterThan:
					return (query.Where(Expression.Lambda<Func<TSource, Boolean>>(Expression.GreaterThan(propertyReference, constantReference), new ParameterExpression[] { pe })));

				case Operand.GreaterThanOrEqual:
					return (query.Where(Expression.Lambda<Func<TSource, Boolean>>(Expression.GreaterThanOrEqual(propertyReference, constantReference), new ParameterExpression[] { pe })));

				case Operand.LessThan:
					return (query.Where(Expression.Lambda<Func<TSource, Boolean>>(Expression.LessThan(propertyReference, constantReference), new ParameterExpression[] { pe })));

				case Operand.LessThanOrEqual:
					return (query.Where(Expression.Lambda<Func<TSource, Boolean>>(Expression.LessThanOrEqual(propertyReference, constantReference), new ParameterExpression[] { pe })));

				case Operand.TypeIs:
					return (query.Where(Expression.Lambda<Func<TSource, Boolean>>(Expression.TypeIs(propertyReference, value as Type))));

				case Operand.TypeIsNot:
					return (query.Where(Expression.Lambda<Func<TSource, Boolean>>(Expression.IsFalse(Expression.TypeIs(propertyReference, value as Type)))));
			}

			throw (new NotImplementedException("Operand is not implemented"));
		}
		#endregion

		#region Between
		public static IQueryable<TSource> Between<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, TKey low, TKey high) where TKey : IComparable<TKey>
		{
			// Get a ParameterExpression node of the TSource that is used in the expression tree 
			ParameterExpression sourceParameter = Expression.Parameter(typeof(TSource));

			// Get the body and parameter of the lambda expression 
			Expression body = keySelector.Body;
			ParameterExpression parameter = null;

			if (keySelector.Parameters.Count > 0)
			{
				parameter = keySelector.Parameters[0];
			}

			// Get the Compare method of the type of the return value 
			MethodInfo compareMethod = typeof(TKey).GetMethod("CompareTo", new Type[] { typeof(TKey) });

			// Expression.LessThanOrEqual and Expression.GreaterThanOrEqual method are only used in 
			// the numeric comparision. If we want to compare the non-numeric type, we can't directly  
			// use the two methods.  
			// So we first use the Compare method to compare the objects, and the Compare method  
			// will return a int number. Then we can use the LessThanOrEqual and GreaterThanOrEqua method. 
			// For this reason, we ask all the TKey type implement the IComparable<> interface. 
			Expression upper = Expression.LessThanOrEqual(Expression.Call(body, compareMethod, Expression.Constant(high)), Expression.Constant(0, typeof(Int32)));
			Expression lower = Expression.GreaterThanOrEqual(Expression.Call(body, compareMethod, Expression.Constant(low)), Expression.Constant(0, typeof(Int32)));

			Expression andExpression = Expression.AndAlso(upper, lower);

			// Get the Where method expression. 
			MethodCallExpression whereCallExpression = Expression.Call(
				typeof(Queryable),
				"Where",
				new Type[] { source.ElementType },
				source.Expression,
				Expression.Lambda<Func<TSource, Boolean>>(andExpression,
				new ParameterExpression[] { parameter }));

			return (source.Provider.CreateQuery<TSource>(whereCallExpression));
		}
        #endregion

        #region Delete

        //Public extension methods
        public static void Delete<T>(this IQueryable<T> queryable)
        {
            if (queryable.GetType().GetGenericTypeDefinition() == typeof(NhQueryable<>))
            {
                ISessionImplementor impl = sessionProperty.GetValue(queryable.Provider, null) as ISessionImplementor;
                IInterceptor oldInterceptor = sessionImplInterceptorField.GetValue(impl) as IInterceptor;
                IInterceptor deleteInterceptor = new DeleteInterceptor();

                batcherInterceptorField.SetValue(impl.Batcher, deleteInterceptor);

                queryable.Any();

                batcherInterceptorField.SetValue(impl.Batcher, oldInterceptor);
            }
            else
            {
                throw (new ArgumentException("Invalid type", "queryable"));
            }
        }
        #endregion
    }
}
