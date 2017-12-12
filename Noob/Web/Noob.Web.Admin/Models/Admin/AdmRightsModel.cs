using System;
using System.Web;
using System.Data;
using FluentValidation.Attributes;
using Noob.Web.Admin.Validators;
using Orchard.Web.Mvc.Models;
namespace Noob.Web.Admin.Models
{

    /// <summary>
    /// (渠道商权限)
    /// </summary>
    [Serializable]
    [Validator(typeof(AdmRightsModelValidator))]
    public class AdmRightsModel : BaseNopModel
    {
        /// <summary>
        /// 权限编号
        /// </summary>
    	public virtual int RightsID { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        public virtual string RightsCode { get; set; }
        /// <summary>
        /// 权限显示名称
        /// </summary>
        public virtual string RightsName { get; set; }
        /// <summary>
        /// 权限类别编号
        /// </summary>
        public virtual int RightsTypeID { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// 排序值 越小越靠前，默认9999
        /// </summary>
        public virtual int SortOrder { get; set; }
        /// <summary>
        /// 权限类型(0:菜单权限 1:普通权限)
        /// </summary>
        public virtual byte RightsType { get; set; }

    }

}
