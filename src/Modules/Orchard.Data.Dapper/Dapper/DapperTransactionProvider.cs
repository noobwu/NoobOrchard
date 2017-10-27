using System;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Orchard.Data.Dapper
{
    public class DapperTransactionProvider : IActiveTransactionProvider, ITransientDependency
    {
        private readonly DbConnection _connection;

        public DapperTransactionProvider(DbConnection connection)
        {
            _connection = connection;
        }

        public DbTransaction GetActiveTransaction(ActiveTransactionProviderArgs args)
        {
            return null;
        }

        public DbConnection GetActiveConnection(ActiveTransactionProviderArgs args)
        {
            return _connection;
        }
    }
}
