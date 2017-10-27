using System;
using System.Data;
using System.Data.Common;

namespace Orchard.Domain.Uow
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITransactionUnitOfWork<T> : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        ITransactionCompleteHandle Begin(T connection);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        ITransactionCompleteHandle Begin(T connection,IsolationLevel level);
    }

}
