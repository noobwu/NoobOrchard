using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Metadata;

namespace Orchard.Data.NHibernate.Extensions
{
	public static class EnumerableExtensions
	{
		#region Collections
		
		public static IQueryable<T> Query<T>(this IEnumerable<T> collection)
		{
			if (collection is AbstractPersistentCollection)
			{
				IPersistentCollection col = collection as IPersistentCollection;

				if (col.WasInitialized == false)
				{
					String role = col.Role;
					Object owner = col.Owner;
					Object key = col.Key;
					ISessionImplementor sessionImpl = typeof(AbstractPersistentCollection).GetField("session", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(collection) as ISessionImplementor;
					ISession session = sessionImpl as ISession;
					ISessionFactory sessionFactory = session.SessionFactory;
					String ownerEntityName = sessionImpl.BestGuessEntityName(owner);
					Type ownerType = Type.GetType(ownerEntityName);
					IClassMetadata metadata = sessionFactory.GetClassMetadata(ownerEntityName);
					String idPropertyName = metadata.IdentifierPropertyName;
					MethodInfo ownerIdGetMethod = ownerType.GetProperty(idPropertyName).GetGetMethod();
					String childPropertyName = role.Split('.').Last();
					MethodInfo ownerChildGetMethod = ownerType.GetProperty(childPropertyName).GetGetMethod();
					Type childType = typeof(T);
					ParameterExpression a = null;
					IQueryable<T> details = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(x => x.Name == "SelectMany" && x.GetParameters().Length == 2).First().MakeGenericMethod(ownerType, childType).Invoke
					(
						null,
						new Object[]
						{					
							typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(x => x.Name == "Where").First().MakeGenericMethod(ownerType).Invoke
							(
								null,
								new Object[]
								{
									typeof(LinqExtensionMethods).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(x => x.Name == "Query" && x.GetParameters()[0].ParameterType == typeof(ISession)).Single().MakeGenericMethod(ownerType).Invoke
									(
										null,
										new Object [] { session }
									),
									Expression.Lambda
									(
										typeof(Func<,>).MakeGenericType(ownerType, typeof(Boolean)),
										Expression.Equal(Expression.Property(a = Expression.Parameter(ownerType, "x"), ownerIdGetMethod), Expression.Constant(key, ownerIdGetMethod.ReturnType)), new ParameterExpression[] { a }
									)
								}
							),
							Expression.Lambda
							(
								typeof(Func<,>).MakeGenericType(ownerType, typeof(IEnumerable<>).MakeGenericType(childType)),
								Expression.Property(a = Expression.Parameter(ownerType, "x"), ownerChildGetMethod), new ParameterExpression[] { a }
							)
						}
					) as IQueryable<T>;

					return (details);
				}
			}

			return (collection.AsQueryable());
		}
		#endregion
	}
}
