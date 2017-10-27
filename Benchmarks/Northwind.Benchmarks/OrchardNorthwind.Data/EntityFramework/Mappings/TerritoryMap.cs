using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using OrchardNorthwind.Common.Entities;
namespace OrchardNorthwind.Data.EntityFramework.Mappings
{
    /// <summary>
    /// Territories
    /// </summary>
    [Serializable]
    public class TerritoryMap: EntityTypeConfiguration<Territory>
    {
         /// <summary>
        /// Territories
        /// </summary>
        public TerritoryMap()
        {
           ToTable("Territories");
		   Ignore(t => t.Id);
		   
		   HasKey(t => t.TerritoryId);
		   Property(t => t.TerritoryId).HasColumnName("TerritoryID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None); 
		
		  
    	   Property(t => t.TerritoryDescription).HasColumnName("TerritoryDescription").IsRequired().HasColumnType("nchar").HasMaxLength(50);
		  
        }
    }	    
}
