using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data
{
    #region Orderable
    public class Orderable<T>
    {
        private IQueryable<T> _queryable;

        public Orderable(IQueryable<T> enumerable)
        {
            _queryable = enumerable;
        }

        public IQueryable<T> Queryable
        {
            get { return _queryable; }
        }

        public Orderable<T> Asc<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            _queryable = _queryable
                .OrderBy(keySelector);
            return this;
        }

        public Orderable<T> Asc<TKey1, TKey2>(Expression<Func<T, TKey1>> keySelector1,
                                              Expression<Func<T, TKey2>> keySelector2)
        {
            _queryable = _queryable
                .OrderBy(keySelector1)
                .OrderBy(keySelector2);
            return this;
        }

        public Orderable<T> Asc<TKey1, TKey2, TKey3>(Expression<Func<T, TKey1>> keySelector1,
                                                     Expression<Func<T, TKey2>> keySelector2,
                                                     Expression<Func<T, TKey3>> keySelector3)
        {
            _queryable = _queryable
                .OrderBy(keySelector1)
                .OrderBy(keySelector2)
                .OrderBy(keySelector3);
            return this;
        }

        public Orderable<T> Desc<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            _queryable = _queryable
                .OrderByDescending(keySelector);
            return this;
        }

        public Orderable<T> Desc<TKey1, TKey2>(Expression<Func<T, TKey1>> keySelector1,
                                               Expression<Func<T, TKey2>> keySelector2)
        {
            _queryable = _queryable
                .OrderByDescending(keySelector1)
                .OrderByDescending(keySelector2);
            return this;
        }

        public Orderable<T> Desc<TKey1, TKey2, TKey3>(Expression<Func<T, TKey1>> keySelector1,
                                                      Expression<Func<T, TKey2>> keySelector2,
                                                      Expression<Func<T, TKey3>> keySelector3)
        {
            _queryable = _queryable
                .OrderByDescending(keySelector1)
                .OrderByDescending(keySelector2)
                .OrderByDescending(keySelector3);
            return this;
        }
    }
    #endregion

    public static class QueryableExtensions
    {
        //public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName)
        //{
        //    return QueryableHelper<T>.OrderBy(queryable, propertyName, false);
        //}
        //public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName, bool desc)
        //{
        //    return QueryableHelper<T>.OrderBy(queryable, propertyName, desc);
        //}
        //static class QueryableHelper<T>
        //{
        //    //ConcurrentDictionary<TKey, TValue>
        //    private static Dictionary<string, LambdaExpression> cache = new Dictionary<string, LambdaExpression>();
        //    public static IQueryable<T> OrderBy(IQueryable<T> queryable, string propertyName, bool desc)
        //    {
        //        dynamic keySelector = GetLambdaExpression(propertyName);
        //        return desc ? Queryable.OrderByDescending(queryable, keySelector) : Queryable.OrderBy(queryable, keySelector);
        //    }
        //    private static LambdaExpression GetLambdaExpression(string propertyName)
        //    {
        //        if (cache.ContainsKey(propertyName)) return cache[propertyName];
        //        var param = Expression.Parameter(typeof(T));
        //        var body = Expression.Property(param, propertyName);
        //        var keySelector = Expression.Lambda(body, param);
        //        cache[propertyName] = keySelector;
        //        return keySelector;
        //    }
        //  }

    }
}
