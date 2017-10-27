using Autofac;
using Orchard.Data.Dapper.Tests.Mappings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Orchard.Data.Dapper.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DapperTestBase
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
        //protected IDbConnection Connection;
        protected OrchardConnectionFactory DbFactory;
        protected virtual void Init()
        {
             //var assemblies = new List<Assembly>() { Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory+"\\Orchard.Data.Dapper.Tests.dll") };
            //var assemblies = new List<Assembly>() { Assembly.GetExecutingAssembly() };
            var assemblies = new List<Assembly>() { Assembly.GetAssembly(typeof(AdmAreaMap)) };
            DapperExtensions.DapperExtensions.SetMappingAssemblies(assemblies);
            DapperExtensions.DapperAsyncExtensions.SetMappingAssemblies(assemblies);
            string connectionString =ConfigurationManager.ConnectionStrings["Test"].ConnectionString;
            DbFactory = new OrchardConnectionFactory(connectionString, SqlServerDialect.Provider);
            var builder = new ContainerBuilder();
            //builder.Register<IActiveTransactionProvider>(c => new DapperTransactionProvider(connection)).InstancePerDependency();
            builder.Register(c => DbFactory).InstancePerDependency();
            builder.RegisterModule(new DapperModule());
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
