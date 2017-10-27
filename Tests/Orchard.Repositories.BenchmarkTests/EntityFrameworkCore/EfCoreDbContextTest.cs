using Microsoft.EntityFrameworkCore;
using Orchard.Data.EntityFrameworkCore;
using Orchard.Tests.Common.Domain.Entities;

namespace Orchard.Repositories.BenchmarkTests.EntityFrameworkCore
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
