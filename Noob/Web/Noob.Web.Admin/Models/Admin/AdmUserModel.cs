using System;
using System.Web;
using System.Data;
using FluentValidation.Attributes;
using Noob.Web.Admin.Validators;
using Orchard.Web.Mvc.Models;
namespace Noob.Web.Admin.Models
{

    /// <summary>
    /// 系统操作员
    /// </summary>
    [Serializable]
    [Validator(typeof(AdmUserModelValidator))]
    public class AdmUserModel : BaseNopModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
    	public virtual int UserID { get; set; }
        /// <summary>
        /// 组织机构ID
        /// </summary>
        public virtual int OrgID { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public virtual string UserName { get; set; }
        /// <summary>
        /// MD5加密后的密码
        /// </summary>
        public virtual string Password { get; set; }
        /// <summary>
        /// EMail
        /// </summary>
        public virtual string Email { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public virtual string TrueName { get; set; }
        /// <summary>
        /// 固定电话
        /// </summary>
        public virtual string Mobile { get; set; }
        /// <summary>
        /// 办公室电话
        /// </summary>
        public virtual string Phone { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// 用户状态(1：正常 0：帐号未启用)
        /// </summary>
        public virtual sbyte Status { get; set; }
        /// <summary>
        /// 注册IP地址
        /// </summary>
        public virtual string RegIP { get; set; }
        /// <summary>
        /// 是否是创建人
        /// </summary>
        public virtual bool? IsFounder { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public virtual DateTime? LastLoginTime { get; set; }
        /// <summary>
        /// 手机验证  0未验证  1已发验证 2验证成功
        /// </summary>
        public virtual sbyte? MobileVerify { get; set; }
        /// <summary>
        /// 手机验证时间
        /// </summary>
        public virtual DateTime? MobileVerifyTime { get; set; }


        ///// <summary>
        ///// 确认密码
        ///// </summary>
        public virtual string Password2 { get; set; }


        ///// <summary>
        ///// 用户所属用户组
        ///// </summary>
        public virtual string GroupId { get; set; }

        /// <summary>
        /// 默认帐号（操作员）ID
        /// </summary>
        public virtual int? DefaultUserId { get; set; }
    }

}
