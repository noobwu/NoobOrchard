using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using OrchardNorthwind.Common.Entities;
namespace OrchardNorthwind.Data.EntityFramework.Mappings
{
    /// <summary>
    /// Categories
    /// </summary>
    [Serializable]
    public class CategoryMap : EntityTypeConfiguration<Category>
    {
        /// <summary>
        /// Categories
        /// </summary>
        public CategoryMap()
        {
            ToTable("Categories");
            Ignore(t => t.Id);

            Property(t => t.CategoryId).HasColumnName("CategoryID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            HasKey(t => t.CategoryId);

            Property(t => t.CategoryName).HasColumnName("CategoryName").IsRequired().HasColumnType("nvarchar").HasMaxLength(15);
            Property(t => t.Description).HasColumnName("Description").IsOptional().HasColumnType("ntext");
            Property(t => t.Picture).HasColumnName("Picture").IsOptional();

        }
    }
}
