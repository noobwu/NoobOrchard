using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using OrchardNorthwind.Common.Entities;
namespace OrchardNorthwind.Data.EntityFramework.Mappings
{
    /// <summary>
    /// Shippers
    /// </summary>
    [Serializable]
    public class ShipperMap: EntityTypeConfiguration<Shipper>
    {
         /// <summary>
        /// Shippers
        /// </summary>
        public ShipperMap()
        {
           ToTable("Shippers");
		   Ignore(t => t.Id);
		   
		    Property(t => t.ShipperId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
		   	HasKey(t => t.ShipperId);
		  
    	   Property(t => t.CompanyName).HasColumnName("CompanyName").IsRequired().HasColumnType("nvarchar").HasMaxLength(40);
    	   Property(t => t.Phone).HasColumnName("Phone").IsOptional().HasColumnType("nvarchar").HasMaxLength(24);
		  
        }
    }	    
}
