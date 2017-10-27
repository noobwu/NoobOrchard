using Autofac;
using Orchard.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Repositories.BenchmarkTests.EntityFramework
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class EfParallelBenchmarkBase : ParallelBenchmarkBase
    {
        protected EfDbContextTest DbContext;
        public virtual void Init()
        {
            DbConnection connection = new SqlConnection(ConnectionString);
            DbContext = new EfDbContextTest(connection);
            var builder = new ContainerBuilder();
            builder.Register<EfDbContext>(c => DbContext).InstancePerDependency();
            builder.RegisterModule(new EntityFrameworkModule());
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
