using Orchard.Domain.Uow;
using System;
using System.Data;
using System.Data.Entity;

namespace Orchard.Data.EntityFramework.Uow
{
    /// <summary>
    /// 
    /// </summary>
    public class EfTransactionUnitOfWork : ITransactionUnitOfWork<Database>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="level"></param>
        public ITransactionCompleteHandle Begin(Database database)
        {
           return Begin(database, IsolationLevel.ReadCommitted);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="level"></param>
        public ITransactionCompleteHandle Begin(Database database, IsolationLevel level)
        {
            var handle = new EfTransactionCompleteHandle(database, level);
            return handle;
        }
    }

}
