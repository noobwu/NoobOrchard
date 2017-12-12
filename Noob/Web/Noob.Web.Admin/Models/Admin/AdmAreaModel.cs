using System;
using System.Web;
using System.Data;
using FluentValidation.Attributes;
using Noob.Web.Admin.Validators;
using Orchard.Web.Mvc.Models;

namespace Noob.Web.Admin.Models
{

    /// <summary>
    /// 地区
    /// </summary>
    [Serializable]
    [Validator(typeof(AdmAreaModelValidator))]
    public class AdmAreaModel:BaseNopModel
    {
        /// <summary>
        /// 自增长ID
        /// </summary>
        public virtual int Id { get; set; }
    	/// <summary>
        /// 地区ID
        /// </summary>
    	public virtual string AreaID { get;set;}
    	/// <summary>
        /// 地区名称
        /// </summary>
    	public virtual string AreaName { get;set;}
    	/// <summary>
        /// 父级ID
        /// </summary>
    	public virtual string ParentId { get;set;}
    	/// <summary>
        /// 地区简称
        /// </summary>
    	public virtual string ShortName { get;set;}
    	/// <summary>
        /// 地区类型(0:国家,1:直辖市或省份 2:市 3:区或者县)
        /// </summary>
    	public virtual byte LevelType { get;set;}
    	/// <summary>
        /// 区号
        /// </summary>
    	public virtual string CityCode { get;set;}
    	/// <summary>
        /// 邮编
        /// </summary>
    	public virtual string ZipCode { get;set;}
    	/// <summary>
        /// 完整地区名称
        /// </summary>
    	public virtual string AreaNamePath { get;set;}
    	/// <summary>
        /// 完整地区名称
        /// </summary>
    	public virtual string AreaIDPath { get;set;}
    	/// <summary>
        /// 经度
        /// </summary>
    	public virtual decimal Lng { get;set;}
    	/// <summary>
        /// 纬度
        /// </summary>
    	public virtual decimal Lat { get;set;}
    	/// <summary>
        /// 拼音
        /// </summary>
    	public virtual string PinYin { get;set;}
    	/// <summary>
        /// 拼音缩写
        /// </summary>
    	public virtual string ShortPinYin { get;set;}
    	/// <summary>
        /// 拼音第一个字母
        /// </summary>
    	public virtual string PYFirstLetter { get;set;}
    	/// <summary>
        /// 排序值
        /// </summary>
    	public virtual int SortOrder { get;set;}
    	/// <summary>
        /// 状态(1:启用,0:禁用)
        /// </summary>
    	public virtual byte Status { get;set;}
        /// <summary>
        /// 是否是热门城市
        /// </summary>
        public virtual byte HotCity { get; set; }
        
    }	
   
}
