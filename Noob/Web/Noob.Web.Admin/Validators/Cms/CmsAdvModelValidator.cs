using FluentValidation;
using FluentValidation.Results;
using Noob.IServices;
using Noob.Web.Admin.Models;
using Orchard.Web.Mvc.Infrastructure;
using Orchard.Web.Mvc.Validators;

namespace Noob.Web.Admin.Validators
{

    /// <summary>
    /// 广告验证
    /// </summary>
    public class CmsAdvModelValidator : BaseNopValidator<CmsAdvModel>
    {
        public CmsAdvModelValidator()
        {
            RuleFor(a => a.AdvName).NotEmpty().WithMessage("广告名称不能为空");
            RuleFor(a => a.AdvCode).NotEmpty().WithMessage("广告标识不能为空");
            RuleFor(a => a.AdvTitle).NotEmpty().WithMessage("广告标题不能为空");
            // RuleFor(a => a.AdvHtmlCode).NotEmpty().WithMessage("广告Html代码不能为空");
            RuleFor(a => a.ImageUrl).NotEmpty().WithMessage("广告图片地址不能为空");
            RuleFor(a => a.Remark).NotEmpty().WithMessage("描述不能为空");
            ICmsAdvService service = ContainerContext.Current.Resolve<ICmsAdvService>();
            RuleFor(model => model).Custom((model, ctx) =>
            {
                if (!string.IsNullOrEmpty(model.AdvName) && !string.IsNullOrEmpty(model.AdvCode)
                    && !string.IsNullOrEmpty(model.AdvTitle)
                    && !string.IsNullOrEmpty(model.ImageUrl)
                    && !string.IsNullOrEmpty(model.Remark))
                {
                    if (service.Exists(a => a.AdvCode == model.AdvCode && a.AdvID != model.AdvID))
                    {
                        ctx.AddFailure(new ValidationFailure("AdvCode", "广告标识已存在"));
                    }

                    if (service.Exists(a => a.AdvName == model.AdvName && a.AdvID != model.AdvID))
                    {
                        ctx.AddFailure(new ValidationFailure("AdvName", "广告名称已存在"));
                    }
                }
            });

            ////model会出错 暂时不用
            //RuleFor(model => model).CustomAsync(async (model, ctx, cancel) =>
            //{
            //    if (!string.IsNullOrEmpty(model.AdvName) && !string.IsNullOrEmpty(model.AdvCode)
            //        && !string.IsNullOrEmpty(model.AdvTitle)
            //        && !string.IsNullOrEmpty(model.ImageUrl)
            //        && !string.IsNullOrEmpty(model.Remark))
            //    {
            //        if (await service.ExistsAsync(a => a.AdvCode == model.AdvCode && a.AdvID != model.AdvID))
            //        {
            //            ctx.AddFailure(new ValidationFailure("AdvCode", "广告标识已存在"));
            //        }

            //        if (await service.ExistsAsync(a => a.AdvName == model.AdvName && a.AdvID != model.AdvID))
            //        {
            //            ctx.AddFailure(new ValidationFailure("AdvName", "广告名称已存在"));
            //        }
            //    }
            //});

        }
    }

}
