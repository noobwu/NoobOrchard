using Orchard.Domain.Entities;
using Orleans;

namespace OrchardNorthwind.Services.GrainInterfaces
{
    public interface IGrainWithString<TEntity>: IGrainWithStringKey, IGrainBase<TEntity, string>
         where TEntity : class, IEntity<string>
    {
    }
}
