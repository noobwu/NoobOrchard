using Orchard.Domain.Entities;
using Orleans;

namespace OrchardNorthwind.Services.GrainInterfaces
{
    public interface IGrainWithInt<TEntity>: IGrainWithIntegerKey, IGrainBase<TEntity, int>
         where TEntity : class, IEntity<int>
    {
    }
}
