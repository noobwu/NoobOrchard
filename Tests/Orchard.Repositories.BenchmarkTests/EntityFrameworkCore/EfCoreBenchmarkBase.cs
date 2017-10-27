using Autofac;
using Microsoft.EntityFrameworkCore;
using Orchard.Data.EntityFrameworkCore;
using System.Configuration;

namespace Orchard.Repositories.BenchmarkTests.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class EfCoreBenchmarkBase:BenchmarkBase
    {
        protected EfCoreDbContextTest DbContext;
        public  virtual void Init()
        {
            var optionsBuilder = new DbContextOptionsBuilder<EfCoreDbContextTest>();
            optionsBuilder.UseSqlServer(ConnectionString);
            DbContext = new EfCoreDbContextTest(optionsBuilder.Options);
            var builder = new ContainerBuilder();
            builder.Register<EfCoreDbContext>(c => DbContext).InstancePerDependency();
            builder.RegisterModule(new EntityFrameworkCoreModule());
            Register(builder);
            Container = builder.Build();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Cleanup()
        {
            DbContext?.Dispose();
            base.Cleanup();
        }
    }
}
