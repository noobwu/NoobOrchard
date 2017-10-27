using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using OrchardNorthwind.Common.Entities;
namespace OrchardNorthwind.Data.EntityFramework.Mappings
{
    /// <summary>
    /// Employees
    /// </summary>
    [Serializable]
    public class EmployeeMap : EntityTypeConfiguration<Employee>
    {
        /// <summary>
        /// Employees
        /// </summary>
        public EmployeeMap()
        {
            ToTable("Employees");
            Ignore(t => t.Id);

            Property(t => t.EmployeeId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            HasKey(t => t.EmployeeId);

            Property(t => t.LastName).HasColumnName("LastName").IsRequired().HasColumnType("nvarchar").HasMaxLength(20);
            Property(t => t.FirstName).HasColumnName("FirstName").IsRequired().HasColumnType("nvarchar").HasMaxLength(10);
            Property(t => t.Title).HasColumnName("Title").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(t => t.TitleOfCourtesy).HasColumnName("TitleOfCourtesy").IsOptional().HasColumnType("nvarchar").HasMaxLength(25);
            Property(t => t.BirthDate).HasColumnName("BirthDate").IsOptional();
            Property(t => t.HireDate).HasColumnName("HireDate").IsOptional();
            Property(t => t.Address).HasColumnName("Address").IsOptional().HasColumnType("nvarchar").HasMaxLength(60);
            Property(t => t.City).HasColumnName("City").IsOptional().HasColumnType("nvarchar").HasMaxLength(15);
            Property(t => t.Region).HasColumnName("Region").IsOptional().HasColumnType("nvarchar").HasMaxLength(15);
            Property(t => t.PostalCode).HasColumnName("PostalCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
            Property(t => t.Country).HasColumnName("Country").IsOptional().HasColumnType("nvarchar").HasMaxLength(15);
            Property(t => t.HomePhone).HasColumnName("HomePhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(24);
            Property(t => t.Extension).HasColumnName("Extension").IsOptional().HasColumnType("nvarchar").HasMaxLength(4);
            Property(t => t.Photo).HasColumnName("Photo").IsOptional();
            Property(t => t.Notes).HasColumnName("Notes").IsOptional().HasColumnType("ntext");
            Property(t => t.PhotoPath).HasColumnName("PhotoPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);

        }
    }
}
