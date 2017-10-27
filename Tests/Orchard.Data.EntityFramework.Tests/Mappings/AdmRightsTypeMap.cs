using System;
using System.Web;
using System.Data;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using Orchard.Data.EntityFramework.Tests.Entities;
namespace Orchard.Data.EntityFramework.Tests.Mappings
{

    /// <summary>
    /// 系统权限分类表
    /// </summary>
    [Serializable]
    public class AdmRightsTypeMap : EntityTypeConfigurationBase<AdmRightsType>
    {
        /// <summary>
        /// 系统权限分类表
        /// </summary>
        public AdmRightsTypeMap()
        {
            ToTable("wt_adm_rights_type");
            Ignore(t => t.RightsTypeID);
            HasKey(t => t.Id);
            Property(t => t.Id).HasColumnName("RightsTypeID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.TypeName).HasColumnName("TypeName").IsRequired().HasColumnType("varchar").HasMaxLength(100);
            Property(t => t.ParentID).HasColumnName("ParentID").IsRequired();
            Property(t => t.SortOrder).HasColumnName("SortOrder").IsRequired();
            Property(t => t.IDPath).HasColumnName("IDPath").IsRequired().HasColumnType("varchar").HasMaxLength(200);
            Property(t => t.NamePath).HasColumnName("NamePath").IsRequired().HasColumnType("varchar").HasMaxLength(500);
            Property(t => t.CreateTime).HasColumnName("CreateTime").IsRequired();
            Property(t => t.CreateUser).HasColumnName("CreateUser").IsRequired();
            Property(t => t.UpdateTime).HasColumnName("UpdateTime").IsRequired();
            Property(t => t.UpdateUser).HasColumnName("UpdateUser").IsRequired();
            Property(t => t.DeleteFlag).HasColumnName("DeleteFlag").IsRequired();
            Property(t => t.DeleteUser).HasColumnName("DeleteUser").IsRequired();
            Property(t => t.DeleteTime).HasColumnName("DeleteTime").IsRequired();


        }
    }
}
