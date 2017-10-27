using Autofac;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Orchard.Data.EntityFrameworkCore.Tests.Entities;
using Orchard.Utility.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.EntityFrameworkCore.Tests.Repositories
{
    [TestFixture]
    public class AdmUserRights_Repository_Tests : EfCoreNUnitTestBase
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
        [TestCase(2)]
        public void AdmUserRights_Load_Test(int id)
        {
            /*exec sp_executesql N'SELECT TOP(1) [t].[UserRightID], [t].[CreateTime], [t].[CreateUser], [t].[DeleteFlag], [t].[DeleteTime], [t].[DeleteUser], [t].[RightsID], [t].[UserID], [w].[RightsID], [w].[CreateTime], [w].[CreateUser], [w].[DeleteFlag], [w].[DeleteTime], [w].[DeleteUser], [w].[Description], [w].[RightsCode], [w].[RightsName], [w].[RightsType], [w].[RightsTypeID], [w].[SortOrder], [w].[UpdateTime], [w].[UpdateUser], [w0].[RightsTypeID], [w0].[CreateTime], [w0].[CreateUser], [w0].[DeleteFlag], [w0].[DeleteTime], [w0].[DeleteUser], [w0].[IDPath], [w0].[NamePath], [w0].[ParentID], [w0].[SortOrder], [w0].[TypeName], [w0].[UpdateTime], [w0].[UpdateUser], [w1].[RightsID], [w1].[CreateTime], [w1].[CreateUser], [w1].[DeleteFlag], [w1].[DeleteTime], [w1].[DeleteUser], [w1].[Description], [w1].[RightsCode], [w1].[RightsName], [w1].[RightsType], [w1].[RightsTypeID], [w1].[SortOrder], [w1].[UpdateTime], [w1].[UpdateUser]
FROM [wt_adm_user_rights] AS [t]
INNER JOIN [wt_adm_rights] AS [w] ON [t].[RightsID] = [w].[RightsID]
INNER JOIN [wt_adm_rights_type] AS [w0] ON [w].[RightsTypeID] = [w0].[RightsTypeID]
INNER JOIN [wt_adm_rights] AS [w1] ON [t].[RightsID] = [w1].[RightsID]
WHERE [t].[UserRightID] = @__id_0',N'@__id_0 int',@__id_0=2
            */
            Expression<Func<AdmUserRightsExt, bool>> predicate = a => a.Id == id;
            DbSet<AdmUserRightsExt> Table = DbContext.Set<AdmUserRightsExt>();
            AdmUserRightsExt admUserRightsExt = Table
                .Include(t=>t.AdmRights)
                .Include(t => t.AdmRights.AdmRightsType)
               .FirstOrDefault(predicate);
            Console.WriteLine("Id:" + admUserRightsExt.Id + ",UserRightID:" + admUserRightsExt.UserRightID);
            Console.WriteLine("AdmRights,Id:" + admUserRightsExt.AdmRights.Id + ",UserRightID:" + admUserRightsExt.AdmRights.RightsID);
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
            /*
            SELECT [t].[UserRightID], [t].[CreateTime], [t].[CreateUser], [t].[DeleteFlag], [t].[DeleteTime], [t].[DeleteUser], [t].[RightsID], [t].[UserID], [w].[RightsID], [w].[CreateTime], [w].[CreateUser], [w].[DeleteFlag], [w].[DeleteTime], [w].[DeleteUser], [w].[Description], [w].[RightsCode], [w].[RightsName], [w].[RightsType], [w].[RightsTypeID], [w].[SortOrder], [w].[UpdateTime], [w].[UpdateUser], [w0].[RightsTypeID], [w0].[CreateTime], [w0].[CreateUser], [w0].[DeleteFlag], [w0].[DeleteTime], [w0].[DeleteUser], [w0].[IDPath], [w0].[NamePath], [w0].[ParentID], [w0].[SortOrder], [w0].[TypeName], [w0].[UpdateTime], [w0].[UpdateUser], [w1].[RightsID], [w1].[CreateTime], [w1].[CreateUser], [w1].[DeleteFlag], [w1].[DeleteTime], [w1].[DeleteUser], [w1].[Description], [w1].[RightsCode], [w1].[RightsName], [w1].[RightsType], [w1].[RightsTypeID], [w1].[SortOrder], [w1].[UpdateTime], [w1].[UpdateUser]
FROM [wt_adm_user_rights] AS [t]
INNER JOIN [wt_adm_rights] AS [w] ON [t].[RightsID] = [w].[RightsID]
INNER JOIN [wt_adm_rights_type] AS [w0] ON [w].[RightsTypeID] = [w0].[RightsTypeID]
INNER JOIN [wt_adm_rights] AS [w1] ON [t].[RightsID] = [w1].[RightsID]
WHERE [t].[UserID] = 1
ORDER BY [t].[RightsID], [t].[UserRightID] DESC
             */
            Expression<Func<AdmUserRightsExt, bool>> predicate = a => a.UserID == 1;
            IOrderByExpression<AdmUserRightsExt>[] orderByExpressions =
             { new OrderByExpression<AdmUserRightsExt, int>(u => u.RightsID),    // a string, asc
    new OrderByExpression<AdmUserRightsExt, int>(u => u.Id, true)};
            DbSet<AdmUserRightsExt> Table = DbContext.Set<AdmUserRightsExt>();
            List<AdmUserRightsExt> list = ApplyOrderBy(Table
                .Include(t => t.AdmRights)
                .Include(t => t.AdmRights.AdmRightsType)
               .Where(predicate),orderByExpressions).ToList();

            Console.WriteLine(JsonSerializationHelper.SerializeByDefault(list));
        }
    }
}
