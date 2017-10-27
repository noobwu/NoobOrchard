using Orchard.Data.NHibernate.EntityMappings;
using Orchard.Data.NHibernate.Tests.Entities;
using System;
namespace Orchard.Data.NHibernate.Tests.Mappings
{

    /// <summary>
    /// 系统权限
    /// </summary>
    [Serializable]
    public class AdmRightsMap : EntityMap<AdmRights, int>
    {
        /// <summary>
        /// 系统权限 Constructor.
        /// </summary>
        /// <param name="tableName">Table name</param>
        public AdmRightsMap() : base("wt_adm_rights")
        {
            Id(x => x.RightsID).GeneratedBy.Native();
            Map(x => x.RightsCode).Column("RightsCode").Length(50);
            Map(x => x.RightsName).Column("RightsName").Length(100);
            Map(x => x.RightsTypeID).Column("RightsTypeID");
            Map(x => x.Description).Column("Description").Length(500);
            Map(x => x.SortOrder).Column("SortOrder");
            Map(x => x.RightsType).Column("RightsType");
            Map(x => x.CreateTime).Column("CreateTime");
            Map(x => x.CreateUser).Column("CreateUser");
            Map(x => x.UpdateTime).Column("UpdateTime");
            Map(x => x.UpdateUser).Column("UpdateUser");
            Map(x => x.DeleteFlag).Column("DeleteFlag");
            Map(x => x.DeleteUser).Column("DeleteUser");
            Map(x => x.DeleteTime).Column("DeleteTime");
        }
    }
}
