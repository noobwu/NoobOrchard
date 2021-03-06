﻿using System;
using System.Reflection;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.Impl;
using NHibernate.Type;

namespace Orchard.Data.NHibernate.Extensions
{
	public static class DialectExtensions
	{
		private static readonly MethodInfo registerFunctionMethod = typeof(Dialect).GetMethod("RegisterFunction", BindingFlags.Instance | BindingFlags.NonPublic);

		public static ISQLFunction RegisterFunction<T>(this ISessionFactory factory, String name)
		{
			IType type = NHibernateUtil.GuessType(typeof(T));
			ISQLFunction function = new StandardSQLFunction(name, type);
			RegisterFunction(factory, name, function);
			return (function);
		}

		public static ISQLFunction RegisterFunction<T>(this ISessionFactory factory, String name, String sql)
		{
			IType type = NHibernateUtil.GuessType(typeof(T));
			ISQLFunction function = new SQLFunctionTemplate(type, sql);
			RegisterFunction(factory, name, function);
			return (function);
		}

		private static void RegisterFunction(Dialect dialect, String name, ISQLFunction function)
		{
			registerFunctionMethod.Invoke(dialect, new Object[] { name, function });
		}

		private static void RegisterFunction(ISessionFactory factory, String name, ISQLFunction function)
		{
			RegisterFunction(GetDialect(factory), name, function);
		}

		public static Dialect GetDialect(this ISessionFactory factory)
		{
			return((factory as SessionFactoryImpl).Dialect);
		}
	}
}
