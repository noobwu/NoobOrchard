using Orchard.Domain.Uow;
using System;
using System.Data;

namespace Orchard.Data.OrmLite.Uow
{
    /// <summary>
    /// 
    /// </summary>
    public class OrmLiteTransactionUnitOfWork : ITransactionUnitOfWork<IDbConnection>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="level"></param>
        public ITransactionCompleteHandle Begin(IDbConnection connection)
        {
           return Begin(connection,IsolationLevel.ReadCommitted);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="level"></param>
        public ITransactionCompleteHandle Begin(IDbConnection connection, IsolationLevel level)
        {
            var handle = new OrmLiteTransactionCompleteHandle(connection,level);
            return handle;
        }
    }

}
