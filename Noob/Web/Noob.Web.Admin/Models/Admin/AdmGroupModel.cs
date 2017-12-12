using System;
using System.Web;
using System.Data;
using FluentValidation.Attributes;
using Noob.Web.Admin.Validators;
using Orchard.Web.Mvc.Models;
namespace Noob.Web.Admin.Models
{

    /// <summary>
    /// 渠道用户组
    /// </summary>
    [Serializable]
    [Validator(typeof(AdmGroupModelValidator))]
    public class AdmGroupModel : BaseNopModel
    {
        /// <summary>
        /// 用户组编号
        /// </summary>
    	public virtual int GroupID { get; set; }
        /// <summary>
        /// 管理员组名称
        /// </summary>
        public virtual string GroupName { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// 用户组类型(0:系统组，1:商户组)
        /// </summary>
        public virtual sbyte GroupType { get; set; }

        /// <summary>
        /// 排序值 越小越靠前，默认9999
        /// </summary>
        public virtual int SortOrder { get; set; }

    }

}
