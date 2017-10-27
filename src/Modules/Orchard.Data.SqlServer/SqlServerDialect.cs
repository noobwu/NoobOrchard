using Orchard.Data.SqlServer;

namespace Orchard.Data
{
    public static class SqlServerDialect
    {
        public static IDbDialectProvider Provider => SqlServerDialectProvider.Instance;
    }

}
