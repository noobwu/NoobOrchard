using System;
using System.Web;
using System.Data;
using Orchard.Domain.Entities;
namespace OrchardNorthwind.Common.Entities
{
    /// <summary>
    /// Shippers
    /// </summary>
    [Serializable]
    public class Shipper:Entity<int>
    {
        /// <summary>
        /// 
        /// </summary>
    	public virtual int ShipperId {  get;set; }
        
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string CompanyName { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string Phone { get;set;}
        
          
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
		public override int  Id { get { return ShipperId; } set { ShipperId=value; }}
        /// <summary>
        /// 获取主键的属性名称
        /// </summary>
        /// <returns></returns>
        public override string GetPKPropertyName()
        {
            return "ShipperId";
        }
	  
    }	
   
}
