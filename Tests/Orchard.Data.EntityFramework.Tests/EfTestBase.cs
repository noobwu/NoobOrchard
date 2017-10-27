using Autofac;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.EntityFramework.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class EfTestBase
    {
        /// <summary>
        /// Parallel开始索引（含）。
        /// </summary>
        protected const int ParallelFromExclusive = 0;
        /// <summary>
        /// Parallel结束索引（不含）。
        /// </summary>
        protected const int ParallelToExclusive = 1000;
        /// <summary>
        /// 
        /// </summary>
        protected const int PageIndex = 1;
        /// <summary>
        /// 
        /// </summary>
        protected const int PageSize = 20;
        protected IContainer Container;
        protected EfDbContextTest DbContext;
        protected virtual void Init()
        {
            string connectionString =ConfigurationManager.ConnectionStrings["Test"].ConnectionString;
            DbConnection connection = new SqlConnection(connectionString);
            DbContext = new EfDbContextTest(connection);
            var builder = new ContainerBuilder();
            builder.Register<EfDbContext>(c => DbContext).InstancePerDependency();
            builder.RegisterModule(new EntityFrameworkModule());
            Register(builder);
            Container = builder.Build();
        }

        protected virtual void Cleanup()
        {
            Container?.Dispose();
        }
        protected abstract void Register(ContainerBuilder builder);
    }
}
