using System;
using Orchard.Caching;
using Orchard.Services;

namespace Orchard.Tests.Stubs {
    public class StubClock : IClock {

        public StubClock()
            : this(new DateTime(2009, 10, 14, 12, 34, 56, DateTimeKind.Utc)) {
        }

        public StubClock(DateTime utcNow) {
            UtcNow = utcNow;
        }
        /// <summary>
        ///  获取一个 System.DateTime 对象，该对象设置为此计算机上的当前日期和时间，表示为本地时间。
        /// </summary>
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
        public DateTime UtcNow { get; private set; }

        public void Advance(TimeSpan span) {
            UtcNow = UtcNow.Add(span);
        }

        public DateTime FutureMoment(TimeSpan span) {
            return UtcNow.Add(span);
        }

        public IVolatileToken When(TimeSpan duration) {
            return new Clock.AbsoluteExpirationToken(this, duration);
        }

        public IVolatileToken WhenUtc(DateTime absoluteUtc) {
            return new Clock.AbsoluteExpirationToken(this, absoluteUtc);
        }
    }
}
