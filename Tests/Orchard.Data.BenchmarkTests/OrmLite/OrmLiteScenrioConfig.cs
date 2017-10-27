using OrchardNorthwind.Data.OrmLite;
using ServiceStack.OrmLite;
using System.Collections.Generic;
using System.Configuration;

namespace Orchard.Data.BenchmarkTests.OrmLite
{
    public class OrmLiteScenrioConfig
    {
        private static readonly Dictionary<IOrmLiteDialectProvider, List<string>> DataProviderAndConnectionStrings
            = new Dictionary<IOrmLiteDialectProvider, List<string>> {
                //{
                //    new SqliteOrmLiteDialectProvider(),
                //    new List<string>
                //      {
                //          ":memory:",
                //        "~/App_Data/PerfTest.sqlite".MapAbsolutePath(),
                //      }
                //}
                //,
                {
                   SqlServer2014Dialect.Provider,
                    new List<string>
                      {
                       //"Server=.;Database=NorthwindTest;User ID=sa;Password=123456;Pooling=true;Max Pool Size=32767;Min Pool Size=0;Asynchronous Processing=True;MultipleActiveResultSets=True;",
                        ConfigurationManager.ConnectionStrings["NorthwindTest"].ConnectionString
                      }
                }
            };
        public static IEnumerable<OrmLiteConfigRun> DataProviderConfigRuns()
        {
            foreach (var providerConnectionString in DataProviderAndConnectionStrings)
            {
                var dialectProvider = providerConnectionString.Key;
                var connectionStrings = providerConnectionString.Value;

                foreach (var connectionString in connectionStrings)
                {
                    yield return new OrmLiteConfigRun
                    {
                        DialectProvider = dialectProvider,
                        ConnectionString = connectionString,
                    };
                }
            }
        }
    }

    public class OrmLiteConfigRun
    {
        public IOrmLiteDialectProvider DialectProvider { get; set; }

        public string ConnectionString { get; set; }

        public IPropertyInvoker PropertyInvoker { get; set; }

        public void Init(OrmLiteScenarioBase scenarioBase)
        {
            var dbScenarioBase = scenarioBase as OrmLiteScenarioBase;
            if (dbScenarioBase == null) return;

            OrmLiteConfig.DialectProvider = this.DialectProvider;

            OrmLiteConfig.ClearCache();
            //PropertyInvoker.ConvertValueFn = OrmLiteConfig.DialectProvider.ConvertDbValue;

            //dbScenarioBase.ConnectionString = this.ConnectionString;
            dbScenarioBase.ConnectionFactory = new OrmLiteConnectionFactory(this.ConnectionString, this.DialectProvider);
            if (this.ConnectionString.Contains("Database=Northwind")|| this.ConnectionString.Contains("Initial Catalog=Northwind"))
            {
                NorthwindMappings.InitAdminOrmLiteMapping();
            }

        }

    }

}