using System;
using System.Web;
using System.Data;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using Orchard.Data.EntityFramework.Tests.Entities;
namespace Orchard.Data.EntityFramework.Tests.Mappings
{

    /// <summary>
    /// 系统用户权限
    /// </summary>
    [Serializable]
    public class AdmUserRightsMap : EntityTypeConfigurationBase<AdmUserRightsExt>
    {
        /// <summary>
        /// 系统用户权限
        /// </summary>
        public AdmUserRightsMap()
        {
            ToTable("wt_adm_user_rights");
            HasKey(t => t.Id);
            Ignore(t => t.UserRightID);
            Property(t => t.Id).HasColumnName("UserRightID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.UserID).HasColumnName("UserID").IsRequired();
            Property(t => t.RightsID).HasColumnName("RightsID").IsRequired();
            Property(t => t.CreateTime).HasColumnName("CreateTime").IsRequired();
            Property(t => t.CreateUser).HasColumnName("CreateUser").IsRequired();
            Property(t => t.DeleteFlag).HasColumnName("DeleteFlag").IsRequired();
            Property(t => t.DeleteUser).HasColumnName("DeleteUser").IsRequired();
            Property(t => t.DeleteTime).HasColumnName("DeleteTime").IsRequired();
            //针对“一对一”关系的设置
            HasRequired(b => b.AdmRights).WithMany().HasForeignKey(b => b.RightsID);
            //HasRequired(t => t.AdmRightsType).WithRequiredDependent();

        }
    }
}
