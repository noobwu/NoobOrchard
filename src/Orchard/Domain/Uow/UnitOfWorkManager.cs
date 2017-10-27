using System.Linq;
using System.Transactions;
namespace Orchard.Domain.Uow
{
    /// <summary>
    /// Unit of work manager.
    /// </summary>
    public class UnitOfWorkManager : IUnitOfWorkManager, ITransientDependency
    {
        private UnitOfWorkBase _uow;
        public UnitOfWorkManager(UnitOfWorkBase uow)
        {
            _uow = uow;
        }

        public IUnitOfWorkCompleteHandle Begin()
        {
            return Begin(new UnitOfWorkOptions());
        }
        /// <summary>
        /// Begins a new unit of work.
        /// </summary>
        /// <returns>A handle to be able to complete the unit of work</returns>
        public IUnitOfWorkCompleteHandle Begin(TransactionScopeOption scope)
        {
           return  Begin(new UnitOfWorkOptions() { Scope = scope });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options)
        {

            _uow.Completed += (sender, args) =>
            {
                
            };

            _uow.Failed += (sender, args) =>
            {
                
            };

            _uow.Disposed += (sender, args) =>
            {
               
            };
            _uow.Begin(options);

            return _uow;
        }
    }
}