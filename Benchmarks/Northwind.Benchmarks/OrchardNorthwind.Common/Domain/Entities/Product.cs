using System;
using System.Web;
using System.Data;
using Orchard.Domain.Entities;
namespace OrchardNorthwind.Common.Entities
{
    /// <summary>
    /// Products
    /// </summary>
    [Serializable]
    public class Product:Entity<int>
    {
        /// <summary>
        /// 
        /// </summary>
    	public virtual int ProductId {  get;set; }
        
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string ProductName { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string QuantityPerUnit { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual decimal? UnitPrice { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual short? UnitsInStock { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual short? UnitsOnOrder { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual short? ReorderLevel { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual bool Discontinued { get;set;}
        
        /// <summary>
        /// 
        /// </summary>
    	public virtual   int? CategoryId { get;set;}
        /// <summary>
        /// 
        /// </summary>
    	public virtual   int? SupplierId { get;set;}
          
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
		public override int  Id { get { return ProductId; } set { ProductId=value; }}
        /// <summary>
        /// 获取主键的属性名称
        /// </summary>
        /// <returns></returns>
        public override string GetPKPropertyName()
        {
            return "ProductId";
        }
	  
    }	
   
}
