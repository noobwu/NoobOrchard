using System;
using System.Data;
using System.Threading.Tasks;

namespace Orchard.Domain.Uow
{
    /// <summary>
    /// Used to complete a unit of work.
    /// This interface can not be injected or directly used.
    /// Use <see cref="IUnitOfWorkManager"/> instead.
    /// </summary>
    public interface ITransactionCompleteHandle : IDisposable
    {
        /// <summary>
        /// Force the underlying transaction to roll back.
        /// </summary>
        void Rollback();
        /// <summary>
        ///  Flush the associated ISession and end the unit of work.
        ///  This method will commit the underlying transaction if and only if the transaction
        ///  was initiated by this object.
        /// </summary>
        void Commit();

    }
}