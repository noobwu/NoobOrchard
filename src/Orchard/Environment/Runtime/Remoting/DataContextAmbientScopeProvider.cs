using Castle.Core.Logging;
using Orchard.Collections;
using Orchard.Exceptions;
using System;
using System.Collections.Concurrent;

namespace Orchard.Environment.Runtime.Remoting
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataContextAmbientScopeProvider<T> : IAmbientScopeProvider<T>
    {
        public ILogger Logger { get; set; }

        private static readonly ConcurrentDictionary<string, ScopeItem> ScopeDictionary = new ConcurrentDictionary<string, ScopeItem>();

        private readonly IAmbientDataContext _dataContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        public DataContextAmbientScopeProvider(IAmbientDataContext dataContext)
        {
            _dataContext = dataContext;

            Logger = NullLogger.Instance;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextKey"></param>
        /// <returns></returns>
        public T GetValue(string contextKey)
        {
            var item = GetCurrentItem(contextKey);
            if (item == null)
            {
                return default(T);
            }

            return item.Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IDisposable BeginScope(string contextKey, T value)
        {
            var item = new ScopeItem(value, GetCurrentItem(contextKey));

            if (!ScopeDictionary.TryAdd(item.Id, item))
            {
                throw new DefaultException("Can not add item! ScopeDictionary.TryAdd returns false!");
            }

            _dataContext.SetData(contextKey, item.Id);

            return new DisposeAction(() =>
            {
                ScopeDictionary.TryRemove(item.Id, out item);

                if (item.Outer == null)
                {
                    _dataContext.SetData(contextKey, null);
                    return;
                }

                _dataContext.SetData(contextKey, item.Outer.Id);
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextKey"></param>
        /// <returns></returns>
        private ScopeItem GetCurrentItem(string contextKey)
        {
            var objKey = _dataContext.GetData(contextKey) as string;
            return objKey != null ? ScopeDictionary.GetOrDefault(objKey) : null;
        }
        /// <summary>
        /// 
        /// </summary>
        private class ScopeItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string Id { get; private set; }
            /// <summary>
            /// 
            /// </summary>
            public ScopeItem Outer { get; private set; }
            /// <summary>
            /// 
            /// </summary>
            public T Value { get; private set; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            /// <param name="outer"></param>
            public ScopeItem(T value, ScopeItem outer = null)
            {
                Id = Guid.NewGuid().ToString();

                Value = value;
                Outer = outer;
            }
        }
    }
}