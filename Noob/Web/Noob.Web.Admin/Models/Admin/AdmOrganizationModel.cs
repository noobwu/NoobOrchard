using System;
using System.Web;
using System.Data;
using FluentValidation.Attributes;
using Noob.Web.Admin.Validators;
using Orchard.Web.Mvc.Models;
namespace Noob.Web.Admin.Models
{

    /// <summary>
    /// 组织机构
    /// </summary>
    [Serializable]
    [Validator(typeof(AdmOrganizationModelValidator))]
    public class AdmOrganizationModel : BaseNopModel
    {
        /// <summary>
        /// 组织机构ID
        /// </summary>
        public virtual int OrgID { get; set; }
        /// <summary>
        /// 组织机构简称
        /// </summary>
        public virtual string OrgName { get; set; }
        /// <summary>
        /// 部门类型 0、公司 1、分公司 2、部门
        /// </summary>
        public virtual byte OrgType { get; set; }
        /// <summary>
        /// 上级ID
        /// </summary>
        public virtual int ParentID { get; set; }
        /// <summary>
        /// 办公电话
        /// </summary>
        public virtual string OfficePhone { get; set; }
        /// <summary>
        /// 部门地址
        /// </summary>
        public virtual string Address { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public virtual string TeamLeader { get; set; }
        /// <summary>
        /// 默认帐号（操作员）ID
        /// </summary>
        public virtual int DefaultUserId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public virtual byte StatusFlag { get; set; }
        /// <summary>
        /// 排序值 越小越靠前，默认9999
        /// </summary>
        public virtual int SortOrder { get; set; }

    }

}
