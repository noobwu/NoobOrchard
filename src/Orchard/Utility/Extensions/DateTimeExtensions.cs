using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard
{
    /// <summary>
    /// A fast, standards-based, serialization-issue free DateTime serailizer.
    /// </summary>
    public static class DateTimeExtensions
    {
        public const long UnixEpoch = 621355968000000000L;
        private static readonly DateTime UnixEpochDateTimeUtc = new DateTime(UnixEpoch, DateTimeKind.Utc);
        private static readonly DateTime UnixEpochDateTimeUnspecified = new DateTime(UnixEpoch, DateTimeKind.Unspecified);
        private static readonly DateTime MinDateTimeUtc = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime FromUnixTime(this int unixTime)
        {
            return UnixEpochDateTimeUtc + TimeSpan.FromSeconds(unixTime);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime FromUnixTime(this double unixTime)
        {
            return UnixEpochDateTimeUtc + TimeSpan.FromSeconds(unixTime);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime FromUnixTime(this long unixTime)
        {
            return UnixEpochDateTimeUtc + TimeSpan.FromSeconds(unixTime);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToUnixTimeMsAlt(this DateTime dateTime)
        {
            return (dateTime.ToStableUniversalTime().Ticks - UnixEpoch) / TimeSpan.TicksPerMillisecond;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToUnixTimeMs(this DateTime dateTime)
        {
            var universal = ToDateTimeSinceUnixEpoch(dateTime);
            return (long)universal.TotalMilliseconds;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToUnixTime(this DateTime dateTime)
        {
            try
            {
                //
                //tick:10000000(tick)为一秒
                return (dateTime.ToUniversalTime().Ticks - UnixEpochDateTimeUtc.Ticks) / TimeSpan.TicksPerSecond;
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ToStableUniversalTime(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;
            if (dateTime == DateTime.MinValue)
                return MinDateTimeUtc;

            return dateTime.ToUniversalTime();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static TimeSpan ToDateTimeSinceUnixEpoch(this DateTime dateTime)
        {
            var dtUtc = dateTime.ToUniversalTime();
            var universal = dtUtc.Subtract(UnixEpochDateTimeUtc);
            return universal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static long ToUnixTimeMs(this long ticks)
        {
            return (ticks - UnixEpoch) / TimeSpan.TicksPerMillisecond;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msSince1970"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimeMs(this double msSince1970)
        {
            return UnixEpochDateTimeUtc + TimeSpan.FromMilliseconds(msSince1970);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msSince1970"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimeMs(this long msSince1970)
        {
            return UnixEpochDateTimeUtc + TimeSpan.FromMilliseconds(msSince1970);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msSince1970"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimeMs(this long msSince1970, TimeSpan offset)
        {
            return DateTime.SpecifyKind(UnixEpochDateTimeUnspecified + TimeSpan.FromMilliseconds(msSince1970) + offset, DateTimeKind.Local);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msSince1970"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimeMs(this double msSince1970, TimeSpan offset)
        {
            return DateTime.SpecifyKind(UnixEpochDateTimeUnspecified + TimeSpan.FromMilliseconds(msSince1970) + offset, DateTimeKind.Local);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msSince1970"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimeMs(string msSince1970)
        {
            long ms;
            if (long.TryParse(msSince1970, out ms)) return ms.FromUnixTimeMs();

            // Do we really need to support fractional unix time ms time strings??
            return double.Parse(msSince1970).FromUnixTimeMs();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msSince1970"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimeMs(string msSince1970, TimeSpan offset)
        {
            long ms;
            if (long.TryParse(msSince1970, out ms)) return ms.FromUnixTimeMs(offset);

            // Do we really need to support fractional unix time ms time strings??
            return double.Parse(msSince1970).FromUnixTimeMs(offset);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime RoundToMs(this DateTime dateTime)
        {
            return new DateTime((dateTime.Ticks / TimeSpan.TicksPerMillisecond) * TimeSpan.TicksPerMillisecond, dateTime.Kind);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime RoundToSecond(this DateTime dateTime)
        {
            return new DateTime((dateTime.Ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond, dateTime.Kind);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static DateTime Floor(this DateTime date, TimeSpan interval)
        {
            return date.AddTicks(-(date.Ticks % interval.Ticks));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static DateTime Ceiling(this DateTime date, TimeSpan interval)
        {
            return date.AddTicks(interval.Ticks - (date.Ticks % interval.Ticks));
        }
    }
}
