using Orchard.Data.NHibernate.EntityMappings;
using Orchard.Data.NHibernate.Tests.Entities;
using System;
namespace Orchard.Data.NHibernate.Tests.Mappings
{

     /// <summary>
    /// 系统权限分类表
    /// </summary>
    [Serializable]
    public class AdmRightsTypeMap:EntityMap<AdmRightsType,int>
    {
        /// <summary>
        /// 系统权限分类表 Constructor.
        /// </summary>
        /// <param name="tableName">Table name</param>
        public AdmRightsTypeMap():base("wt_adm_rights_type")
        {
             Id(x => x.RightsTypeID).GeneratedBy.Native();
             Map(x => x.TypeName).Column("TypeName").Length(100);
             Map(x => x.ParentID).Column("ParentID");
             Map(x => x.SortOrder).Column("SortOrder");
             Map(x => x.IDPath).Column("IDPath").Length(200);
             Map(x => x.NamePath).Column("NamePath").Length(500);
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
