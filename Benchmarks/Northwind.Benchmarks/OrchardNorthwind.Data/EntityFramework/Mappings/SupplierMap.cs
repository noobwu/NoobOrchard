using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using OrchardNorthwind.Common.Entities;
namespace OrchardNorthwind.Data.EntityFramework.Mappings
{
    /// <summary>
    /// Suppliers
    /// </summary>
    [Serializable]
    public class SupplierMap: EntityTypeConfiguration<Supplier>
    {
         /// <summary>
        /// Suppliers
        /// </summary>
        public SupplierMap()
        {
           ToTable("Suppliers");
		   Ignore(t => t.Id);
		   
		    Property(t => t.SupplierId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
		   	HasKey(t => t.SupplierId);
		  
    	   Property(t => t.CompanyName).HasColumnName("CompanyName").IsRequired().HasColumnType("nvarchar").HasMaxLength(40);
    	   Property(t => t.ContactName).HasColumnName("ContactName").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
    	   Property(t => t.ContactTitle).HasColumnName("ContactTitle").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
    	   Property(t => t.Address).HasColumnName("Address").IsOptional().HasColumnType("nvarchar").HasMaxLength(60);
    	   Property(t => t.City).HasColumnName("City").IsOptional().HasColumnType("nvarchar").HasMaxLength(15);
    	   Property(t => t.Region).HasColumnName("Region").IsOptional().HasColumnType("nvarchar").HasMaxLength(15);
    	   Property(t => t.PostalCode).HasColumnName("PostalCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
    	   Property(t => t.Country).HasColumnName("Country").IsOptional().HasColumnType("nvarchar").HasMaxLength(15);
    	   Property(t => t.Phone).HasColumnName("Phone").IsOptional().HasColumnType("nvarchar").HasMaxLength(24);
    	   Property(t => t.Fax).HasColumnName("Fax").IsOptional().HasColumnType("nvarchar").HasMaxLength(24);
    	   Property(t => t.HomePage).HasColumnName("HomePage").IsOptional().HasColumnType("ntext");
		  
        }
    }	    
}
