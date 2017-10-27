using Autofac;
using NUnit.Framework;
using Orchard.Data.OrmLite.Tests.Entities;
using Orchard.Utility.Json;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.OrmLite.Tests.Repositories
{
    [TestFixture]
    public class AdmUserRights_Repository_Tests : OrmLiteNUnitTestBase
    {
        protected override void Register(ContainerBuilder builder)
        {

        }
        private Domain.Repositories.IRepository<AdmUserRights> _admUserRightsRepository;
        protected override void Init()
        {
            base.Init();
            _admUserRightsRepository = Container.Resolve<Domain.Repositories.IRepository<AdmUserRights>>();
        }

        /// <summary>
        /// Creates an entity with given primary key without database access.
        /// </summary>
        /// <param name="id">Primary key of the entity to load</param>
        /// <returns>Entity</returns>
        [TestCase(1)]
        public void AdmUserRights_Load_Test(int id)
        {
            //exec sp_executesql N'SELECT "ID", "AreaID", "AreaName", "ParentId", "ShortName", "LevelType", "CityCode", "ZipCode", "AreaNamePath", "AreaIDPath", "lng", "Lat", "PinYin", "ShortPinYin", "PYFirstLetter", "SortOrder", "Status", "CreateTime", "CreateUser", "UpdateTime", "UpdateUser" FROM "wt_adm_area" WHERE "ID" = @ID',N'@ID int',@ID=4
            AdmUserRights admUserRights = _admUserRightsRepository.Load(id);
            Assert.AreEqual(admUserRights.Id, id);
            Console.WriteLine("Id:" + admUserRights.Id + ",UserRightID:" + admUserRights.UserRightID);
            //Console.WriteLine(JsonSerializationHelper.SerializeByDefault(AdmArea));
        }
        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        [Test]
        public void AdmUserRights_GetAdmUserRightsExtList_Test()
        {
            Expression<Func<AdmUserRights, bool>> expression = a => a.UserID == 1;
            IOrderByExpression<AdmUserRights>[] orderByExpressions =
             { new OrderByExpression<AdmUserRights, int>(u => u.RightsID),    // a string, asc
    new OrderByExpression<AdmUserRights, int>(u => u.UserRightID, true)};
            List<AdmUserRightsExt> list = null;
            using (var db = DbFactory.OpenDbConnection())
            {
                SqlExpression<AdmUserRights> sqlExpression = db.From<AdmUserRights>()
                                        .LeftJoin<AdmRights>((ur, r) => ur.RightsID == r.RightsID)
                                        .LeftJoin<AdmRights, AdmRightsType>((r, rt) => r.RightsTypeID == rt.RightsTypeID)
                                        .Where(expression);
                sqlExpression = ApplySqlExpressionOrderBy(sqlExpression,orderByExpressions);
                list=db.Select<AdmUserRightsExt>(sqlExpression);
            }
            Console.WriteLine(JsonSerializationHelper.SerializeByDefault(list));
        }
    }
}
