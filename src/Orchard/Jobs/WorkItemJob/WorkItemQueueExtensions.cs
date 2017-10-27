using Orchard.Queues;
using Orchard.Serializer;
using Orchard.Threading.Tasks;
using System;
using System.Threading.Tasks;

namespace Orchard.Jobs
{
    public static class WorkItemQueueExtensions {
        public static async Task<string> EnqueueAsync<T>(this IQueue<WorkItemData> queue, T workItemData, bool includeProgressReporting = false) {
            string id = Guid.NewGuid().ToString("N");
            var json = await queue.Serializer.SerializeToStringAsync(workItemData).AnyContext();
            await queue.EnqueueAsync(new WorkItemData {
                Data = json,
                WorkItemId = id,
                Type = typeof(T).AssemblyQualifiedName,
                SendProgressReports = includeProgressReporting
            }).AnyContext();

            return id;
        }
    }
}