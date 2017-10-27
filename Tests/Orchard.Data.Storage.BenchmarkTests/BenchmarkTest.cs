using BenchmarkDotNet.Attributes;
using MySql.Data.MySqlClient;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.Storage.BenchmarkTests
{
    public class BenchmarkTest : BenchmarkBase
    {
        /// <summary>
        /// 
        /// </summary>
        private string sqlServerConnString;
        /// <summary>
        /// 
        /// </summary>
        private string mysqlConnString;
        /// <summary>
        /// 
        /// </summary>
        private string redisConnString;

        [GlobalCleanup]
        public void Cleanup()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            sqlServerConnString = ConfigurationManager.ConnectionStrings["Test"].ConnectionString;
            mysqlConnString = ConfigurationManager.ConnectionStrings["MySqlTest"].ConnectionString;
            redisConnString = ConfigurationManager.AppSettings["RedisTest"];
        }
        [Benchmark(OperationsPerInvoke = BenchmarkBase.OperationsPerInvoke)]
        public void SqlConnectionBenchmark()
        {
            //SqlConnection connection = new SqlConnection(sqlServerConnString);
            //connection.Open();
            Console.WriteLine("SqlConnectionBenchmark:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") + ",ManagedThreadId:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            //System.Threading.Thread.Sleep(1000);

        }

        [Benchmark(OperationsPerInvoke = BenchmarkBase.OperationsPerInvoke)]
        public void MySqlConnectionBenchmark()
        {
            //MySqlConnection connection = new MySqlConnection(mysqlConnString);
            //connection.Open();
            Console.WriteLine("MySqlConnectionBenchmark:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") + ",ManagedThreadId:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            //System.Threading.Thread.Sleep(1000);

        }

        [Benchmark(OperationsPerInvoke = BenchmarkBase.OperationsPerInvoke)]
        public void RedisConnectionBenchmark()
        {
            //ConnectionMultiplexer connectionMultiplexer= ConnectionMultiplexer.Connect(redisConnString);
            Console.WriteLine("RedisConnectionBenchmark:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") + ",ManagedThreadId:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            //System.Threading.Thread.Sleep(1000);

        }
    }
}
