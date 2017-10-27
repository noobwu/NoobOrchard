using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using OrchardNorthwind.Common.Entities;
namespace OrchardNorthwind.Data.EntityFramework.Mappings
{
    /// <summary>
    /// EmployeeTerritories
    /// </summary>
    [Serializable]
    public class EmployeeTerritoryMap: EntityTypeConfiguration<EmployeeTerritory>
    {
         /// <summary>
        /// EmployeeTerritories
        /// </summary>
        public EmployeeTerritoryMap()
        {
           ToTable("EmployeeTerritories");
		   Ignore(t => t.Id);
		   
		   HasKey(t =>new {t.EmployeeId,t.TerritoryId});
		   Property(t => t.EmployeeId).HasColumnName("EmployeeID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None); 
		   Property(t => t.TerritoryId).HasColumnName("TerritoryID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None); 
		  
		  
        }
    }	    
}
