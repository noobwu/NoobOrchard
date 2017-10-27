using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using OrchardNorthwind.Common.Entities;
namespace OrchardNorthwind.Data.EntityFramework.Mappings
{
    /// <summary>
    /// Order Details
    /// </summary>
    [Serializable]
    public class OrderDetailMap: EntityTypeConfiguration<OrderDetail>
    {
         /// <summary>
        /// Order Details
        /// </summary>
        public OrderDetailMap()
        {
           ToTable("Order Details");
		   Ignore(t => t.Id);
		   
		   HasKey(t =>new {t.OrderId,t.ProductId});
		   Property(t => t.OrderId).HasColumnName("OrderID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None); 
		   Property(t => t.ProductId).HasColumnName("ProductID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None); 
		  
    	   Property(t => t.UnitPrice).HasColumnName("UnitPrice").IsRequired();
    	   Property(t => t.Quantity).HasColumnName("Quantity").IsRequired();
    	   Property(t => t.Discount).HasColumnName("Discount").IsRequired();
		  
        }
    }	    
}
