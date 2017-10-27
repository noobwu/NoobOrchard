using System;
using System.ComponentModel.DataAnnotations.Schema;
using Orchard.Data.EntityFramework.Tests.Entities;
namespace Orchard.Data.EntityFramework.Tests.Mappings
{
    /// <summary>
    /// 地区
    /// </summary>
    [Serializable]
    public class AdmAreaMap : EntityTypeConfigurationBase<AdmArea>
    {
        /// <summary>
        /// 地区
        /// </summary>
        public AdmAreaMap()
        {
            ToTable("wt_adm_area");
            HasKey(t => t.Id);
            Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.AreaID).HasColumnName("AreaID").IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.AreaName).HasColumnName("AreaName").IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.ParentId).HasColumnName("ParentId").IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.ShortName).HasColumnName("ShortName").IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.LevelType).HasColumnName("LevelType").IsRequired();
            Property(t => t.CityCode).HasColumnName("CityCode").IsOptional().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.ZipCode).HasColumnName("ZipCode").IsOptional().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.AreaNamePath).HasColumnName("AreaNamePath").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(t => t.AreaIDPath).HasColumnName("AreaIDPath").IsRequired().HasColumnType("varchar").HasMaxLength(500);
            Property(t => t.Lng).HasColumnName("lng").IsRequired();
            Property(t => t.Lat).HasColumnName("Lat").IsRequired();
            Property(t => t.PinYin).HasColumnName("PinYin").IsOptional().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.ShortPinYin).HasColumnName("ShortPinYin").IsOptional().HasColumnType("varchar").HasMaxLength(20);
            Property(t => t.PYFirstLetter).HasColumnName("PYFirstLetter").IsOptional().HasColumnType("varchar").HasMaxLength(10);
            Property(t => t.SortOrder).HasColumnName("SortOrder").IsRequired();
            Property(t => t.Status).HasColumnName("Status").IsRequired();
            Property(t => t.CreateTime).HasColumnName("CreateTime").IsRequired();
            Property(t => t.CreateUser).HasColumnName("CreateUser").IsRequired();
            Property(t => t.UpdateTime).HasColumnName("UpdateTime").IsRequired();
            Property(t => t.UpdateUser).HasColumnName("UpdateUser").IsRequired();
        }
    }
}
