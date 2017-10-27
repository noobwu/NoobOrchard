using NUnit.Framework;
using Orchard.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Orchard.Data.EntityFramework.Uow;
using Orchard.Data.EntityFramework.Tests.Entities;
using Orchard.Utility;
using System.Transactions;

namespace Orchard.Data.EntityFramework.Tests.Uow
{
    [TestFixture]
    public class UnitOfWorkManager_Tests : EfNUnitTestBase
    {
        protected override void Init()
        {
            base.Init();
        }
        /// <summary>
        /// 这种方式不能回滚
        /// </summary>
        [Test]
        public void DbContextEfTransactionStrategy_Complete_Test()
        {
            var unitOfWork = new EfUnitOfWork(new DbContextEfTransactionStrategy(), new UnitOfWorkDefaultOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted  });
            unitOfWork.ActiveDbContexts.Add(DbContext.Database.Connection.ConnectionString, DbContext);
            IUnitOfWorkManager  _unitOfWorkManager = new UnitOfWorkManager(unitOfWork);
            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin())
            {
                var admAreaTable = DbContext.Set<AdmArea>();
                var insertResult = admAreaTable.Add(CreateAdmArea());
                //dbContext.SaveChanges();//这种方式不能自动回滚需要补偿回滚
                if (insertResult.Id > 0)
                {
                    Console.WriteLine("insertResult,Id:" + insertResult.Id);
                    var removeResult = admAreaTable.Remove(insertResult);
                    Console.WriteLine("removeResult,Id:" + removeResult.Id);
                    //uow.Complete();
                }
            }
        }
        /// <summary>
        /// 这种方式不能回滚
        /// </summary>
        [Test]
        public void TransactionScopeEfTransactionStrategy_Complete_Test()
        {
            var unitOfWork = new EfUnitOfWork(new TransactionScopeEfTransactionStrategy(), new UnitOfWorkDefaultOptions() { IsolationLevel=System.Transactions.IsolationLevel.ReadUncommitted});
            unitOfWork.ActiveDbContexts.Add(DbContext.Database.Connection.ConnectionString, DbContext);
            IUnitOfWorkManager _unitOfWorkManager = new UnitOfWorkManager(unitOfWork);
            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin())
            {
                var admAreaTable = DbContext.Set<AdmArea>();
                var insertResult = admAreaTable.Add(CreateAdmArea());
                //dbContext.SaveChanges();//这种方式不能自动回滚需要补偿回滚
                if (insertResult.Id > 0)
                {
                    Console.WriteLine("insertResult,Id:" + insertResult.Id);
                    var removeResult = admAreaTable.Remove(insertResult);
                    Console.WriteLine("removeResult,Id:" + removeResult.Id);
                    //uow.Complete();
                }
            }
        }
        /// <summary>
        /// 这种方式可以回滚
        /// </summary>
        [Test]
        public void Database_BeginTransaction_Test()
        {
            using (var dbContextTransaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    var admAreaTable = DbContext.Set<AdmArea>();
                    var insertResult = admAreaTable.Add(CreateAdmArea());
                    DbContext.SaveChanges();
                    if (insertResult.Id > 0)
                    {
                        Console.WriteLine("insertResult,Id:" + insertResult.Id);
                        //var removeResult = admAreaTable.Remove(insertResult);
                        //Console.WriteLine("removeResult,Id:" + removeResult.Id);
                        //dbContext.SaveChanges();
                        //dbContextTransaction.Commit();
                        dbContextTransaction.Rollback();
                    }
                    else
                    {
                        dbContextTransaction.Rollback();
                    }
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback();
                }
            }

        }
        [Test]
        public void TransactionScope_Test()
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var admAreaTable = DbContext.Set<AdmArea>();
                    var insertResult = admAreaTable.Add(CreateAdmArea());
                    DbContext.SaveChanges();
                    if (insertResult.Id > 0)
                    {
                        Console.WriteLine("insertResult,Id:" + insertResult.Id);
                        var removeResult = admAreaTable.Remove(insertResult);
                        Console.WriteLine("removeResult,Id:" + removeResult.Id);
                        DbContext.SaveChanges();
                        //提交事务
                        //transaction.Complete();
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }
        }

        protected override void Register(ContainerBuilder builder)
        {
            //IocManager.RegisterIfNot<IEfTransactionStrategy, DbContextEfTransactionStrategy>(DependencyLifeStyle.Transient);
            builder.RegisterInstance<IUnitOfWork>(new EfUnitOfWork(new DbContextEfTransactionStrategy(), new UnitOfWorkDefaultOptions()));
        }
    }
}
