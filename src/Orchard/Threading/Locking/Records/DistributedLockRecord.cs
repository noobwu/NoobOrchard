using Orchard.Domain.Entities;
using System;

namespace Orchard.Threading.Locking.Records
{
    public class DistributedLockRecord : Entity
    {
        public override int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string MachineName { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
        public virtual DateTime? ValidUntilUtc { get; set; }
    }
}
