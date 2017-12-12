using System;
using System.Web;
using System.Data;
using FluentValidation.Attributes;
using Noob.Web.Admin.Validators;
using Orchard.Web.Mvc.Models;

namespace Noob.Web.Admin.Models
{

    /// <summary>
    /// 广告位
    /// </summary>
    [Serializable]
    [Validator(typeof(CmsAdvPositionModelValidator))]
    public class CmsAdvPositionModel:BaseNopModel
    {
        /// <summary>
        /// 广告位ID
        /// </summary>
    	public virtual int AdvPositionId {get;set;}
    	/// <summary>
        /// 广告位置编号
        /// </summary>
    	public virtual string AdvPositionCode { get;set;}
    	/// <summary>
        /// 广告位名称
        /// </summary>
    	public virtual string AdvPositionName { get;set;}
    	/// <summary>
        /// 状态(1:启用  0:禁用)
        /// </summary>
    	public virtual byte Status { get;set;}
    	/// <summary>
        /// 广告位宽度
        /// </summary>
    	public virtual int Width { get;set;}
    	/// <summary>
        /// 广告位高度
        /// </summary>
    	public virtual int Height { get;set;}
    	/// <summary>
        /// 排序值 越小越靠前，默认9999
        /// </summary>
    	public virtual int SortOrder { get;set;}
    	/// <summary>
        /// 描述
        /// </summary>
    	public virtual string Remark { get;set;}
        
    }	
   
}
