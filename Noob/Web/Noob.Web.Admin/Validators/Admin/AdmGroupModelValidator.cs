using FluentValidation;
using Noob.Web.Admin.Models;
using Orchard.Web.Mvc.Validators;
using Noob.IServices;
using FluentValidation.Results;
using Orchard.Web.Mvc.Infrastructure;

namespace Noob.Web.Admin.Validators
{

    /// <summary>
    /// 渠道用户组验证
    /// </summary>
    public class AdmGroupModelValidator : BaseNopValidator<AdmGroupModel>
    {
        public AdmGroupModelValidator()
        {
            RuleFor(a => a.GroupName).NotEmpty().WithMessage("用户员组名称不能为空");
            RuleFor(a => a.Description).NotEmpty().WithMessage("说明不能为空");
            IAdmGroupService service = ContainerContext.Current.Resolve<IAdmGroupService>();
            RuleFor(model => model).Custom((model, ctx) =>
            {
                if (!string.IsNullOrEmpty(model.GroupName))
                {
                    if (service.Exists(a => a.GroupName == model.GroupName && a.GroupID != model.GroupID))
                    {
                        ctx.AddFailure(new ValidationFailure("GroupName", "该名称已存在"));
                    }
                }
                if (model.GroupType != 0 && model.GroupType != 1)
                {
                    ctx.AddFailure(new ValidationFailure("GroupType", "无效用户组类型"));
                }
            });
            //RuleFor(model => model).CustomAsync(async (model, ctx, cancel) =>
            //{
            //    if (!string.IsNullOrEmpty(model.GroupName))
            //    {
            //        if (await service.ExistsAsync(a => a.GroupName == model.GroupName && a.GroupID != model.GroupID))
            //        {
            //            ctx.AddFailure(new ValidationFailure("GroupName", "该名称已存在"));
            //        }
            //    }
            //    if (model.GroupType != 0 && model.GroupType != 1)
            //    {
            //        ctx.AddFailure(new ValidationFailure("GroupType", "无效用户组类型"));
            //    }
            //});
        }
    }

}
