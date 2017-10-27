using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using NUnit.Framework;
using Orchard.Data.EntityFramework.Uow;
using Orchard.Data.EntityFramework.Uow.Transaction;
using Orchard.Domain.Uow;
using Orchard.Domain.Uow.Transaction;
using Orchard.Tests.Common.Domain.Entities;
using Orchard.Utility;
using System;
using System.Data.Entity;

namespace Orchard.Data.EntityFramework.Tests.Uow
{
    [Category("EfTransaction")]
    public class EfTransactionUow_Tests : EfNUnitTestBase
    {
        DbSet<AdmAreaTest> admAreaTable;
        ITransactionUnitOfWork<Database> transUnitOfWork;
        ITransactionInterceptor transInterceptor;
        ILogTest logTest;
        IUowTransaction uowTrans;
        protected override void Init()
        {
            base.Init();
            admAreaTable = DbContext.Set<AdmAreaTest>();
            transUnitOfWork = Container.Resolve<ITransactionUnitOfWork<Database>>();
            transInterceptor = Container.Resolve<ITransactionInterceptor>();
            logTest = Container.Resolve<ILogTest>();
            uowTrans = Container.Resolve<IUowTransaction>();
        }
        protected override void Register(ContainerBuilder builder)
        {
            builder.RegisterType<EfTransactionUnitOfWork>().As<ITransactionUnitOfWork<Database>>();
            builder.RegisterType<EfTransactionInterceptor>().As<ITransactionInterceptor>();
            builder.RegisterType<UowTransaction>().As<IUowTransaction>().EnableInterfaceInterceptors();
            builder.RegisterType<DefaultLogTest>().As<ILogTest>().EnableInterfaceInterceptors();
            builder.RegisterType<LogTestInterceptor>();
        }
        [Test]
        public void ITransactionCompleteHandle_Commit_Test()
        {
            // TODO: Add your test code here
            var entity = CreateAdmAreaTest();
            using (var transHandle = transUnitOfWork.Begin(DbContext.Database))
            {
                var insertResult = admAreaTable.Add(entity);
                DbContext.SaveChanges();
                Console.WriteLine("insertResult:" + insertResult.Id);
                var deleteResult = admAreaTable.Remove(entity);
                Console.WriteLine("deleteResult:" + deleteResult.Id);
                DbContext.SaveChanges();
                transHandle.Commit();
            }
        }
        [Test]
        public void ITransactionCompleteHandle_Rollback_Test()
        {
            // TODO: Add your test code here
            var entity = CreateAdmAreaTest();
            using (var transHandle = transUnitOfWork.Begin(DbContext.Database))
            {
                var insertResult = admAreaTable.Add(entity);
                DbContext.SaveChanges();
                Console.WriteLine("insertResult:" + insertResult.Id); ;
                transHandle.Rollback();
            }
        }
        [Test]
        public void UowTransaction_Commit_Test()
        {
            var entity = CreateAdmAreaTest();
            var proxyGenerate = new ProxyGenerator();
            IInterceptor intercept = transInterceptor;
            var objProxy = proxyGenerate.CreateClassProxy<TransactionInterceptorClass>(intercept);
            objProxy.AdmAreaTestTransactionTest(entity, admAreaTable);
        }
        [Test]
        public void DynamicProxy_Interceptor_Test()
        {
            var entity = CreateAdmAreaTest();
            var proxyGenerate = new ProxyGenerator();
            IInterceptor intercept = new TestIntercept();
            var objProxy = proxyGenerate.CreateClassProxy<InterceptClass>(intercept);
            objProxy.InterceptMethod("HelloWord");
            objProxy.InterceptMethod();
            objProxy.AdmAreaTestTransactionTest(entity, admAreaTable);
        }
        [Test]
        public void UowTransactionAttribute_Commit_Test()
        {
            var entity = CreateAdmAreaTest();
            uowTrans.AdmAreaTestTransactionTest(entity, admAreaTable);
        }

        [Test]
        public void LogTest_Test()
        {
            logTest.WriteLine("Hello LogTest");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected AdmAreaTest CreateAdmAreaTest()
        {
            AdmAreaTest entity = new AdmAreaTest()
            {
                AreaId = RandomData.GetString(maxLength: 50),//AreaID
                AreaName = RandomData.GetString(maxLength: 50),//AreaName
                ParentId = RandomData.GetString(maxLength: 50),//ParentId
                ShortName = RandomData.GetString(maxLength: 50),//ShortName
                LevelType = default(byte),//LevelType
                CityCode = RandomData.GetString(maxLength: 50),//CityCode
                ZipCode = RandomData.GetString(maxLength: 50),//ZipCode
                AreaNamePath = RandomData.GetString(maxLength: 500),//AreaNamePath
                AreaIdPath = RandomData.GetString(maxLength: 500),//AreaIDPath
                Lng = RandomData.GetDecimal(-(int)Math.Pow(2, 18), (int)Math.Pow(2, 18)),//lng
                Lat = RandomData.GetDecimal(-(int)Math.Pow(2, 18), (int)Math.Pow(2, 18)),//Lat
                PinYin = RandomData.GetString(maxLength: 50),//PinYin
                ShortPinYin = RandomData.GetString(maxLength: 20),//ShortPinYin
                PYFirstLetter = RandomData.GetString(maxLength: 10),//PYFirstLetter
                SortOrder = RandomData.GetInt(),//SortOrder
                Status = default(byte),//Status
                CreateTime = RandomData.GetDateTime(),//CreateTime
                CreateUser = RandomData.GetInt(),//CreateUser
                UpdateTime = RandomData.GetDateTime(),//UpdateTime
                UpdateUser = RandomData.GetInt(),//UpdateUser
                DeleteFlag = false,//DeleteFlag
                DeleteTime = RandomData.GetDateTime(),//DeleteTime
                DeleteUser = RandomData.GetInt(),//DeleteUser
            };
            return entity;
        }
    }
    public class TestIntercept : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.MethodInvocationTarget != null)
            {
                Console.WriteLine("Intercept,invocation.MethodInvocationTarget:" + invocation.MethodInvocationTarget.Name);
            }
            Console.WriteLine("Intercept,Start,invocation.Method.Name:" + invocation.Method.Name);
            invocation.Proceed();
            Console.WriteLine("Intercept,End, invocation.Method.Name:" + invocation.Method.Name);
        }
    }
    public class InterceptClass
    {
        public virtual void InterceptMethod(string name)
        {
            Console.WriteLine("Intercept Mehod name:" + name);
        }
        public virtual void InterceptMethod()
        {
            Console.WriteLine("Intercept Mehod");
        }
        [UowTransactionAttribute]
        public virtual void AdmAreaTestTransactionTest(AdmAreaTest entity, DbSet<AdmAreaTest> admAreaTable)
        {
            var insertResult = admAreaTable.Add(entity);
            if (!admAreaTable.Local.Contains(entity))
            {
                admAreaTable.Attach(entity);
            }
            admAreaTable.Remove(entity);
            Console.WriteLine("Intercept AdmAreaTestTransactionTest");
        }
    }
    public class TransactionInterceptorClass
    {
        //virtual 是必需的
        [UowTransactionAttribute]
        public virtual void AdmAreaTestTransactionTest(AdmAreaTest entity, DbSet<AdmAreaTest> admAreaTable)
        {
            var insertResult = admAreaTable.Add(entity);
            if (!admAreaTable.Local.Contains(entity))
            {
                admAreaTable.Attach(entity);
            }
            admAreaTable.Remove(entity);
            Console.WriteLine("TransactionInterceptor,AdmAreaTestTransactionTest");
        }
    }
    [Intercept(typeof(ITransactionInterceptor))]
    public interface IUowTransaction
    {
        void AdmAreaTestTransactionTest(AdmAreaTest entity, DbSet<AdmAreaTest> admAreaTable);
    }
    public class UowTransaction : IUowTransaction
    {
        [UowTransaction]
        public virtual void AdmAreaTestTransactionTest(AdmAreaTest entity, DbSet<AdmAreaTest> admAreaTable)
        {
            var insertResult = admAreaTable.Add(entity);
            if (!admAreaTable.Local.Contains(entity))
            {
                admAreaTable.Attach(entity);
            }
            admAreaTable.Remove(entity);
            Console.WriteLine("UowTransactionAttribute,AdmAreaTestTransactionTest");
        }
    }

    public interface ILogTest
    {
        void WriteLine(String line);
    }
    [Intercept(typeof(LogTestInterceptor))]
    public class DefaultLogTest : ILogTest
    {
        public void WriteLine(string line)
        {
            Console.WriteLine("Default Logger : {0}", line);
        }
    }
    public class LogTestInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("LogTestInterceptor");
            invocation.Proceed();
        }
    }
}
