using Autofac;
using NHibernate;
using NUnit.Framework;
using Orchard.Data.NHibernate.Uow;
using Orchard.Domain.Uow;
using Orchard.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.NHibernate.Tests.Uow
{
    [TestFixture]
    public class NHibernateTransactionUow_Tests : NHibernateTestsBase
    {
        public override void Register(ContainerBuilder builder)
        {
            builder.RegisterType<NHibernateTransactionUnitOfWork>().As<ITransactionUnitOfWork<ISession>>();
            builder.RegisterType<NHibernateTransactionCompleteHandle>().As<ITransactionCompleteHandle>();
        }
        public override void Init()
        {
            base.Init();

        }

        [Test]
        public void ITransactionCompleteHandle_Commit_Test()
        {
            // TODO: Add your test code here
            var transactionUnitOfWork = _container.Resolve<ITransactionUnitOfWork<ISession>>();
            var entity = CreateAdmArea();
            using (var transHandle= transactionUnitOfWork.Begin(_session))
            {
                var insertResult = _session.Save(entity);
                Console.WriteLine("insertResult:" + insertResult);
                _session.Delete(entity);
                transHandle.Commit();
            }
        }
        [Test]
        public void ITransactionCompleteHandle_Rollback_Test()
        {
            // TODO: Add your test code here
            var transactionUnitOfWork = _container.Resolve<ITransactionUnitOfWork<ISession>>();
            var entity = CreateAdmArea();
            using (var transHandle = transactionUnitOfWork.Begin(_session))
            {
                var insertResult = _session.Save(entity);
                Console.WriteLine("insertResult:" + insertResult);
                transHandle.Rollback();
            }
        }
    }
}
