using Autofac;
using NUnit.Framework;
using Orchard.Data.EntityFramework.Tests.Entities;
using Orchard.Utility.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.EntityFramework.Tests.Repositories
{
    [TestFixture]
    public class AdmUserRights_Repository_Tests : EfNUnitTestBase
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
            /*exec sp_executesql N'SELECT 
    [Limit1].[UserRightID] AS[UserRightID], 
    [Limit1].[UserID] AS[UserID], 
    [Limit1].[RightsID] AS[RightsID], 
    [Limit1].[CreateTime] AS[CreateTime], 
    [Limit1].[CreateUser] AS[CreateUser], 
    [Limit1].[DeleteFlag] AS[DeleteFlag], 
    [Limit1].[DeleteUser] AS[DeleteUser], 
    [Limit1].[DeleteTime] AS[DeleteTime], 
    [Limit1].[RightsID1] AS[RightsID1], 
    [Limit1].[RightsCode] AS[RightsCode], 
    [Limit1].[RightsName] AS[RightsName], 
    [Limit1].[RightsTypeID] AS[RightsTypeID], 
    [Limit1].[Description] AS[Description], 
    [Limit1].[SortOrder] AS[SortOrder], 
    [Limit1].[RightsType] AS[RightsType], 
    [Limit1].[CreateTime1] AS[CreateTime1], 
    [Limit1].[CreateUser1] AS[CreateUser1], 
    [Limit1].[UpdateTime] AS[UpdateTime], 
    [Limit1].[UpdateUser] AS[UpdateUser], 
    [Limit1].[DeleteFlag1] AS[DeleteFlag1], 
    [Limit1].[DeleteUser1] AS[DeleteUser1], 
    [Limit1].[DeleteTime1] AS[DeleteTime1], 
    [Limit1].[RightsTypeID1] AS[RightsTypeID1], 
    [Limit1].[TypeName] AS[TypeName], 
    [Limit1].[ParentID] AS[ParentID], 
    [Limit1].[SortOrder1] AS[SortOrder1], 
    [Limit1].[IDPath] AS[IDPath], 
    [Limit1].[NamePath] AS[NamePath], 
    [Limit1].[CreateTime2] AS[CreateTime2], 
    [Limit1].[CreateUser2] AS[CreateUser2], 
    [Limit1].[UpdateTime1] AS[UpdateTime1], 
    [Limit1].[UpdateUser1] AS[UpdateUser1], 
    [Limit1].[DeleteFlag2] AS[DeleteFlag2], 
    [Limit1].[DeleteUser2] AS[DeleteUser2], 
    [Limit1].[DeleteTime2]
        AS[DeleteTime2]
FROM(SELECT TOP (1)
        [Extent1].[UserRightID] AS[UserRightID], 
        [Extent1].[UserID] AS[UserID], 
        [Extent1].[RightsID] AS[RightsID], 
        [Extent1].[CreateTime] AS[CreateTime], 
        [Extent1].[CreateUser] AS[CreateUser], 
        [Extent1].[DeleteFlag] AS[DeleteFlag], 
        [Extent1].[DeleteUser] AS[DeleteUser], 
        [Extent1].[DeleteTime] AS[DeleteTime], 
        [Extent2].[RightsID] AS[RightsID1], 
        [Extent2].[RightsCode] AS[RightsCode], 
        [Extent2].[RightsName] AS[RightsName], 
        [Extent2].[RightsTypeID] AS[RightsTypeID], 
        [Extent2].[Description] AS[Description], 
        [Extent2].[SortOrder] AS[SortOrder], 
        [Extent2].[RightsType] AS[RightsType], 
        [Extent2].[CreateTime] AS[CreateTime1], 
        [Extent2].[CreateUser] AS[CreateUser1], 
        [Extent2].[UpdateTime] AS[UpdateTime], 
        [Extent2].[UpdateUser] AS[UpdateUser], 
        [Extent2].[DeleteFlag] AS[DeleteFlag1], 
        [Extent2].[DeleteUser] AS[DeleteUser1], 
        [Extent2].[DeleteTime] AS[DeleteTime1], 
        [Extent3].[RightsTypeID] AS[RightsTypeID1], 
        [Extent3].[TypeName] AS[TypeName], 
        [Extent3].[ParentID] AS[ParentID], 
        [Extent3].[SortOrder] AS[SortOrder1], 
        [Extent3].[IDPath] AS[IDPath], 
        [Extent3].[NamePath] AS[NamePath], 
        [Extent3].[CreateTime] AS[CreateTime2], 
        [Extent3].[CreateUser] AS[CreateUser2], 
        [Extent3].[UpdateTime] AS[UpdateTime1], 
        [Extent3].[UpdateUser] AS[UpdateUser1], 
        [Extent3].[DeleteFlag] AS[DeleteFlag2], 
        [Extent3].[DeleteUser] AS[DeleteUser2], 
        [Extent3].[DeleteTime]
        AS[DeleteTime2]
FROM[dbo].[wt_adm_user_rights]
        AS[Extent1]
INNER JOIN[dbo].[wt_adm_rights] AS[Extent2] ON[Extent1].[RightsID] = [Extent2].[RightsID]
        INNER JOIN[dbo].[wt_adm_rights_type] AS[Extent3] ON[Extent2].[RightsTypeID] = [Extent3].[RightsTypeID]
        WHERE[Extent1].[UserRightID] = @p__linq__0
    )  AS[Limit1]',N'@p__linq__0 int',@p__linq__0=2
            */
            Expression<Func<AdmUserRightsExt, bool>> expression = a => a.Id == id;
            DbSet<AdmUserRightsExt> Table = DbContext.Set<AdmUserRightsExt>();
            AdmUserRightsExt admUserRightsExt = Table
                .Include(t=>t.AdmRights)
                .Include(t => t.AdmRights.AdmRightsType)
               .FirstOrDefault(expression);
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
             * SELECT 
    [Extent1].[UserRightID] AS [UserRightID], 
    [Extent1].[RightsID] AS [RightsID], 
    [Extent1].[UserID] AS [UserID], 
    [Extent1].[CreateTime] AS [CreateTime], 
    [Extent1].[CreateUser] AS [CreateUser], 
    [Extent1].[DeleteFlag] AS [DeleteFlag], 
    [Extent1].[DeleteUser] AS [DeleteUser], 
    [Extent1].[DeleteTime] AS [DeleteTime], 
    [Extent2].[RightsID] AS [RightsID1], 
    [Extent2].[RightsCode] AS [RightsCode], 
    [Extent2].[RightsName] AS [RightsName], 
    [Extent2].[RightsTypeID] AS [RightsTypeID], 
    [Extent2].[Description] AS [Description], 
    [Extent2].[SortOrder] AS [SortOrder], 
    [Extent2].[RightsType] AS [RightsType], 
    [Extent2].[CreateTime] AS [CreateTime1], 
    [Extent2].[CreateUser] AS [CreateUser1], 
    [Extent2].[UpdateTime] AS [UpdateTime], 
    [Extent2].[UpdateUser] AS [UpdateUser], 
    [Extent2].[DeleteFlag] AS [DeleteFlag1], 
    [Extent2].[DeleteUser] AS [DeleteUser1], 
    [Extent2].[DeleteTime] AS [DeleteTime1], 
    [Extent3].[RightsTypeID] AS [RightsTypeID1], 
    [Extent3].[TypeName] AS [TypeName], 
    [Extent3].[ParentID] AS [ParentID], 
    [Extent3].[SortOrder] AS [SortOrder1], 
    [Extent3].[IDPath] AS [IDPath], 
    [Extent3].[NamePath] AS [NamePath], 
    [Extent3].[CreateTime] AS [CreateTime2], 
    [Extent3].[CreateUser] AS [CreateUser2], 
    [Extent3].[UpdateTime] AS [UpdateTime1], 
    [Extent3].[UpdateUser] AS [UpdateUser1], 
    [Extent3].[DeleteFlag] AS [DeleteFlag2], 
    [Extent3].[DeleteUser] AS [DeleteUser2], 
    [Extent3].[DeleteTime] AS [DeleteTime2]
    FROM   [dbo].[wt_adm_user_rights] AS [Extent1]
    INNER JOIN [dbo].[wt_adm_rights] AS [Extent2] ON [Extent1].[RightsID] = [Extent2].[RightsID]
    INNER JOIN [dbo].[wt_adm_rights_type] AS [Extent3] ON [Extent2].[RightsTypeID] = [Extent3].[RightsTypeID]
    WHERE 1 = [Extent1].[UserID]
    ORDER BY [Extent1].[RightsID] ASC, [Extent1].[UserRightID] DESC
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
