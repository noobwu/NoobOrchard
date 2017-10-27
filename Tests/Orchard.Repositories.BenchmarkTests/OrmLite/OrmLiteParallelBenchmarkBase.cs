using Autofac;
using BenchmarkDotNet.Attributes;
using Orchard.Data.OrmLite;
using Orchard.Tests.Common.Domain.Entities;
using ServiceStack;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;
using System.Data;
using System.Configuration;
namespace Orchard.Repositories.BenchmarkTests.OrmLite
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class OrmLiteParallelBenchmarkBase : ParallelBenchmarkBase
    {
       
        protected OrmLiteConnectionFactory DbFactory;
        /// <summary>
        /// 
        /// </summary>
        [GlobalCleanup]
        public void GlobalCleanup()
        {
            Cleanup();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Cleanup()
        {
            DbFactory = null;
            base.Cleanup();
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void Init()
        {
            DbFactory = new OrmLiteConnectionFactory(ConnectionString, SqlServer2014Dialect.Provider);

            InitOrmLiteMappings();

            var builder = new ContainerBuilder();
            builder.Register(c => DbFactory).InstancePerDependency();
            builder.RegisterModule(new OrmLiteModule());
            Register(builder);
            Container = builder.Build();
        }
        /// <summary>
        /// 
        /// </summary>
        private void InitOrmLiteMappings()
        {
            var admAreaTestType = typeof(AdmAreaTest);
            admAreaTestType.AddAttributes(new Attribute[] { new AliasAttribute("wt_adm_area_test") });
            admAreaTestType.GetProperty("Id").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute(), new AliasAttribute("ID") });
        }
     
    }
}
