using System;
using System.Transactions;

namespace Orchard.Domain.Uow
{
    /// <summary>
    /// Defines a unit of work.
    /// This interface is internally
    /// Use <see cref="IUnitOfWorkManager.Begin()"/> to start a new unit of work.
    /// </summary>
    public interface IUnitOfWork :IUnitOfWorkCompleteHandle
    {
        /// <summary>
        /// Unique id of this UOW.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Begins the unit of work with given level.
        /// </summary>
        /// <param name="options">Unit of work options</param>
        void Begin(UnitOfWorkOptions options);
    }
}