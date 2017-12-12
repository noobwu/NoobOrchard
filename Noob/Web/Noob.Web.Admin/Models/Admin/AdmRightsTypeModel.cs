using System;
using System.Web;
using System.Data;
using FluentValidation.Attributes;
using Noob.Web.Admin.Validators;
using Orchard.Web.Mvc.Models;
namespace Noob.Web.Admin.Models
{

    /// <summary>
    /// 渠道商权限分类表
    /// </summary>
    [Serializable]
    [Validator(typeof(AdmRightsTypeModelValidator))]
    public class AdmRightsTypeModel : BaseNopModel
    {
        /// <summary>
        /// 权限类别编号
        /// </summary>
        public virtual int RightsTypeID { get; set; }
        /// <summary>
        /// 权限类别名称
        /// </summary>
        public virtual string TypeName { get; set; }
        /// <summary>
        /// 父类编号
        /// </summary>
        public virtual int ParentID { get; set; }
        /// <summary>
        /// 排序值 越小越靠前，默认9999
        /// </summary>
        public virtual int SortOrder { get; set; }

    }

}
