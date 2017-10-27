using System;
using System.Data;

namespace Orchard.Domain.Uow
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITransactionManager : IDependency, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        void Begin();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        void Begin(IsolationLevel level);

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
