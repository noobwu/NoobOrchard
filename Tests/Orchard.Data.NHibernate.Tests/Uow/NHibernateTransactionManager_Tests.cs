using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Orchard.Domain.Uow;
using Orchard.Data.NHibernate.Tests.Entities;
using Orchard.Utility;
using Orchard.Data.NHibernate.Uow;

namespace Orchard.Data.NHibernate.Tests.Uow
{
    [TestFixture]
    public class NHibernateTransactionManager_Tests : NHibernateTestsBase
    {
        private ITransactionManager _transactionManager;
        public override void Register(ContainerBuilder builder)
        {

        }
        public override void Init()
        {
            base.Init();
            _transactionManager = new NHibernateTransactionManager(_session);
        }
        [Test]
        public void RequireNew_Test()
        {
            // TODO: Add your test code here
            var entity = CreateAdmArea();
            _transactionManager.Begin();
            var insertResult = _session.Save(entity);
            Console.WriteLine("insertResult:"+ insertResult);
            var deleteResult = _session.Delete(string.Format("from {0} where AreaID={1}", typeof(AdmArea).FullName, entity.AreaID));
            Console.WriteLine("deleteResult:" + insertResult);
            _transactionManager.Commit();
        }
    }
}
