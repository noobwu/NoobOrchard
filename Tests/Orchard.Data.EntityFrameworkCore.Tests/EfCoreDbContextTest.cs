using Microsoft.EntityFrameworkCore;
using Orchard.Data.EntityFrameworkCore.Tests.Entities;
using Orchard.Tests.Common.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchard.Data.EntityFrameworkCore.Tests
{
    public class EfCoreDbContextTest : EfCoreDbContext
    {
        public EfCoreDbContextTest(DbContextOptions options) : base(options)
        {
        }
        //
        // 摘要:
        //     Override this method to further configure the model that was discovered by convention
        //     from the entity types exposed in Microsoft.EntityFrameworkCore.DbSet`1 properties
        //     on your derived context. The resulting model may be cached and re-used for subsequent
        //     instances of your derived context.
        //
        // 参数:
        //   modelBuilder:
        //     The builder being used to construct the model for this context. Databases (and
        //     other extensions) typically define extension methods on this object that allow
        //     you to configure aspects of the model that are specific to a given database.
        //
        // 备注:
        //     If a model is explicitly set on the options for this context (via Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel))
        //     then this method will not be run.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var admAreaEntity = modelBuilder.Entity<AdmArea>();
            admAreaEntity.ToTable("wt_adm_area");
            admAreaEntity.HasKey(t => t.Id);
            admAreaEntity.Property(t => t.Id);
            //admAreaEntity.Property(t => t.AreaID).HasColumnName("AreaID").IsRequired().HasMaxLength(50);
            //admAreaEntity.Property(t => t.AreaName).HasColumnName("AreaName").IsRequired().HasMaxLength(50);
            //admAreaEntity.Property(t => t.ParentId).HasColumnName("ParentId").IsRequired().HasMaxLength(50);
            //admAreaEntity.Property(t => t.ShortName).HasColumnName("ShortName").IsRequired().HasMaxLength(50);
            //admAreaEntity.Property(t => t.LevelType).HasColumnName("LevelType").IsRequired();
            //admAreaEntity.Property(t => t.CityCode).HasColumnName("CityCode").HasMaxLength(50);
            //admAreaEntity.Property(t => t.ZipCode).HasColumnName("ZipCode").HasMaxLength(50);
            //admAreaEntity.Property(t => t.AreaNamePath).HasColumnName("AreaNamePath").IsRequired().ForSqlServerHasColumnType("nvarchar").HasMaxLength(500);
            //admAreaEntity.Property(t => t.AreaIDPath).HasColumnName("AreaIDPath").IsRequired().HasMaxLength(500);
            //admAreaEntity.Property(t => t.Lng).HasColumnName("lng").IsRequired();
            //admAreaEntity.Property(t => t.Lat).HasColumnName("Lat").IsRequired();
            //admAreaEntity.Property(t => t.PinYin).HasColumnName("PinYin").HasMaxLength(50);
            //admAreaEntity.Property(t => t.ShortPinYin).HasColumnName("ShortPinYin").HasMaxLength(20);
            //admAreaEntity.Property(t => t.PYFirstLetter).HasColumnName("PYFirstLetter").HasMaxLength(10);
            //admAreaEntity.Property(t => t.SortOrder).HasColumnName("SortOrder").IsRequired();
            //admAreaEntity.Property(t => t.Status).HasColumnName("Status").IsRequired();
            //admAreaEntity.Property(t => t.CreateTime).HasColumnName("CreateTime").IsRequired();
            //admAreaEntity.Property(t => t.CreateUser).HasColumnName("CreateUser").IsRequired();
            //admAreaEntity.Property(t => t.UpdateTime).HasColumnName("UpdateTime").IsRequired();
            //admAreaEntity.Property(t => t.UpdateUser).HasColumnName("UpdateUser").IsRequired();
            //admAreaEntity.Property(t => t.DeleteFlag).HasColumnName("DeleteFlag").IsRequired();
            //admAreaEntity.Property(t => t.DeleteTime).HasColumnName("DeleteTime").IsRequired();
            //admAreaEntity.Property(t => t.DeleteUser).HasColumnName("DeleteUser").IsRequired();




            var admRightsEntity = modelBuilder.Entity<AdmRightsExt>();
            admRightsEntity.ToTable("wt_adm_rights");
            admRightsEntity.Ignore(t => t.RightsID);
            admRightsEntity.HasKey(t => t.Id);
            admRightsEntity.Property(t => t.Id).HasColumnName("RightsID");
            //admRightsEntity.Property(t => t.RightsCode).HasColumnName("RightsCode").IsRequired();
            //admRightsEntity.Property(t => t.RightsName).HasColumnName("RightsName").IsRequired();
            //admRightsEntity.Property(t => t.RightsTypeID).HasColumnName("RightsTypeID").IsRequired();
            //admRightsEntity.Property(t => t.Description).HasColumnName("Description").IsRequired();
            //admRightsEntity.Property(t => t.SortOrder).HasColumnName("SortOrder").IsRequired();
            //admRightsEntity.Property(t => t.RightsType).HasColumnName("RightsType").IsRequired();
            //admRightsEntity.Property(t => t.CreateTime).HasColumnName("CreateTime").IsRequired();
            //admRightsEntity.Property(t => t.CreateUser).HasColumnName("CreateUser").IsRequired();
            //admRightsEntity.Property(t => t.UpdateTime).HasColumnName("UpdateTime").IsRequired();
            //admRightsEntity.Property(t => t.UpdateUser).HasColumnName("UpdateUser").IsRequired();
            //admRightsEntity.Property(t => t.DeleteFlag).HasColumnName("DeleteFlag").IsRequired();
            //admRightsEntity.Property(t => t.DeleteUser).HasColumnName("DeleteUser").IsRequired();
            //admRightsEntity.Property(t => t.DeleteTime).HasColumnName("DeleteTime").IsRequired();
            admRightsEntity.HasOne(b => b.AdmRightsType).WithMany().HasForeignKey(b => b.RightsTypeID);

            var admRightsTypeEntity = modelBuilder.Entity<AdmRightsType>();
            admRightsTypeEntity.ToTable("wt_adm_rights_type");
            admRightsTypeEntity.Ignore(t => t.RightsTypeID);
            admRightsTypeEntity.HasKey(t => t.Id);
            admRightsTypeEntity.Property(t => t.Id).HasColumnName("RightsTypeID");
            //admRightsTypeEntity.Property(t => t.TypeName).HasColumnName("TypeName").IsRequired();
            //admRightsTypeEntity.Property(t => t.ParentID).HasColumnName("ParentID").IsRequired();
            //admRightsTypeEntity.Property(t => t.SortOrder).HasColumnName("SortOrder").IsRequired();
            //admRightsTypeEntity.Property(t => t.IDPath).HasColumnName("IDPath").IsRequired();
            //admRightsTypeEntity.Property(t => t.NamePath).HasColumnName("NamePath").IsRequired();
            //admRightsTypeEntity.Property(t => t.CreateTime).HasColumnName("CreateTime").IsRequired();
            //admRightsTypeEntity.Property(t => t.CreateUser).HasColumnName("CreateUser").IsRequired();
            //admRightsTypeEntity.Property(t => t.UpdateTime).HasColumnName("UpdateTime").IsRequired();
            //admRightsTypeEntity.Property(t => t.UpdateUser).HasColumnName("UpdateUser").IsRequired();
            //admRightsTypeEntity.Property(t => t.DeleteFlag).HasColumnName("DeleteFlag").IsRequired();
            //admRightsTypeEntity.Property(t => t.DeleteUser).HasColumnName("DeleteUser").IsRequired();
            //admRightsTypeEntity.Property(t => t.DeleteTime).HasColumnName("DeleteTime").IsRequired();


            var admUserRightsEntity = modelBuilder.Entity<AdmUserRightsExt>();
            admUserRightsEntity.ToTable("wt_adm_user_rights");
            admUserRightsEntity.Ignore(t => t.UserRightID);
            admUserRightsEntity.HasKey(t => t.Id);
            admUserRightsEntity.Property(t => t.Id).HasColumnName("UserRightID");
            //admUserRightsEntity.Property(t => t.UserID).HasColumnName("UserID").IsRequired();
            //admUserRightsEntity.Property(t => t.RightsID).HasColumnName("RightsID").IsRequired();
            //admUserRightsEntity.Property(t => t.CreateTime).HasColumnName("CreateTime").IsRequired();
            //admUserRightsEntity.Property(t => t.CreateUser).HasColumnName("CreateUser").IsRequired();
            //admUserRightsEntity.Property(t => t.DeleteFlag).HasColumnName("DeleteFlag").IsRequired();
            //admUserRightsEntity.Property(t => t.DeleteUser).HasColumnName("DeleteUser").IsRequired();
            //admUserRightsEntity.Property(t => t.DeleteTime).HasColumnName("DeleteTime").IsRequired();
            admUserRightsEntity.HasOne(b => b.AdmRights).WithMany().HasForeignKey(b => b.RightsID);



            var admAreaTestEntity = modelBuilder.Entity<AdmAreaTest>();
            admAreaTestEntity.ToTable("wt_adm_area_test");
            admAreaTestEntity.HasKey(t => t.Id);
            admAreaTestEntity.Property(t => t.Id).HasColumnName("ID"); ;
            admAreaTestEntity.Property(t => t.AreaId).HasColumnName("AreaID").IsRequired().HasMaxLength(50);
            admAreaTestEntity.Property(t => t.AreaName).HasColumnName("AreaName").IsRequired().HasMaxLength(50);
            admAreaTestEntity.Property(t => t.ParentId).HasColumnName("ParentId").IsRequired().HasMaxLength(50);
            admAreaTestEntity.Property(t => t.ShortName).HasColumnName("ShortName").IsRequired().HasMaxLength(50);
            admAreaTestEntity.Property(t => t.LevelType).HasColumnName("LevelType").IsRequired();
            admAreaTestEntity.Property(t => t.CityCode).HasColumnName("CityCode").HasMaxLength(50);
            admAreaTestEntity.Property(t => t.ZipCode).HasColumnName("ZipCode").HasMaxLength(50);
            admAreaTestEntity.Property(t => t.AreaNamePath).HasColumnName("AreaNamePath").IsRequired().HasMaxLength(500);
            admAreaTestEntity.Property(t => t.AreaIdPath).HasColumnName("AreaIDPath").IsRequired().HasMaxLength(500);
            admAreaTestEntity.Property(t => t.Lng).HasColumnName("lng").IsRequired();
            admAreaTestEntity.Property(t => t.Lat).HasColumnName("Lat").IsRequired();
            admAreaTestEntity.Property(t => t.PinYin).HasColumnName("PinYin").HasMaxLength(50);
            admAreaTestEntity.Property(t => t.ShortPinYin).HasColumnName("ShortPinYin").HasMaxLength(20);
            admAreaTestEntity.Property(t => t.PYFirstLetter).HasColumnName("PYFirstLetter").HasMaxLength(10);
            admAreaTestEntity.Property(t => t.SortOrder).HasColumnName("SortOrder").IsRequired();
            admAreaTestEntity.Property(t => t.Status).HasColumnName("Status").IsRequired();
            admAreaTestEntity.Property(t => t.CreateTime).HasColumnName("CreateTime").IsRequired();
            admAreaTestEntity.Property(t => t.CreateUser).HasColumnName("CreateUser").IsRequired();
            admAreaTestEntity.Property(t => t.UpdateTime).HasColumnName("UpdateTime").IsRequired();
            admAreaTestEntity.Property(t => t.UpdateUser).HasColumnName("UpdateUser").IsRequired();
            admAreaTestEntity.Property(t => t.DeleteFlag).HasColumnName("DeleteFlag").IsRequired();
            admAreaTestEntity.Property(t => t.DeleteTime).HasColumnName("DeleteTime").IsRequired();
            admAreaTestEntity.Property(t => t.DeleteUser).HasColumnName("DeleteUser").IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
