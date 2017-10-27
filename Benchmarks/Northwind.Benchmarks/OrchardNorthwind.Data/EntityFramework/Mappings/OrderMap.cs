using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using OrchardNorthwind.Common.Entities;
namespace OrchardNorthwind.Data.EntityFramework.Mappings
{
    /// <summary>
    /// Orders
    /// </summary>
    [Serializable]
    public class OrderMap: EntityTypeConfiguration<Order>
    {
         /// <summary>
        /// Orders
        /// </summary>
        public OrderMap()
        {
           ToTable("Orders");
		   Ignore(t => t.Id);
		   
		    Property(t => t.OrderId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
		   	HasKey(t => t.OrderId);
		  
    	   Property(t => t.OrderDate).HasColumnName("OrderDate").IsOptional();
    	   Property(t => t.RequiredDate).HasColumnName("RequiredDate").IsOptional();
    	   Property(t => t.ShippedDate).HasColumnName("ShippedDate").IsOptional();
    	   Property(t => t.Freight).HasColumnName("Freight").IsOptional();
    	   Property(t => t.ShipName).HasColumnName("ShipName").IsOptional().HasColumnType("nvarchar").HasMaxLength(40);
    	   Property(t => t.ShipAddress).HasColumnName("ShipAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(60);
    	   Property(t => t.ShipCity).HasColumnName("ShipCity").IsOptional().HasColumnType("nvarchar").HasMaxLength(15);
    	   Property(t => t.ShipRegion).HasColumnName("ShipRegion").IsOptional().HasColumnType("nvarchar").HasMaxLength(15);
    	   Property(t => t.ShipPostalCode).HasColumnName("ShipPostalCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
    	   Property(t => t.ShipCountry).HasColumnName("ShipCountry").IsOptional().HasColumnType("nvarchar").HasMaxLength(15);
		  
        }
    }	    
}
