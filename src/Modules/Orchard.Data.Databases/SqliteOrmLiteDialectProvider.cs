using Orchard.Text;
using System.Collections.Generic;
using System.Data;
using System.IO;
#if NETSTANDARD2_0
using Microsoft.Data.Sqlite;
#else
using SqliteConnection = System.Data.SQLite.SQLiteConnection;
#endif

namespace Orchard.Data.Databases
{
    public class SqliteDialectProvider : DbDialectProviderBase<SqliteDialectProvider>
    {
        public static string Password { get; set; }
        public static bool UTF8Encoded { get; set; }
        public static bool ParseViaFramework { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options)
        {
            var isFullConnectionString = connectionString.Contains(";");
            var connString = StringBuilderCache.Allocate();
            if (!isFullConnectionString)
            {
                if (connectionString != ":memory:")
                {
                    var existingDir = Path.GetDirectoryName(connectionString);
                    if (!string.IsNullOrEmpty(existingDir) && !Directory.Exists(existingDir))
                    {
                        Directory.CreateDirectory(existingDir);
                    }
                }
#if NETSTANDARD2_0
                connString.AppendFormat(@"Data Source={0};", connectionString.Trim());
#else
                connString.AppendFormat(@"Data Source={0};Version=3;New=True;Compress=True;", connectionString.Trim());
#endif
            }
            else
            {
                connString.Append(connectionString);
            }
            if (!string.IsNullOrEmpty(Password))
            {
                connString.AppendFormat("Password={0};", Password);
            }
            if (UTF8Encoded)
            {
                connString.Append("UseUTF16Encoding=True;");
            }

            if (options != null)
            {
                foreach (var option in options)
                {
                    connString.AppendFormat("{0}={1};", option.Key, option.Value);
                }
            }

            return CreateConnection(StringBuilderCache.ReturnAndFree(connString));
        }
        protected  IDbConnection CreateConnection(string connectionString)
        {
#if NETSTANDARD2_0
            return new NetStandardSqliteConnection(connectionString);
#else
            return new SqliteConnection(connectionString);
#endif
        }
    }
    public static class SqliteExtensions
    {
        public static IDbDialectProvider Configure(this IDbDialectProvider provider,
            string password = null, bool parseViaFramework = false, bool utf8Encoding = false)
        {
            if (password != null)
                SqliteDialectProvider.Password = password;
            if (parseViaFramework)
                SqliteDialectProvider.ParseViaFramework = true;
            if (utf8Encoding)
                SqliteDialectProvider.UTF8Encoded = true;

            return provider;
        }
    }
}