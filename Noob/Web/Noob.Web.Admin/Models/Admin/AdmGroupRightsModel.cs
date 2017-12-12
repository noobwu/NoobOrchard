using System;
using System.Web;
using System.Data;
using FluentValidation.Attributes;
using Noob.Web.Admin.Validators;
using Orchard.Web.Mvc.Models;
namespace Noob.Web.Admin.Models
{

    /// <summary>
    /// 渠道用户组权限
    /// </summary>
    [Serializable]
    [Validator(typeof(AdmGroupRightsModelValidator))]
    public class AdmGroupRightsModel : BaseNopModel
    {
        /// <summary>
        /// 用户组权限ID
        /// </summary>
    	public virtual int GroupRightsID { get; set; }
        /// <summary>
        /// 用户组ID
        /// </summary>
        public virtual int GroupID { get; set; }
        /// <summary>
        /// 权限编号
        /// </summary>
        public virtual int RightsID { get; set; }

    }

}
