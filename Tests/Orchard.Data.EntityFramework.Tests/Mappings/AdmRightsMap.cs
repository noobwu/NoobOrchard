using System;
using System.Web;
using System.Data;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using Orchard.Data.EntityFramework.Tests.Entities;
namespace Orchard.Data.EntityFramework.Tests.Mappings
{

    /// <summary>
    /// 系统权限
    /// </summary>
    [Serializable]
    public class AdmRightsMap : EntityTypeConfigurationBase<AdmRightsExt>
    {
        /// <summary>
        /// 系统权限
        /// </summary>
        public AdmRightsMap()
        {
            ToTable("wt_adm_rights");
            Ignore(t => t.RightsID);
            HasKey(t => t.Id);
            Property(t => t.Id).HasColumnName("RightsID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.RightsCode).HasColumnName("RightsCode").IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.RightsName).HasColumnName("RightsName").IsRequired().HasColumnType("varchar").HasMaxLength(100);
            Property(t => t.RightsTypeID).HasColumnName("RightsTypeID").IsRequired();
            Property(t => t.Description).HasColumnName("Description").IsRequired().HasColumnType("varchar").HasMaxLength(500);
            Property(t => t.SortOrder).HasColumnName("SortOrder").IsRequired();
            Property(t => t.RightsType).HasColumnName("RightsType").IsRequired();
            Property(t => t.CreateTime).HasColumnName("CreateTime").IsRequired();
            Property(t => t.CreateUser).HasColumnName("CreateUser").IsRequired();
            Property(t => t.UpdateTime).HasColumnName("UpdateTime").IsRequired();
            Property(t => t.UpdateUser).HasColumnName("UpdateUser").IsRequired();
            Property(t => t.DeleteFlag).HasColumnName("DeleteFlag").IsRequired();
            Property(t => t.DeleteUser).HasColumnName("DeleteUser").IsRequired();
            Property(t => t.DeleteTime).HasColumnName("DeleteTime").IsRequired();

            //针对“一对一”关系的设置
            HasRequired(b => b.AdmRightsType).WithMany().HasForeignKey(b => b.RightsTypeID);

        }
    }
}
