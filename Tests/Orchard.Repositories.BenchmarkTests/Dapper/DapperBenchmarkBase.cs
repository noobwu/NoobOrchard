using Autofac;
using Orchard.Data;
using Orchard.Data.Dapper;
using Orchard.Repositories.BenchmarkTests.Dapper.Mappings;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Orchard.Repositories.BenchmarkTests.Dapper
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DapperBenchmarkBase : BenchmarkBase
    {
        protected IDbConnection Connection;
        public virtual void Init()
        {
            //var assemblies = new List<Assembly>() { Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory+"\\Orchard.Data.Dapper.Tests.dll") };
            //var assemblies = new List<Assembly>() { Assembly.GetExecutingAssembly() };
            var assemblies = new List<Assembly>() { Assembly.GetAssembly(typeof(AdmAreaTestMap)) };
            DapperExtensions.DapperExtensions.SetMappingAssemblies(assemblies);
            DapperExtensions.DapperAsyncExtensions.SetMappingAssemblies(assemblies);
            //var connection = new SqlConnection(ConnectionString);
            var DbFactory = new OrchardConnectionFactory(ConnectionString, SqlServerDialect.Provider);
            var builder = new ContainerBuilder();
            //builder.Register<IActiveTransactionProvider>(c => new DapperTransactionProvider(connection)).InstancePerDependency();
            builder.Register(c => DbFactory).InstancePerDependency();
            builder.RegisterModule(new DapperModule());
            Register(builder);
            Container = builder.Build();
        }

    }
}
