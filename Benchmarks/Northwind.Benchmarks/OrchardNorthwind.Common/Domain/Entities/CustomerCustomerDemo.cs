using System;
using System.Web;
using System.Data;
using Orchard.Domain.Entities;
namespace OrchardNorthwind.Common.Entities
{
    /// <summary>
    /// CustomerCustomerDemo
    /// </summary>
    [Serializable]
    public class CustomerCustomerDemo:Entity<string>
    {
        /// <summary>
        /// 
        /// </summary>
    	public virtual string CustomerId {  get;set; }
        /// <summary>
        /// 
        /// </summary>
    	public virtual string CustomerTypeId {  get;set; }
        
        
          
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
		public override string  Id { get; set;}
        /// <summary>
        /// 获取主键的属性名称
        /// </summary>
        /// <returns></returns>
        public override string GetPKPropertyName()
        {
            return "CustomerId_CustomerTypeId";
        }
	  
    }	
   
}
