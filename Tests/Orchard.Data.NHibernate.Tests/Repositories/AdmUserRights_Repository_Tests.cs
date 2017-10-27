using Autofac;
using NHibernate;
using NHibernate.Linq;
using NUnit.Framework;
using Orchard.Data.NHibernate.Tests.Entities;
using Orchard.Utility.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.NHibernate.Tests.Repositories
{
    [TestFixture]
    public class AdmUserRights_Repository_Tests : NHibernateTestsBase
    {
        public override void Register(ContainerBuilder builder)
        {

        }
        private Domain.Repositories.IRepository<AdmUserRights> _admUserRightsRepository;
        public override void Init()
        {
            base.Init();
            _admUserRightsRepository = _container.Resolve<Domain.Repositories.IRepository<AdmUserRights>>();
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
            AdmRights rightsAlias = null;
            Expression<Func<AdmUserRightsExt, bool>> expression = a => a.Id >id;
            AdmUserRightsExt admUserRightsExt = null;
            using (ISession session =_sessionFactory.OpenSession())
            {
                admUserRightsExt = session.QueryOver<AdmUserRightsExt>()
              .JoinAlias(t => t.AdmRights, () => rightsAlias)
              .Where(t=>t.RightsID==rightsAlias.RightsID)
              .Where(expression).SingleOrDefault();
            }
          
            if (admUserRightsExt != null)

            {
                Console.WriteLine("Id:" + admUserRightsExt.Id + ",UserRightID:" + admUserRightsExt.UserRightID);
                Console.WriteLine("AdmRights,Id:" + admUserRightsExt.AdmRights.Id + ",UserRightID:" + admUserRightsExt.AdmRights.RightsID);
            }
            //Console.WriteLine(JsonSerializationHelper.SerializeByDefault(admUserRightsExt));
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
            AdmRights rightsAlias = null;
            IList<AdmUserRightsExt> list = null;
            Expression<Func<AdmUserRightsExt, bool>> expression = a => a.UserID > 1;
            using (ISession session = _sessionFactory.OpenSession())
            {
                list = session.QueryOver<AdmUserRightsExt>()
              .JoinQueryOver(t => t.AdmRights)
              .Where(t => t.RightsID == rightsAlias.RightsID).List();
            }
            Console.WriteLine(JsonSerializationHelper.SerializeByDefault(list));
        }
    }
}
