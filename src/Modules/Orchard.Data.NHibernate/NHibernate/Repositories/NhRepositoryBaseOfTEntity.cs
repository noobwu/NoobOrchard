using NHibernate;
using Orchard.Domain.Entities;

namespace Orchard.Data.NHibernate.Repositories
{
    /// <summary>
    /// A shortcut of <see cref="NhRepositoryBase{TEntity,TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public class NhRepositoryBase<TEntity> : NhRepositoryBase<TEntity, int>, Domain.Repositories.IRepository<TEntity> where TEntity : class, IEntity<int>
    {
        /// <summary>
        /// Creates a new <see cref="NhRepositoryBase{TEntity,TPrimaryKey}"/> object.
        /// </summary>
        /// <param name="sessionFactory">A sessionFactory provider to obtain sessionFactory for database operations</param>
        public NhRepositoryBase(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }
    }
}
