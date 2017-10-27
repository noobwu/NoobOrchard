using System;
using System.Web;
using System.Data;
using Orchard.Domain.Entities;
namespace Orchard.Tests.Common.Domain.Entities
{
    /// <summary>
    /// wt_adm_area_test
    /// </summary>
    [Serializable]
    public  class AdmAreaTest : Entity<int>, ISoftDelete
    {
        /// <summary>
        /// 
        /// </summary>
    	public override int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string AreaId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string AreaName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string ParentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string ShortName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual byte LevelType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string CityCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string ZipCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string AreaNamePath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string AreaIdPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual decimal Lng { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual decimal Lat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string PinYin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string ShortPinYin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string PYFirstLetter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int SortOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual byte Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int CreateUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int UpdateUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool DeleteFlag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime DeleteTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int DeleteUser { get; set; }



        /// <summary>
        /// 获取主键的属性名称
        /// </summary>
        /// <returns></returns>
        public override string GetPKPropertyName()
        {
            return "Id";
        }

    }

}
