using Orchard.Domain.Uow;
using System.Data.Entity;

namespace Orchard.Data.EntityFramework.Uow
{
    public interface IEfTransactionStrategy
    {
        void InitOptions(UnitOfWorkOptions options);

        void Commit();

        void Dispose();
    }
}