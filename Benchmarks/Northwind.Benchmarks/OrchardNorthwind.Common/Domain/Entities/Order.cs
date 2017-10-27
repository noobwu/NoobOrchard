using System;
using System.Web;
using System.Data;
using Orchard.Domain.Entities;
namespace OrchardNorthwind.Common.Entities
{
    /// <summary>
    /// Orders
    /// </summary>
    [Serializable]
    public class Order:Entity<int>
    {
        /// <summary>
        /// 
        /// </summary>
    	public virtual int OrderId {  get;set; }
        
    	/// <summary>
        /// 
        /// </summary>
    	public virtual DateTime? OrderDate { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual DateTime? RequiredDate { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual DateTime? ShippedDate { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual decimal? Freight { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string ShipName { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string ShipAddress { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string ShipCity { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string ShipRegion { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string ShipPostalCode { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string ShipCountry { get;set;}
        
        /// <summary>
        /// 
        /// </summary>
    	public virtual   string CustomerId { get;set;}
        /// <summary>
        /// 
        /// </summary>
    	public virtual   int? EmployeeId { get;set;}
        /// <summary>
        /// 
        /// </summary>
    	public virtual   int? ShipVia { get;set;}
          
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
		public override int  Id { get { return OrderId; } set { OrderId=value; }}
        /// <summary>
        /// 获取主键的属性名称
        /// </summary>
        /// <returns></returns>
        public override string GetPKPropertyName()
        {
            return "OrderId";
        }
	  
    }	
   
}
