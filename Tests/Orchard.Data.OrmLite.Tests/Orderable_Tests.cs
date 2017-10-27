using NUnit.Framework;
using Orchard.Data.OrmLite.Tests.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using System.Linq.Expressions;

namespace Orchard.Data.OrmLite.Tests
{
    [TestFixture]
    public class Orderable_Tests:OrmLiteNUnitTestBase
    {
        protected override void Register(ContainerBuilder builder)
        {

        }
        private Domain.Repositories.IRepository<AdmArea> _admAreaRepository;

        protected override void Init()
        {
            base.Init();
            _admAreaRepository = Container.Resolve<Domain.Repositories.IRepository<AdmArea>>();
        }


        [Test]
        public void Orderable_Test()
        {
            Expression<Func<AdmArea, bool>> predicate = x => x.Id >= 100 && x.Id < 104;
            Action<Orderable<AdmArea>> orderAction = (o =>
            {
                o.Desc(x => x.AreaID,x=>x.Id)
                 .Asc(x => x.CreateUser);
            });
            var query = _admAreaRepository.GetList(predicate);
            var orderable = new Orderable<AdmArea>(query.AsQueryable());
            orderAction(orderable);
           
        }
    }
}
