
using Orchard.Domain.Entities;
using ServiceStack.OrmLite;

namespace Orchard.Data.OrmLite.Repositories
{
    /// <summary>
    /// A shortcut of <see cref="NhRepositoryBase{TEntity,TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public class OrmLiteRepositoryBase<TEntity> : OrmLiteRepositoryBase<TEntity, int>,
        Domain.Repositories.IRepository<TEntity> where TEntity : class, IEntity<int>
    {

        /// <summary>
        /// Creates a new <see cref="OrmLiteRepositoryBase{TEntity,TPrimaryKey}"/> object.
        /// </summary>
        /// <param name="dbFactory">providing  connection factory</param>
        public OrmLiteRepositoryBase(OrmLiteConnectionFactory dbFactory)
            : base(dbFactory)
        {
        }
    }
}
