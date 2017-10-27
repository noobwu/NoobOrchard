using OrchardNorthwind.Data.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Configuration;
namespace Orchard.Data.BenchmarkTests.EntityFramework
{
    public class EfScenrioConfig
    {
        public static IEnumerable<DbContext> DbContextConfigRuns()
        {
            return new DbContext[] {
                //Server=.;Database=NorthwindTest;User ID=sa;Password=123456;Pooling=true;Max Pool Size=32767;Min Pool Size=0;Asynchronous Processing=True;MultipleActiveResultSets=True;
                new EfTestDbContext(new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindTest"].ConnectionString))
            };
        }
    }

}