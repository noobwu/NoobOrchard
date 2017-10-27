using System;
using System.Web;
using System.Data;
using Orchard.Domain.Entities;
namespace OrchardNorthwind.Common.Entities
{
    /// <summary>
    /// Region
    /// </summary>
    [Serializable]
    public class Region:Entity<int>
    {
        /// <summary>
        /// 
        /// </summary>
    	public virtual int RegionId {  get;set; }
        
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string RegionDescription { get;set;}
        
          
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
		public override int  Id { get { return RegionId; } set { RegionId=value; }}
        /// <summary>
        /// 获取主键的属性名称
        /// </summary>
        /// <returns></returns>
        public override string GetPKPropertyName()
        {
            return "RegionId";
        }
	  
    }	
   
}
