using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Orchard.Data.SqlServer
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlServerDialectProvider : DbDialectProviderBase<SqlServerDialectProvider>
    {
        public static SqlServerDialectProvider Instance = new SqlServerDialectProvider();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options)
        {
            var isFullConnectionString = connectionString.Contains(";");

            if (!isFullConnectionString)
            {
                var filePath = connectionString;

                var filePathWithExt = filePath.EndsWith(".mdf")
                    ? filePath
                    : filePath + ".mdf";

                var fileName = Path.GetFileName(filePathWithExt);
                var dbName = fileName.Substring(0, fileName.Length - ".mdf".Length);

                connectionString = $@"Data Source=.\SQLEXPRESS;AttachDbFilename={filePathWithExt};Initial Catalog={dbName};Integrated Security=True;User Instance=True;";
            }

            if (options != null)
            {
                foreach (var option in options)
                {
                    if (option.Key.ToLower() == "read only")
                    {
                        if (option.Value.ToLower() == "true")
                        {
                            connectionString += "Mode = Read Only;";
                        }
                        continue;
                    }
                    connectionString += option.Key + "=" + option.Value + ";";
                }
            }
            return new SqlConnection(connectionString);
        }
    }
}
