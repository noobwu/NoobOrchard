using System.Data;
using System.Data.Common;

namespace Orchard.Data
{
    public interface IActiveTransactionProvider
    {
        /// <summary>
        ///     Gets the active transaction or null if current UOW is not transactional.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        DbTransaction GetActiveTransaction(ActiveTransactionProviderArgs args);

        /// <summary>
        ///     Gets the active database connection.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        DbConnection GetActiveConnection(ActiveTransactionProviderArgs args);
    }
}
