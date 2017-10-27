using System.Collections.Generic;
using System.Data;

namespace Orchard.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDialect"></typeparam>
    public abstract class DbDialectProviderBase<TDialect>:IDbDialectProvider
       where TDialect : IDbDialectProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public abstract IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options);
    }
}
