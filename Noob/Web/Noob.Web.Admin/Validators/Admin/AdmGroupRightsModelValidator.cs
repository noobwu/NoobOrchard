using FluentValidation;
using Noob.Web.Admin.Models;
using Orchard.Web.Mvc.Validators;
namespace Noob.Web.Admin.Validators
{

    /// <summary>
    /// 渠道用户组权限验证
    /// </summary>
    public class AdmGroupRightsModelValidator : BaseNopValidator<AdmGroupRightsModel>
    {
        public AdmGroupRightsModelValidator()
        {
        }
    }

}
