using System;
using System.Web;
using System.Data;
using Orchard.Domain.Entities;
namespace OrchardNorthwind.Common.Entities
{
    /// <summary>
    /// EmployeeTerritories
    /// </summary>
    [Serializable]
    public class EmployeeTerritory:Entity<string>
    {
        /// <summary>
        /// 
        /// </summary>
    	public virtual int EmployeeId {  get;set; }
        /// <summary>
        /// 
        /// </summary>
    	public virtual string TerritoryId {  get;set; }
        
        
          
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
            return "EmployeeId_TerritoryId";
        }
	  
    }	
   
}
