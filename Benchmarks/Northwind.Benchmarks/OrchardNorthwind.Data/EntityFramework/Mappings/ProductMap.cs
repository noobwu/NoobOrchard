using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using OrchardNorthwind.Common.Entities;
namespace OrchardNorthwind.Data.EntityFramework.Mappings
{
    /// <summary>
    /// Products
    /// </summary>
    [Serializable]
    public class ProductMap: EntityTypeConfiguration<Product>
    {
         /// <summary>
        /// Products
        /// </summary>
        public ProductMap()
        {
           ToTable("Products");
		   Ignore(t => t.Id);
		   
		    Property(t => t.ProductId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
		   	HasKey(t => t.ProductId);
		  
    	   Property(t => t.ProductName).HasColumnName("ProductName").IsRequired().HasColumnType("nvarchar").HasMaxLength(40);
    	   Property(t => t.QuantityPerUnit).HasColumnName("QuantityPerUnit").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
    	   Property(t => t.UnitPrice).HasColumnName("UnitPrice").IsOptional();
    	   Property(t => t.UnitsInStock).HasColumnName("UnitsInStock").IsOptional();
    	   Property(t => t.UnitsOnOrder).HasColumnName("UnitsOnOrder").IsOptional();
    	   Property(t => t.ReorderLevel).HasColumnName("ReorderLevel").IsOptional();
    	   Property(t => t.Discontinued).HasColumnName("Discontinued").IsRequired();
		  
        }
    }	    
}
