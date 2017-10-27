using NHibernate;
using Orchard.Domain.Uow;
using System;
using System.Data;

namespace Orchard.Data.NHibernate.Uow
{
    /// <summary>
    /// 
    /// </summary>
    public class NHibernateTransactionUnitOfWork : ITransactionUnitOfWork<ISession>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="level"></param>
        public ITransactionCompleteHandle Begin(ISession connection)
        {
           return Begin(connection,IsolationLevel.ReadCommitted);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="level"></param>
        public ITransactionCompleteHandle Begin(ISession connection, IsolationLevel level)
        {
            var handle = new NHibernateTransactionCompleteHandle(connection,level);
            return handle;
        }
    }

}
