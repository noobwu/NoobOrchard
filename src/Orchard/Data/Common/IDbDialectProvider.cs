using System.Collections.Generic;
using System.Data;

namespace Orchard.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDbDialectProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options);
    }
}
