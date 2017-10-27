using Orchard.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Orchard.Messaging {
    public class InMemoryMessageBus : MessageBusBase<InMemoryMessageBusOptions> {
        private readonly ConcurrentDictionary<Type, long> _messageCounts = new ConcurrentDictionary<Type, long>();
        private long _messagesSent;

        public InMemoryMessageBus(InMemoryMessageBusOptions options) : base(options) { }

        public long MessagesSent => _messagesSent;

        public long GetMessagesSent(Type messageType) {
            return _messageCounts.TryGetValue(messageType, out long count) ? count : 0;
        }

        public long GetMessagesSent<T>() {
            return _messageCounts.TryGetValue(typeof(T), out long count) ? count : 0;
        }

        public void ResetMessagesSent() {
            Interlocked.Exchange(ref _messagesSent, 0);
            _messageCounts.Clear();
        }

        protected override Task PublishImplAsync(Type messageType, object message, TimeSpan? delay, CancellationToken cancellationToken) {
            Interlocked.Increment(ref _messagesSent);
            _messageCounts.AddOrUpdate(messageType, t => 1, (t, c) => c + 1);

            if (_subscribers.IsEmpty)
                return Task.CompletedTask;

            if (delay.HasValue && delay.Value > TimeSpan.Zero) {
                _logger.Debug($"Schedule delayed message: {messageType.FullName} ({delay.Value.TotalMilliseconds}ms)");
                return AddDelayedMessageAsync(messageType, message, delay.Value);
            }

            var subscribers = _subscribers.Values.Where(s => s.IsAssignableFrom(messageType)).ToList();
            if (subscribers.Count == 0) {
                _logger.Debug($"Done sending message to 0 subscribers for message type {messageType.Name}.");
                return Task.CompletedTask;
            }

            _logger.Debug($"Message Publish: {messageType.FullName}");

            return SendMessageToSubscribersAsync(subscribers, messageType, message.DeepClone());
        }
    }
}