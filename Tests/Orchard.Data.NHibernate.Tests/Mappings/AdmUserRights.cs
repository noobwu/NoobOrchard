using Orchard.Data.NHibernate.EntityMappings;
using Orchard.Data.NHibernate.Tests.Entities;
using System;
namespace Orchard.Data.NHibernate.Tests.Mappings
{

     /// <summary>
    /// 系统用户权限
    /// </summary>
    [Serializable]
    public class AdmUserRightsMap:EntityMap<AdmUserRights,int>
    {
        /// <summary>
        /// 系统用户权限 Constructor.
        /// </summary>
        /// <param name="tableName">Table name</param>
        public AdmUserRightsMap():base("wt_adm_user_rights")
        {
             Id(x => x.UserRightID).GeneratedBy.Native();
             Map(x => x.UserID).Column("UserID");
             Map(x => x.RightsID).Column("RightsID");
             Map(x => x.CreateTime).Column("CreateTime");
             Map(x => x.CreateUser).Column("CreateUser");
             Map(x => x.DeleteFlag).Column("DeleteFlag");
             Map(x => x.DeleteUser).Column("DeleteUser");
             Map(x => x.DeleteTime).Column("DeleteTime");
        }
    }	    
}
