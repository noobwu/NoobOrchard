using Orchard.Domain.Entities;

namespace Orchard.Localization.Records
{
    public class CultureRecord : Entity
    {
        public override int Id { get; set; }
        public virtual string Culture { get; set; }
    }
}
