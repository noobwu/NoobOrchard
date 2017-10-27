using Autofac;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.EntityFrameworkCore.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class EfCoreTestBase
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
        protected EfCoreDbContextTest DbContext;
        protected  virtual void Init()
        {
            var optionsBuilder = new DbContextOptionsBuilder<EfCoreDbContextTest>();
            string connectionString =ConfigurationManager.ConnectionStrings["Test"].ConnectionString;
            optionsBuilder.UseSqlServer(connectionString);
            DbContext = new EfCoreDbContextTest(optionsBuilder.Options);
            var builder = new ContainerBuilder();
            builder.Register<EfCoreDbContext>(c => DbContext).InstancePerDependency();
            builder.RegisterModule(new EntityFrameworkCoreModule());
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
