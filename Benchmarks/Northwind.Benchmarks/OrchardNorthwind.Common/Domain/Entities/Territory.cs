using System;
using System.Web;
using System.Data;
using Orchard.Domain.Entities;
namespace OrchardNorthwind.Common.Entities
{
    /// <summary>
    /// Territories
    /// </summary>
    [Serializable]
    public class Territory:Entity<string>
    {
        /// <summary>
        /// 
        /// </summary>
    	public virtual string TerritoryId {  get;set; }
        
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string TerritoryDescription { get;set;}
        
        /// <summary>
        /// 
        /// </summary>
    	public virtual   int RegionId { get;set;}
          
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
		public override string  Id { get { return TerritoryId; } set { TerritoryId=value; }}
        /// <summary>
        /// 获取主键的属性名称
        /// </summary>
        /// <returns></returns>
        public override string GetPKPropertyName()
        {
            return "TerritoryId";
        }
	  
    }	
   
}
