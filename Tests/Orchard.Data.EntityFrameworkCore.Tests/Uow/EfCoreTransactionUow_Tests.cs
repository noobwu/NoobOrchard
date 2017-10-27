using Autofac;
using Autofac.Extras.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NUnit.Framework;
using Orchard.Data.EntityFrameworkCore.Uow;
using Orchard.Data.EntityFrameworkCore.Uow.Transaction;
using Orchard.Domain.Uow;
using Orchard.Domain.Uow.Transaction;
using Orchard.Tests.Common.Domain.Entities;
using System;

namespace Orchard.Data.EntityFrameworkCore.Tests.Uow
{
    [Category("EfCoreTransaction")]
    public class EfCoreTransactionUow_Tests : EfCoreNUnitTestBase
    {
        DbSet<AdmAreaTest> admAreaTable;
        ITransactionUnitOfWork<DatabaseFacade> transUnitOfWork;
        UowTransaction uowTrans;
        protected override void Init()
        {
            base.Init();
            admAreaTable = DbContext.Set<AdmAreaTest>();
            transUnitOfWork = Container.Resolve<ITransactionUnitOfWork<DatabaseFacade>>();
            uowTrans = Container.Resolve<UowTransaction>();
        }
        protected override void Register(ContainerBuilder builder)
        {
            builder.RegisterType<EfCoreTransactionUnitOfWork>().As<ITransactionUnitOfWork<DatabaseFacade>>();
            builder.RegisterType<EfCoreTransactionInterceptor>().As<ITransactionInterceptor>();
            builder.RegisterType<UowTransaction>().EnableClassInterceptors();
        }
        [Test]
        public void ITransactionCompleteHandle_Commit_Test()
        {
            // TODO: Add your test code here
            var entity = CreateAdmAreaTest();
            using (var transHandle = transUnitOfWork.Begin(DbContext.Database))
            {
                var insertResult = admAreaTable.Add(entity).Entity;
                DbContext.SaveChanges();
                Console.WriteLine("insertResult:" + insertResult.Id);
                var deleteResult = admAreaTable.Remove(entity).Entity;
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
                var insertResult = admAreaTable.Add(entity).Entity;
                DbContext.SaveChanges();
                Console.WriteLine("insertResult:" + insertResult.Id);
                transHandle.Rollback();
            }
        }

        [Test]
        public void UowTransactionAttribute_Commit_Test()
        {
            var entity = CreateAdmAreaTest();
            uowTrans.AdmAreaTestTransactionTest(entity, admAreaTable);
        }
    }

    [Intercept(typeof(ITransactionInterceptor))]
    public class UowTransaction
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
}
