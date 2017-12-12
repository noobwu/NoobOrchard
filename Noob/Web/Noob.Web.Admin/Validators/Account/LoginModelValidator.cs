using FluentValidation;
using Orchard.Web.Mvc.Validators;
using Noob.Web.Admin.Models.Account;

namespace Noob.Web.Admin.Validators
{

    /// <summary>
    /// 登录验证
    /// </summary>
    public class LoginModelValidator : BaseNopValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(a => a.UserName).NotEmpty().WithMessage("登录名不能为空");
            RuleFor(a => a.Password).NotEmpty().WithMessage("密码不能为空");
        }
    }

}
