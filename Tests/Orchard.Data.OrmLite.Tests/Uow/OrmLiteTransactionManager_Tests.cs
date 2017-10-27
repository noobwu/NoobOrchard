using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Orchard.Domain.Uow;
using Orchard.Utility;
using Orchard.Data.OrmLite.Tests.Entities;
using ServiceStack.OrmLite;
using Orchard.Data.OrmLite.Uow;

namespace Orchard.Data.OrmLite.Tests.Uow
{
    [TestFixture]
    public class OrmLiteTransactionManager_Tests : OrmLiteNUnitTestBase
    {
        private ITransactionManager _transactionManager;
        protected override void Register(ContainerBuilder builder)
        {

        }
        protected override void Init()
        {
            base.Init();
            _transactionManager = new OrmLiteTransactionManager(Connection);
        }
        [Test]
        public void OrmLiteTransactionManager_Commit_Test()
        {
            // TODO: Add your test code here
            var entity = CreateAdmArea();
            Connection.Open();
            _transactionManager.Begin();
            var insertResult = Connection.Insert(entity,entity.IsTransient());
            Console.WriteLine("insertResult:" + insertResult);
            var deleteResult = Connection.Delete<AdmArea>(a => a.Id == insertResult);
            Console.WriteLine("deleteResult:" + deleteResult);
            _transactionManager.Commit();
        }
        [Test]
        public void OrmLiteTransactionManager_Rollback_Test()
        {
            // TODO: Add your test code here
            var entity = CreateAdmArea();
            Connection.Open();
            _transactionManager.Begin();
            var insertResult = Connection.Insert(entity,entity.IsTransient());
            Console.WriteLine("insertResult:" + insertResult);
            var deleteResult = Connection.Delete<AdmArea>(a => a.Id == insertResult);
            Console.WriteLine("deleteResult:" + deleteResult);
            _transactionManager.Rollback();
        }
    }
}
