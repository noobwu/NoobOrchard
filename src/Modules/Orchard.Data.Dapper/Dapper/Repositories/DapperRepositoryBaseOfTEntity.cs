using Orchard.Data;
using Orchard.Domain.Entities;
using Orchard.Domain.Repositories;

namespace Orchard.Data.Dapper.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DapperRepositoryBase<TEntity> :
        DapperRepositoryBase<TEntity, int>
        ,IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbFactory"></param>
        public DapperRepositoryBase(OrchardConnectionFactory dbFactory)
            : base(dbFactory)
        {
        }
    }
}
