using System;
using System.Web;
using System.Data;
using FluentValidation.Attributes;
using Noob.Web.Admin.Validators;
using Orchard.Web.Mvc.Models;
namespace Noob.Web.Admin.Models
{

    /// <summary>
    /// 支付方式配置
    /// </summary>
    [Serializable]
    [Validator(typeof(AdmPayConfigsModelValidator))]
    public class AdmPayConfigsModel:BaseNopModel
    {
        /// <summary>
        /// 系统配置ID
        /// </summary>
    	public virtual int PayConfigID {get;set;}
    	/// <summary>
        /// 支付方式名称
        /// </summary>
    	public virtual string PayName { get;set;}
    	/// <summary>
        /// 支付编号
        /// </summary>
    	public virtual string PayCode { get;set;}
    	/// <summary>
        /// 支付配置信息
        /// </summary>
    	public virtual string PayConfig { get;set;}
    	/// <summary>
        /// 支付类型（0:后台充值 1:在线支付 ）
        /// </summary>
    	public virtual int PayType { get;set;}
    	/// <summary>
        /// logo图标
        /// </summary>
    	public virtual string Logo { get;set;}
    	/// <summary>
        /// 描述
        /// </summary>
    	public virtual string Remark { get;set;}
    	/// <summary>
        /// 支付帮助说明（提示用户）
        /// </summary>
    	public virtual string Note { get;set;}
    	/// <summary>
        /// 支付方式状态(0，开发中 1 开启，2 关闭)
        /// </summary>
    	public virtual byte Status { get;set;}
    	/// <summary>
        /// 排序值 越小越靠前，默认9999
        /// </summary>
    	public virtual int SortOrder { get;set;}
        
    }	
   
}
