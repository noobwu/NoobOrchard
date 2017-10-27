using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Orchard.Caching {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class Cache<TKey, TResult> : ICache<TKey, TResult> {
        private readonly ICacheContextAccessor _cacheContextAccessor;
        private readonly ConcurrentDictionary<TKey, CacheEntry> _entries;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheContextAccessor"></param>
        public Cache(ICacheContextAccessor cacheContextAccessor) {
            _cacheContextAccessor = cacheContextAccessor;
            _entries = new ConcurrentDictionary<TKey, CacheEntry>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="acquire"></param>
        /// <returns></returns>
        public TResult Get(TKey key, Func<AcquireContext<TKey>, TResult> acquire) {
            var entry = _entries.AddOrUpdate(key,
                // "Add" lambda
                k => AddEntry(k, acquire),
                // "Update" lambda
                (k, currentEntry) => UpdateEntry(currentEntry, k, acquire));

            return entry.Result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <param name="acquire"></param>
        /// <returns></returns>
        private CacheEntry AddEntry(TKey k, Func<AcquireContext<TKey>, TResult> acquire) {
            var entry = CreateEntry(k, acquire);
            PropagateTokens(entry);
            return entry;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentEntry"></param>
        /// <param name="k"></param>
        /// <param name="acquire"></param>
        /// <returns></returns>
        private CacheEntry UpdateEntry(CacheEntry currentEntry, TKey k, Func<AcquireContext<TKey>, TResult> acquire) {
            var entry = (currentEntry.Tokens.Any(t => t != null && !t.IsCurrent)) ? CreateEntry(k, acquire) : currentEntry;
            PropagateTokens(entry);
            return entry;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        private void PropagateTokens(CacheEntry entry) {
            // Bubble up volatile tokens to parent context
            if (_cacheContextAccessor.Current != null) {
                foreach (var token in entry.Tokens)
                    _cacheContextAccessor.Current.Monitor(token);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <param name="acquire"></param>
        /// <returns></returns>

        private CacheEntry CreateEntry(TKey k, Func<AcquireContext<TKey>, TResult> acquire) {
            var entry = new CacheEntry();
            var context = new AcquireContext<TKey>(k, entry.AddToken);

            IAcquireContext parentContext = null;
            try {
                // Push context
                parentContext = _cacheContextAccessor.Current;
                _cacheContextAccessor.Current = context;

                entry.Result = acquire(context);
            }
            finally {
                // Pop context
                _cacheContextAccessor.Current = parentContext;
            }
            entry.CompactTokens();
            return entry;
        }
        /// <summary>
        /// 
        /// </summary>
        private class CacheEntry {
            private IList<IVolatileToken> _tokens;
            public TResult Result { get; set; }

            public IEnumerable<IVolatileToken> Tokens {
                get {
                    return _tokens ?? Enumerable.Empty<IVolatileToken>();
                }
            }

            public void AddToken(IVolatileToken volatileToken) {
                if (_tokens == null) {
                    _tokens = new List<IVolatileToken>();
                }

                _tokens.Add(volatileToken);
            }

            public void CompactTokens() {
                if (_tokens != null)
                    _tokens = _tokens.Distinct().ToArray();
            }
        }
    }
}
