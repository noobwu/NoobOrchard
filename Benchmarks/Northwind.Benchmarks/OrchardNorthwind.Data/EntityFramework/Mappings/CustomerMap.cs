using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using OrchardNorthwind.Common.Entities;
namespace OrchardNorthwind.Data.EntityFramework.Mappings
{
    /// <summary>
    /// Customers
    /// </summary>
    [Serializable]
    public class CustomerMap: EntityTypeConfiguration<Customer>
    {
         /// <summary>
        /// Customers
        /// </summary>
        public CustomerMap()
        {
           ToTable("Customers");
		   Ignore(t => t.Id);
		   
		   HasKey(t => t.CustomerId);
		   Property(t => t.CustomerId).HasColumnName("CustomerID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None); 
		
		  
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
    	   Property(t => t.Picture).HasColumnName("Picture").IsOptional();
		  
        }
    }	    
}
