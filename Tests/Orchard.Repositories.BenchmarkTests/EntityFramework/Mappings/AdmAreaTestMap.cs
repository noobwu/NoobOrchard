using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using Orchard.Data.EntityFramework;
using Orchard.Tests.Common.Domain.Entities;

namespace Orchard.Repositories.BenchmarkTests.EntityFramework.Mappings
{
    /// <summary>
    /// wt_adm_area_test
    /// </summary>
    [Serializable]
    public class AdmAreaTestMap : EntityTypeConfigurationBase<AdmAreaTest>
    {
        /// <summary>
        /// wt_adm_area_test
        /// </summary>
        public AdmAreaTestMap()
        {
            ToTable("wt_adm_area_test");

            HasKey(t => t.Id);
            Property(t => t.Id).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.AreaId).HasColumnName("AreaID").IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.AreaName).HasColumnName("AreaName").IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.ParentId).HasColumnName("ParentId").IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.ShortName).HasColumnName("ShortName").IsRequired().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.LevelType).HasColumnName("LevelType").IsRequired();
            Property(t => t.CityCode).HasColumnName("CityCode").IsOptional().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.ZipCode).HasColumnName("ZipCode").IsOptional().HasColumnType("varchar").HasMaxLength(50);
            Property(t => t.AreaNamePath).HasColumnName("AreaNamePath").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(t => t.AreaIdPath).HasColumnName("AreaIDPath").IsRequired().HasColumnType("varchar").HasMaxLength(500);
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
            Property(t => t.DeleteFlag).HasColumnName("DeleteFlag").IsRequired();
            Property(t => t.DeleteTime).HasColumnName("DeleteTime").IsRequired();
            Property(t => t.DeleteUser).HasColumnName("DeleteUser").IsRequired();

        }
    }
}
