using Orchard.Domain.Entities;
using Orchard.Environment.Extensions;
using System;

namespace Orchard.MessageBus.Models
{

    [OrchardFeature("Orchard.MessageBus.SqlServerServiceBroker")]
    public class MessageRecord : Entity
    {

        public override int Id { get; set; }
        public virtual string Publisher { get; set; }
        public virtual string Channel { get; set; }
        public virtual DateTime CreatedUtc { get; set; }

        public virtual string Message { get; set; }
    }
}
