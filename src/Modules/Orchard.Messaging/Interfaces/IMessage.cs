using System;
using Orchard.Domain.Entities;

namespace Orchard.Messaging
{
    public interface IMessage
        : IHasId<Guid>, IMeta
    {
        DateTime CreatedDate { get; }

        long Priority { get; set; }

        int RetryAttempts { get; set; }

        Guid? ReplyId { get; set; }

        string ReplyTo { get; set; }

        int Options { get; set; }

        ResponseStatus Error { get; set; }

        string Tag { get; set; }

        object Body { get; set; }
    }

    public interface IMessage<T>
        : IMessage
    {
        T GetBody();
    }
}