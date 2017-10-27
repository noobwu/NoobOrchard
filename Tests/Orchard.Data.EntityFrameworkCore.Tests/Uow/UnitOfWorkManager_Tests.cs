using NUnit.Framework;
using Orchard.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Orchard.Utility;
using System.Transactions;
using Orchard.Data.EntityFrameworkCore.Tests.Entities;

namespace Orchard.Data.EntityFrameworkCore.Tests.Uow
{
    [TestFixture]
    public class UnitOfWorkManager_Tests : EfCoreNUnitTestBase
    {
        protected override void Init()
        {
            base.Init();
        }
        protected override void Register(ContainerBuilder builder)
        {

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
                    var insertResult = admAreaTable.Add(CreateAdmArea()).Entity;
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
                    var insertResult = admAreaTable.Add(CreateAdmArea()).Entity;
                    DbContext.SaveChanges();
                    if (insertResult.Id > 0)
                    {
                        Console.WriteLine("insertResult,Id:" + insertResult.Id);
                        var removeResult = admAreaTable.Remove(insertResult).Entity;
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


    }
}
