using FluentValidation;
using FluentValidation.Results;
using Noob.Web.Admin.Models;
using Orchard.Web.Mvc.Validators;
using Noob.IServices;
using Orchard.Web.Mvc.Infrastructure;

namespace Noob.Web.Admin.Validators
{

    /// <summary>
    /// 广告位验证
    /// </summary>
    public class CmsAdvPositionModelValidator : BaseNopValidator<CmsAdvPositionModel>
    {
        public CmsAdvPositionModelValidator()
        {
            RuleFor(a => a.AdvPositionCode).NotEmpty().WithMessage("广告位置编号不能为空");
            RuleFor(a => a.AdvPositionName).NotEmpty().WithMessage("广告位名称不能为空");
            RuleFor(a => a.Remark).NotEmpty().WithMessage("描述不能为空");
            ICmsAdvPositionService service = ContainerContext.Current.Resolve<ICmsAdvPositionService>();
            RuleFor(model => model).Custom((model, ctx) =>
            {
                if (!string.IsNullOrEmpty(model.AdvPositionCode) && !string.IsNullOrEmpty(model.AdvPositionName) && !string.IsNullOrEmpty(model.Remark))
                {
                    if (service.Exists(a => a.AdvPositionCode == model.AdvPositionCode && a.AdvPositionId != model.AdvPositionId))
                    {
                        ctx.AddFailure(new ValidationFailure("AdvPositionCode", "广告位置编号已存在"));
                    }
                    if (service.Exists(a => a.AdvPositionName == model.AdvPositionName && a.AdvPositionId != model.AdvPositionId))
                    {
                        ctx.AddFailure(new ValidationFailure("AdvPositionName", "广告位置名称已存在"));
                    }
                }
            });
        }
    }

}
