using Orchard.Data;
using Orchard.Domain.Entities;
using Orchard.Domain.Repositories;

namespace Orchard.Data.Dapper.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DapperRepositoryBaseBackup<TEntity> :
        DapperRepositoryBaseBackup<TEntity, int>
        ,IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        public DapperRepositoryBaseBackup(IActiveTransactionProvider activeTransactionProvider)
            : base(activeTransactionProvider)
        {
        }
    }
}
