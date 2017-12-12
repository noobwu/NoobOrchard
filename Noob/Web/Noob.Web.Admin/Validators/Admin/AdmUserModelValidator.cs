using FluentValidation;
using Noob.Web.Admin.Models;
using Orchard.Web.Mvc.Validators;
using Noob.IServices;
using FluentValidation.Results;
using Orchard.Web.Mvc.Infrastructure;
namespace Noob.Web.Admin.Validators
{

    /// <summary>
    /// 渠道商操作员验证
    /// </summary>
    public class AdmUserModelValidator : BaseNopValidator<AdmUserModel>
    {
        public AdmUserModelValidator()
        {
            RuleFor(a => a.UserName).NotEmpty().WithMessage("登录名不能为空");
            RuleFor(a => a.Email).NotEmpty().WithMessage("EMail地址不能为空").EmailAddress().WithMessage("EMail无效");
            RuleFor(a => a.TrueName).NotEmpty().WithMessage("姓名不能为空");
            RuleFor(a => a.Phone).NotEmpty().WithMessage("办公室电话不能为空");
            RuleFor(a => a.Description).NotEmpty().WithMessage("说明不能为空");
            IAdmUserService service = ContainerContext.Current.Resolve<IAdmUserService>();
            RuleFor(model => model).Custom((model, ctx) =>
            {
                if (model.UserID < 1)
                {
                    RuleFor(a => a.Password).NotEmpty().WithMessage("密码不能为空").Equal(a => a.Password2).WithMessage("两次输入的密码不一致");
                    return;
                }
                if (!string.IsNullOrEmpty(model.UserName))
                {
                    if (service.Exists(a => a.UserName == model.UserName && a.UserID != model.UserID))
                    {
                        ctx.AddFailure(new ValidationFailure("UserName", "该账号名称已存在"));
                        return;
                    }
                }
                if (!string.IsNullOrEmpty(model.TrueName))
                {
                    if (service.Exists(a => a.TrueName == model.TrueName && a.UserID != model.UserID))
                    {
                        ctx.AddFailure(new ValidationFailure("UserName", "该真实姓名已存在"));
                        return;
                    }
                }
            });
            //RuleFor(model => model).CustomAsync(async (model, ctx, cancel) =>
            //{
            //    if (model.UserID < 1)
            //    {
            //        RuleFor(a => a.Password).NotEmpty().WithMessage("密码不能为空").Equal(a => a.Password2).WithMessage("两次输入的密码不一致");
            //        return;
            //    }
            //    if (!string.IsNullOrEmpty(model.UserName))
            //    {
            //        if (await service.ExistsAsync(a => a.UserName == model.UserName && a.UserID != model.UserID))
            //        {
            //            ctx.AddFailure(new ValidationFailure("UserName", "该账号名称已存在"));
            //            return;
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(model.TrueName))
            //    {
            //        if (await service.ExistsAsync(a => a.TrueName == model.TrueName && a.UserID != model.UserID))
            //        {
            //            ctx.AddFailure(new ValidationFailure("UserName", "该真实姓名已存在"));
            //            return;
            //        }
            //    }
            //});

        }
    }

}
