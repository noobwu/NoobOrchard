using Microsoft.EntityFrameworkCore.Infrastructure;
using Orchard.Domain.Uow;
using System;
using System.Data;
namespace Orchard.Data.EntityFrameworkCore.Uow
{
    /// <summary>
    /// 
    /// </summary>
    public class EfCoreTransactionUnitOfWork : ITransactionUnitOfWork<DatabaseFacade>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="level"></param>
        public ITransactionCompleteHandle Begin(DatabaseFacade database)
        {
           return Begin(database, IsolationLevel.ReadCommitted);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="level"></param>
        public ITransactionCompleteHandle Begin(DatabaseFacade database, IsolationLevel level)
        {
            var handle = new EfCoreTransactionCompleteHandle(database, level);
            return handle;
        }
    }

}
