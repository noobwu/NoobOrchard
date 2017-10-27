using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using OrchardNorthwind.Common.Entities;
namespace OrchardNorthwind.Data.EntityFramework.Mappings
{
    /// <summary>
    /// CustomerDemographics
    /// </summary>
    [Serializable]
    public class CustomerDemographicMap: EntityTypeConfiguration<CustomerDemographic>
    {
         /// <summary>
        /// CustomerDemographics
        /// </summary>
        public CustomerDemographicMap()
        {
           ToTable("CustomerDemographics");
		   Ignore(t => t.Id);
		   
		   HasKey(t => t.CustomerTypeId);
		   Property(t => t.CustomerTypeId).HasColumnName("CustomerTypeID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None); 
		
		  
    	   Property(t => t.CustomerDesc).HasColumnName("CustomerDesc").IsOptional().HasColumnType("ntext");
		  
        }
    }	    
}
