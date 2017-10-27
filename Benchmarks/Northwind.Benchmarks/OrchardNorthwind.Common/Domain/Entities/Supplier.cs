using System;
using System.Web;
using System.Data;
using Orchard.Domain.Entities;
namespace OrchardNorthwind.Common.Entities
{
    /// <summary>
    /// Suppliers
    /// </summary>
    [Serializable]
    public class Supplier:Entity<int>
    {
        /// <summary>
        /// 
        /// </summary>
    	public virtual int SupplierId {  get;set; }
        
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string CompanyName { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string ContactName { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string ContactTitle { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string Address { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string City { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string Region { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string PostalCode { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string Country { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string Phone { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string Fax { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string HomePage { get;set;}
        
          
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
		public override int  Id { get { return SupplierId; } set { SupplierId=value; }}
        /// <summary>
        /// 获取主键的属性名称
        /// </summary>
        /// <returns></returns>
        public override string GetPKPropertyName()
        {
            return "SupplierId";
        }
	  
    }	
   
}
