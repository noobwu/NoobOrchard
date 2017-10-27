using System;
using System.Data;
namespace Orchard.Domain.Uow.Transaction
{
    /// <summary>
    /// This attribute is used to indicate that declaring method is atomic and should be considered as a unit of work.
    /// A method that has this attribute is intercepted, a database connection is opened and a transaction is started before call the method.
    /// At the end of method call, transaction is committed and all changes applied to the database if there is no exception,
    /// otherwise it's rolled back. 
    /// </summary>
    /// <remarks>
    /// This attribute has no effect if there is already a unit of work before calling this method, if so, it uses the same transaction.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface)]
    public class UowTransactionAttribute : Attribute
    {

        /// <summary>
        /// Timeout of UOW As milliseconds.
        /// Uses default value if not supplied.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// If this UOW is transactional, this option indicated the isolation level of the transaction.
        /// Uses default value if not supplied.
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }
        /// <summary>
        /// Creates a new <see cref="UnitOfWorkAttribute"/> object.
        /// </summary>
        public UowTransactionAttribute()
        {
        }
        /// <summary>
        /// Creates a new <see cref="UnitOfWorkAttribute"/> object.
        /// </summary>
        /// <param name="timeout">As seconds</param>
        public UowTransactionAttribute(int timeout)
        {
            Timeout = TimeSpan.FromSeconds(timeout);
        }
        /// <summary>
        /// Creates a new <see cref="UnitOfWorkAttribute"/> object.
        /// <see cref="IsTransactional"/> is automatically set to true.
        /// </summary>
        /// <param name="isolationLevel">Transaction isolation level</param>
        public UowTransactionAttribute(IsolationLevel isolationLevel)
        {
            IsolationLevel = isolationLevel;
        }

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkAttribute"/> object.
        /// <see cref="IsTransactional"/> is automatically set to true.
        /// </summary>
        /// <param name="isolationLevel">Transaction isolation level</param>
        /// <param name="timeout">Transaction  timeout as seconds</param>
        public UowTransactionAttribute(IsolationLevel isolationLevel, int timeout)
        {
            IsolationLevel = isolationLevel;
            Timeout = TimeSpan.FromSeconds(timeout);
        }

    }
}
