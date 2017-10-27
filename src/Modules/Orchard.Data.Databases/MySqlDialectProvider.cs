using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace Orchard.Data.Databases
{
    /// <summary>
    /// 
    /// </summary>
    public class MySqlDialectProvider : DbDialectProviderBase<MySqlDialectProvider>
    {
        public override IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options)
        {
            return new MySqlConnection(connectionString);
        }
    }
}
