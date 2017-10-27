using System;
using System.Web;
using System.Data;
using Orchard.Domain.Entities;
namespace OrchardNorthwind.Common.Entities
{
    /// <summary>
    /// Order Details
    /// </summary>
    [Serializable]
    public class OrderDetail:Entity<string>
    {
        /// <summary>
        /// 
        /// </summary>
    	public virtual int OrderId {  get;set; }
        /// <summary>
        /// 
        /// </summary>
    	public virtual int ProductId {  get;set; }
        
    	/// <summary>
        /// 
        /// </summary>
    	public virtual decimal UnitPrice { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual short Quantity { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual double Discount { get;set;}
        
          
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
            return "OrderId_ProductId";
        }
	  
    }	
   
}
