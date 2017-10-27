using System;
using System.Web;
using System.Data;
using Orchard.Domain.Entities;
namespace OrchardNorthwind.Common.Entities
{
    /// <summary>
    /// Categories
    /// </summary>
    [Serializable]
    public class Category:Entity<int>
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual int CategoryId {  get;set; }
        
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string CategoryName { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string Description { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual byte[] Picture { get;set;}
        
          
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
		public override int  Id { get { return CategoryId; } set { CategoryId=value; }}
        /// <summary>
        /// 获取主键的属性名称
        /// </summary>
        /// <returns></returns>
        public override string GetPKPropertyName()
        {
            return "CategoryId";
        }
	  
    }	
   
}
