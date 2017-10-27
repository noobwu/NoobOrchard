using System;
using System.Web;
using System.Data;
using Orchard.Domain.Entities;
namespace OrchardNorthwind.Common.Entities
{
    /// <summary>
    /// CustomerDemographics
    /// </summary>
    [Serializable]
    public class CustomerDemographic:Entity<string>
    {
        /// <summary>
        /// 
        /// </summary>
    	public virtual string CustomerTypeId {  get;set; }
        
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string CustomerDesc { get;set;}
        
          
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
		public override string  Id { get { return CustomerTypeId; } set { CustomerTypeId=value; }}
        /// <summary>
        /// 获取主键的属性名称
        /// </summary>
        /// <returns></returns>
        public override string GetPKPropertyName()
        {
            return "CustomerTypeId";
        }
	  
    }	
   
}
