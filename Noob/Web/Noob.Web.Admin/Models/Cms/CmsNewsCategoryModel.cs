using System;
using System.Web;
using System.Data;
using FluentValidation.Attributes;
using Noob.Web.Admin.Validators;
using Orchard.Web.Mvc.Models;

namespace Noob.Web.Admin.Models
{

    /// <summary>
    /// 资讯类别
    /// </summary>
    [Serializable]
    [Validator(typeof(CmsNewsCategoryModelValidator))]
    public class CmsNewsCategoryModel:BaseNopModel
    {
        /// <summary>
        /// 资讯类别Id
        /// </summary>
    	public virtual int CategoryId {get;set;}
    	/// <summary>
        /// 类别名称
        /// </summary>
    	public virtual string CategoryName { get;set;}
    	/// <summary>
        /// 类别标识
        /// </summary>
    	public virtual string CategoryCode { get;set;}
    	/// <summary>
        /// 父类编号
        /// </summary>
    	public virtual int ParentID { get;set;}
    	/// <summary>
        /// 排序值 越小越靠前，默认9999
        /// </summary>
    	public virtual int SortOrder { get;set;}
    	/// <summary>
        /// 类别类型
        /// </summary>
    	public virtual byte CategoryType { get;set;}
        /// <summary>
        /// 图片地址
        /// </summary>
        public virtual string ImageUrl { get; set; }
        
    }	
   
}
