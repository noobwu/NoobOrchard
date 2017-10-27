using Microsoft.EntityFrameworkCore;
using OrchardNorthwind.Data.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;

namespace Orchard.Data.BenchmarkTests.EntityFrameworkCore
{
    public class EfCoreScenrioConfig
    {
        public static IEnumerable<DbContext> DbContextConfigRuns()
        {
            return new DbContext[] {
                new EfCoreTestDbContext(GetDbContextOptions())
            };
        }
        private static DbContextOptions GetDbContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<EfCoreTestDbContext>();
            //                //Server=.;Database=NorthwindTest;User ID=sa;Password=123456;Pooling=true;Max Pool Size=32767;Min Pool Size=0;Asynchronous Processing=True;MultipleActiveResultSets=True;
            var connectionString = ConfigurationManager.ConnectionStrings["NorthwindTest"].ConnectionString;
            optionsBuilder.UseSqlServer(connectionString);
            return optionsBuilder.Options;
        }
    }

}