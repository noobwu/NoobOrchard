using System;
using System.Web;
using System.Data;
using FluentValidation.Attributes;
using Noob.Web.Admin.Validators;
using Orchard.Web.Mvc.Models;
namespace Noob.Web.Admin.Models
{

    /// <summary>
    /// 系统配置
    /// </summary>
    [Serializable]
    [Validator(typeof(AdmConfigsModelValidator))]
    public class AdmConfigsModel : BaseNopModel
    {
        /// <summary>
        /// 系统配置ID
        /// </summary>
        public virtual int ConfigID { get; set; }
        /// <summary>
        /// 配置标识名称
        /// </summary>
        public virtual string ConfigCode { get; set; }
        /// <summary>
        /// 配置内容
        /// </summary>
        public virtual string ConfigValue { get; set; }
        /// <summary>
        /// 配置名称
        /// </summary>
        public virtual string ConfigName { get; set; }
        /// <summary>
        /// 配置组名称
        /// </summary>
        public virtual string ConfigGroupName { get; set; }
        /// <summary>
        /// 状态(1:启用,0:禁用)
        /// </summary>
        public virtual byte Status { get; set; }
        /// <summary>
        /// 排序值 越小越靠前，默认9999
        /// </summary>
        public virtual int SortOrder { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Remark { get; set; }

    }

}
