using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Linq;
using NHibernate.Proxy;
using NHibernate.Tuple;
using NHibernate.Persister.Entity;
using System.Linq;
namespace Orchard.Data.NHibernate.Extensions
{
	public static class SessionExtensions
	{
		public static System.Linq.IQueryable Query(this ISession session, Type entityType)
		{
			MethodInfo method = typeof(LinqExtensionMethods).GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod).Where(x => x.GetParameters()[0].ParameterType == typeof(ISession)).Single().MakeGenericMethod(entityType);
			return(method.Invoke(null, new Object[] { session }) as System.Linq.IQueryable);
		}

		public static T Attach<T>(this ISession session, T entity, LockMode mode = null)
		{
			mode = mode ?? LockMode.None;

			IEntityPersister persister = session.GetSessionImplementation().GetEntityPersister(NHibernateProxyHelper.GuessClass(entity).FullName, entity);
			Object[] fields = persister.GetPropertyValues(entity, session.ActiveEntityMode);
			Object id = persister.GetIdentifier(entity, session.ActiveEntityMode);
			Object version = persister.GetVersion(entity, session.ActiveEntityMode);
			EntityEntry entry = session.GetSessionImplementation().PersistenceContext.AddEntry(entity, Status.Loaded, fields, null, id, version, LockMode.None, true, persister, true, false);
			
			return (entity);
		}

		public static IDictionary<String, Object> GetDirtyProperties<T>(this ISession session, T entity)
		{
			ISessionImplementor sessionImpl = session.GetSessionImplementation();
			IPersistenceContext context = sessionImpl.PersistenceContext;
			EntityEntry entry = context.GetEntry(context.Unproxy(entity));

			if ((entry == null) || (entry.RequiresDirtyCheck(entity) == false) || (entry.ExistsInDatabase == false) || (entry.LoadedState == null))
			{
				return (null);
			}

			IEntityPersister persister = entry.Persister;
			String[] propertyNames = persister.PropertyNames;
			Object[] currentState = persister.GetPropertyValues(entity, sessionImpl.EntityMode);
			Object[] loadedState = entry.LoadedState;
			IEnumerable<StandardProperty> dp = (persister.EntityMetamodel.Properties.Where((property, i) => (LazyPropertyInitializer.UnfetchedProperty.Equals(loadedState[i]) == false) && (property.Type.IsDirty(loadedState[i], currentState[i], sessionImpl) == true))).ToArray();

			return (dp.ToDictionary(x => x.Name, x => currentState[Array.IndexOf(propertyNames, x.Name)]));
		}

		public static IEnumerable<T> Local<T>(this ISession session, Status status = Status.Loaded)
		{
			ISessionImplementor impl = session.GetSessionImplementation();
			IPersistenceContext pc = impl.PersistenceContext;

			foreach (T key in pc.EntityEntries.Keys.OfType<T>())
			{
				EntityEntry entry = pc.EntityEntries[key] as EntityEntry;

				if (entry.Status == status)
				{
					yield return (key);
				}
			}
		}

		public static EntityEntry Entry<T>(this ISession session, T entity)
		{
			ISessionImplementor impl = session.GetSessionImplementation();
			IPersistenceContext pc = impl.PersistenceContext;			
			EntityEntry entry = pc.EntityEntries[entity] as EntityEntry;

			return (entry);
		}

		public static Int32 Delete<T>(this ISession session)
		{
			String hql = String.Format("delete {0}", typeof(T));
			return (session.CreateQuery(hql).ExecuteUpdate());
		}

		public static Boolean DeleteById<T>(this ISession session, Object id)
		{
			String hql = String.Format("delete {0} where id = :id", typeof(T));
			return (session.CreateQuery(hql).SetParameter("id", id).ExecuteUpdate() == 1);
		}

		public static void Evict<T>(this ISession session, Object id)
		{
			foreach (KeyValuePair<EntityKey, Object> entity in session.GetSessionImplementation().PersistenceContext.EntitiesByKey.Where(x => x.Value is T && Object.Equals(x.Key.Identifier, id)).ToArray())
			{
				session.Evict(entity.Value);
			}
		}

		public static void Evict<T>(this ISession session)
		{
			foreach (T entity in session.GetSessionImplementation().PersistenceContext.EntityEntries.Keys.OfType<T>())
			{
				session.Evict(entity);
			}
		}

		public static T GetFromCache<T>(this ISession session, Object id)
		{
			ISessionImplementor sessionImpl = session.GetSessionImplementation();
			IPersistenceContext context = sessionImpl.PersistenceContext;

			foreach (DictionaryEntry entry in context.EntityEntries)
			{
				if (entry.Key is T)
				{
					if (Object.Equals((entry.Value as EntityEntry).Id, id) == true)
					{
						return ((T)entry.Key);
					}
				}
			}

			return (default(T));
		}

		public static Boolean HasDirtyProperties(this ISession session, Object entity)
		{
			ISessionImplementor sessionImpl = session.GetSessionImplementation();
			IPersistenceContext context = sessionImpl.PersistenceContext;
			EntityEntry entry = context.GetEntry(context.Unproxy(entity));

			if ((entry == null) || (entry.RequiresDirtyCheck(entity) == false) || (entry.ExistsInDatabase == false) || (entry.LoadedState == null))
			{
				return (false);}

			IEntityPersister persister = entry.Persister;
			Object[] currentState = persister.GetPropertyValues(entity, sessionImpl.EntityMode);
			Object[] loadedState = entry.LoadedState;
			IEnumerable<StandardProperty> dp = (persister.EntityMetamodel.Properties.Where((property, i) => (LazyPropertyInitializer.UnfetchedProperty.Equals(loadedState[i]) == false) && (property.Type.IsDirty(loadedState[i], currentState[i], sessionImpl) == true))).ToArray();

			return(dp.Any());
		}

		public static void Reset(this ISession session, Object entity)
		{
			ISessionImplementor sessionImpl = session.GetSessionImplementation();
			IPersistenceContext context = sessionImpl.PersistenceContext;
			EntityEntry entry = context.GetEntry(entity);

			if (entry == null)
			{
				return;
			}

			IEntityPersister persister = entry.Persister;
			Object[] currentState = persister.GetPropertyValues(entity, sessionImpl.EntityMode);
			Object[] loadedState = entry.LoadedState;
			String[] propertyNames = persister.PropertyNames;			

			for (Int32 i = 0; i < currentState.Length; ++i)
			{
				if (persister.IdentifierPropertyName != propertyNames[i])
				{
					currentState[i] = loadedState[i];
					entity.GetType().GetProperty(propertyNames[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty).SetValue(entity, loadedState[i], null);
				}
			}
		}
	}
}
