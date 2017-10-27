using System;
using Orchard.Caching;

namespace Orchard.Services {
    /// <summary>
    /// 
    /// </summary>
    public class Clock : IClock {
        /// <summary>
        ///  获取一个 System.DateTime 对象，该对象设置为此计算机上的当前日期和时间，表示为本地时间。
        /// </summary>
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime UtcNow {
            get { return DateTime.UtcNow; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public IVolatileToken When(TimeSpan duration) {
            return new AbsoluteExpirationToken(this, duration);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="absoluteUtc"></param>
        /// <returns></returns>
        public IVolatileToken WhenUtc(DateTime absoluteUtc) {
            return new AbsoluteExpirationToken(this, absoluteUtc);
        }
        /// <summary>
        /// 
        /// </summary>
        public class AbsoluteExpirationToken : IVolatileToken {
            private readonly IClock _clock;
            private readonly DateTime _invalidateUtc;

            public AbsoluteExpirationToken(IClock clock, DateTime invalidateUtc) {
                _clock = clock;
                _invalidateUtc = invalidateUtc;
            }

            public AbsoluteExpirationToken(IClock clock, TimeSpan duration) {
                _clock = clock;
                _invalidateUtc = _clock.UtcNow.Add(duration);
            }

            public bool IsCurrent {
                get {
                    return _clock.UtcNow < _invalidateUtc;
                }
            }
        }
    }
}
