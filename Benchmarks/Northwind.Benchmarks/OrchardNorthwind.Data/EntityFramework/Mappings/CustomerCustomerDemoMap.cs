using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using OrchardNorthwind.Common.Entities;
namespace OrchardNorthwind.Data.EntityFramework.Mappings
{
    /// <summary>
    /// CustomerCustomerDemo
    /// </summary>
    [Serializable]
    public class CustomerCustomerDemoMap: EntityTypeConfiguration<CustomerCustomerDemo>
    {
         /// <summary>
        /// CustomerCustomerDemo
        /// </summary>
        public CustomerCustomerDemoMap()
        {
           ToTable("CustomerCustomerDemo");
		   Ignore(t => t.Id);
		   
		   HasKey(t =>new {t.CustomerId,t.CustomerTypeId});
		   Property(t => t.CustomerId).HasColumnName("CustomerID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None); 
		   Property(t => t.CustomerTypeId).HasColumnName("CustomerTypeID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None); 
		  
		  
        }
    }	    
}
