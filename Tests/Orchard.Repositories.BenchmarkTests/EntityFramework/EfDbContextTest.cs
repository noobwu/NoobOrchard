using Orchard.Data.EntityFramework;
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Reflection;

namespace Orchard.Repositories.BenchmarkTests.EntityFramework
{
    [DbConfigurationType(typeof(DbContextConfiguration))]
    public class EfDbContextTest : EfDbContext
    {
        public EfDbContextTest(DbConnection connection)
            : base(connection, false)
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
      .Where(type => type.BaseType != null && type.BaseType.IsGenericType
      && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfigurationBase<>)
    );
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            base.OnModelCreating(modelBuilder);
        }
        public EfDbContextTest(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
        }
    }

    public class DbContextConfiguration : DbConfiguration
    {
        public DbContextConfiguration()
        {
            SetProviderServices(
                "System.Data.SqlClient",
                SqlProviderServices.Instance
            );
        }
    }
}
