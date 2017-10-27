using System;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Impl;

namespace NHibernateTest
{
	public static class QueryOverExtensions
	{
		public static void Register()
		{
			ExpressionProcessor.RegisterCustomProjection(() => QueryOverExtensions.Day(default(DateTime)), QueryOverExtensions.ProcessDay);
			ExpressionProcessor.RegisterCustomProjection(() => QueryOverExtensions.Month(default(DateTime)), QueryOverExtensions.ProcessMonth);
			ExpressionProcessor.RegisterCustomProjection(() => QueryOverExtensions.Year(default(DateTime)), QueryOverExtensions.ProcessYear);
		}

		public static Int32 Day(this DateTime dateTimeProperty)
		{
			return (dateTimeProperty.Day);
		}

		public static Int32 Month(this DateTime dateTimeProperty)
		{
			return (dateTimeProperty.Month);
		}

		public static Int32 Year(this DateTime dateTimeProperty)
		{
			return (dateTimeProperty.Year);
		}

		private static IProjection ProcessDay(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			return (Projections.SqlFunction("day", NHibernateUtil.Int32, property));
		}

		private static IProjection ProcessMonth(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			return (Projections.SqlFunction("month", NHibernateUtil.Int32, property));
		}

		private static IProjection ProcessYear(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			return (Projections.SqlFunction("year", NHibernateUtil.Int32, property));
		}
	}
}
