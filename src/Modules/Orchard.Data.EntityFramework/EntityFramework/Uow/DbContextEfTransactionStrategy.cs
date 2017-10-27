using Orchard.Domain.Uow;
using System.Collections.Generic;
using System.Data.Entity;
using System.Transactions;

namespace Orchard.Data.EntityFramework.Uow
{
    public class DbContextEfTransactionStrategy : IEfTransactionStrategy
    {
        protected UnitOfWorkOptions Options { get; private set; }

        protected IDictionary<string, ActiveTransactionInfo> ActiveTransactions { get; }

        public DbContextEfTransactionStrategy()
        {
            ActiveTransactions = new Dictionary<string, ActiveTransactionInfo>();
        }

        public void InitOptions(UnitOfWorkOptions options)
        {
            Options = options;
        }


        public void Commit()
        {
            foreach (var activeTransaction in ActiveTransactions.Values)
            {
                activeTransaction.DbContextTransaction.Commit();
            }
        }

        public void Dispose()
        {
            ActiveTransactions.Clear();
        }
    }
}