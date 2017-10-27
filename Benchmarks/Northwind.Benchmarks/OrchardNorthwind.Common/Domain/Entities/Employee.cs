using System;
using System.Web;
using System.Data;
using Orchard.Domain.Entities;
namespace OrchardNorthwind.Common.Entities
{
    /// <summary>
    /// Employees
    /// </summary>
    [Serializable]
    public class Employee:Entity<int>
    {
        /// <summary>
        /// 
        /// </summary>
    	public virtual int EmployeeId {  get;set; }
        
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string LastName { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string FirstName { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string Title { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string TitleOfCourtesy { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual DateTime? BirthDate { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual DateTime? HireDate { get;set;}
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
    	public virtual string HomePhone { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string Extension { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual byte[] Photo { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string Notes { get;set;}
    	/// <summary>
        /// 
        /// </summary>
    	public virtual string PhotoPath { get;set;}
        
        /// <summary>
        /// 
        /// </summary>
    	public virtual   int? ReportsTo { get;set;}
          
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
		public override int  Id { get { return EmployeeId; } set { EmployeeId=value; }}
        /// <summary>
        /// 获取主键的属性名称
        /// </summary>
        /// <returns></returns>
        public override string GetPKPropertyName()
        {
            return "EmployeeId";
        }
	  
    }	
   
}
