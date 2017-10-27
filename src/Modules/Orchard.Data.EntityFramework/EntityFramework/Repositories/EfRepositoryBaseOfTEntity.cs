using Orchard.Domain.Entities;

namespace Orchard.Data.EntityFramework.Repositories
{
    /// <summary>
    /// A shortcut of <see cref="EfRepositoryBase{TEntity,TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public class EfRepositoryBase<TEntity> :
        EfRepositoryBase<TEntity, int>, Domain.Repositories.IRepository<TEntity>
      where TEntity : class, IEntity<int>
    {
        /// <summary>
        /// Creates a new <see cref="EfRepositoryBase{TEntity,TPrimaryKey}"/> object.
        /// </summary>
        /// <param name="EfDbContext">dbContext</param>
        public EfRepositoryBase(EfDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
