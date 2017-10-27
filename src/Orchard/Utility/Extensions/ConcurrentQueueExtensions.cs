using System;
using System.Collections.Concurrent;

namespace Orchard
{
    internal static class ConcurrentQueueExtensions {
        public static void Clear<T>(this ConcurrentQueue<T> queue) {
            T item;
            while (queue.TryDequeue(out item)) { }
        }
    }
}
