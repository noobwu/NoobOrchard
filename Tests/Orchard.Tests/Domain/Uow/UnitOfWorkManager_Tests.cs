using System.Transactions;
using NSubstitute;
using NUnit.Framework;
using Orchard.Domain.Uow;
using Autofac;

namespace Orchard.Tests.Domain.Uow
{
    public class UnitOfWorkManager_Tests : ContainerTestBase
    {
        IUnitOfWork fakeUow = null;
        IUnitOfWorkManager uowManager = null;
        public override void Init()
        {
            base.Init();
        }
        protected override void Register(ContainerBuilder builder) {
            fakeUow =Substitute.For<IUnitOfWork>() ;
            uowManager = new UnitOfWorkManager(new NullUnitOfWork(new UnitOfWorkDefaultOptions()));
        }
        [TestCase]
        public void Should_Call_Uow_Methods()
        {
            //Starting the first uow
            using (var uow1 = uowManager.Begin())
            {
                //so, begin will be called
                fakeUow.Received(1).Begin(Arg.Any<UnitOfWorkOptions>());

                //trying to begin a uow (not starting a new one, using the outer)
                using (var uow2 = uowManager.Begin())
                {
                    //Since there is a current uow, begin is not called
                    fakeUow.Received(1).Begin(Arg.Any<UnitOfWorkOptions>());

                    uow2.Complete();

                    //complete has no effect since outer uow should complete it
                    fakeUow.DidNotReceive().Complete();
                }

                //trying to begin a uow (forcing to start a NEW one)
                using (var uow2 = uowManager.Begin(TransactionScopeOption.RequiresNew))
                {
                    //So, begin is called again to create an inner uow
                    fakeUow.Received(2).Begin(Arg.Any<UnitOfWorkOptions>());

                    uow2.Complete();

                    //And the inner uow should be completed
                    fakeUow.Received(1).Complete();
                }

                //complete the outer uow
                uow1.Complete();
            }

            fakeUow.Received(2).Complete();
            fakeUow.Received(2).Dispose();
        }
    }
}
