using Orchard.Domain.Entities;

namespace Orchard.Data.EntityFrameworkCore.Repositories
{
    /// <summary>
    /// A shortcut of <see cref="EfCoreRepositoryBase{TEntity,TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public class EfCoreRepositoryBase<TEntity> :
        EfCoreRepositoryBase<TEntity, int>, Domain.Repositories.IRepository<TEntity>
      where TEntity : class, IEntity<int>
    {
        /// <summary>
        /// Creates a new <see cref="EfCoreRepositoryBase{TEntity,TPrimaryKey}"/> object.
        /// </summary>
        /// <param name="EfCoreDbContext">dbContext</param>
        public EfCoreRepositoryBase(EfCoreDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
