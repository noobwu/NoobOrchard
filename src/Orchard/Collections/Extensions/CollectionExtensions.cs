using System;
using System.Collections.Generic;
using System.Linq;

namespace Orchard.Collections
{
    /// <summary>
    /// Extension methods for Collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Checks whatever given collection object is null or has no item.
        /// </summary>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count <= 0;
        }

        /// <summary>
        /// Adds an item to the collection if it's not already in the collection.
        /// </summary>
        /// <param name="source">Collection</param>
        /// <param name="item">Item to check and add</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns True if added, returns False if not.</returns>
        public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source.Contains(item))
            {
                return false;
            }

            source.Add(item);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="dateSelector"></param>
        /// <param name="reducer"></param>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        public static ICollection<T> ReduceTimeSeries<T>(this ICollection<T> items, Func<T, DateTime> dateSelector, Func<ICollection<T>, DateTime, T> reducer, int dataPoints)
        {
            if (items.Count <= dataPoints)
                return items;

            var minTicks = items.Min(dateSelector).Ticks;
            var maxTicks = items.Max(dateSelector).Ticks;

            var bucketSize = (maxTicks - minTicks) / dataPoints;
            var buckets = new List<long>();
            long currentTick = minTicks;
            while (currentTick < maxTicks)
            {
                buckets.Add(currentTick);
                currentTick += bucketSize;
            }

            buckets.Reverse();

            return items.GroupBy(i => buckets.First(b => dateSelector(i).Ticks >= b)).Select(g => reducer(g.ToList(), new DateTime(g.Key))).ToList();
        }
    }
}
