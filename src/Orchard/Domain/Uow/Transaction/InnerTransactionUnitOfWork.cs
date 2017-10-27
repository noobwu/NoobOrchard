using System;
using System.Data;

namespace Orchard.Domain.Uow
{
    /// <summary>
    /// 
    /// </summary>
    public class InnerTransactionUnitOfWork: ITransactionUnitOfWork<IDbConnection>
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
            var handle = new InnerTransactionCompleteHandle(connection,level);
            return handle;
        }
    }

}
