using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using OrchardNorthwind.Common.Entities;
namespace OrchardNorthwind.Data.EntityFramework.Mappings
{
    /// <summary>
    /// Region
    /// </summary>
    [Serializable]
    public class RegionMap: EntityTypeConfiguration<Region>
    {
         /// <summary>
        /// Region
        /// </summary>
        public RegionMap()
        {
           ToTable("Region");
		   Ignore(t => t.Id);
		   
		   HasKey(t => t.RegionId);
		   Property(t => t.RegionId).HasColumnName("RegionID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None); 
		
		  
    	   Property(t => t.RegionDescription).HasColumnName("RegionDescription").IsRequired().HasColumnType("nchar").HasMaxLength(50);
		  
        }
    }	    
}
