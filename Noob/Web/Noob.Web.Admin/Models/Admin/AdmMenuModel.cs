using System;
using System.Web;
using System.Data;
using FluentValidation.Attributes;
using Noob.Web.Admin.Validators;
using Orchard.Web.Mvc.Models;
namespace Noob.Web.Admin.Models
{

    /// <summary>
    /// 渠道商菜单管理
    /// </summary>
    [Serializable]
    [Validator(typeof(AdmMenuModelValidator))]
    public class AdmMenuModel : BaseNopModel
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
    	public virtual int MenuID { get; set; }
        /// <summary>
        /// 上级ID
        /// </summary>
        public virtual int ParentID { get; set; }
        /// <summary>
        /// 菜单编码
        /// </summary>
        public virtual string MenuCode { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public virtual string MenuName { get; set; }
        /// <summary>
        /// 菜单地址
        /// </summary>
        public virtual string MenuUrl { get; set; }
        /// <summary>
        /// 菜单权限
        /// </summary>
        public virtual int RightsID { get; set; }
        /// <summary>
        /// 菜单类型(0:菜单类别,1:菜单)
        /// </summary>
        public virtual sbyte MenuType { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public virtual sbyte StatusFlag { get; set; }
        /// <summary>
        /// 排序值 越小越靠前，默认9999
        /// </summary>
        public virtual int SortOrder { get; set; }


        /// <summary>
        /// 权限类别编号
        /// </summary>
        public int RightsTypeID { get; set; }

    }

}
