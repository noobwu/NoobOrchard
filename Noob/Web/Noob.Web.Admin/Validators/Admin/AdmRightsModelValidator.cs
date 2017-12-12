using FluentValidation;
using Noob.Web.Admin.Models;
using Orchard.Web.Mvc.Validators;
using Noob.IServices;
using FluentValidation.Results;
using Orchard.Web.Mvc.Infrastructure;
namespace Noob.Web.Admin.Validators
{

    /// <summary>
    /// (渠道商权限)验证
    /// </summary>
    public class AdmRightsModelValidator : BaseNopValidator<AdmRightsModel>
    {
        public AdmRightsModelValidator()
        {
            RuleFor(a => a.RightsCode).NotEmpty().WithMessage("权限名称不能为空");
            RuleFor(a => a.RightsName).NotEmpty().WithMessage("权限显示名称不能为空");
            RuleFor(a => a.Description).NotEmpty().WithMessage("说明不能为空");
            RuleFor(a => a.RightsTypeID).GreaterThan(0).WithMessage("无效权限类别编号");
            //RuleFor(a => a.RightsID).InclusiveBetween(1, 10);
            IAdmRightsService service = ContainerContext.Current.Resolve<IAdmRightsService>();
            RuleFor(model => model).Custom((model, ctx) =>
            {
                if (model.RightsType != 0 && model.RightsType != 1)
                {
                    ctx.AddFailure(new ValidationFailure("RightsType", "无效权限类型"));
                }
                if (!string.IsNullOrEmpty(model.RightsCode))
                {
                    if (service.Exists(a => a.RightsCode == model.RightsCode && a.RightsID != model.RightsID))
                    {
                        ctx.AddFailure(new ValidationFailure("RightsCode", "该权限代码已存在"));
                    }
                }
                else if (!string.IsNullOrEmpty(model.RightsName))
                {
                    if (service.Exists(a => a.RightsName == model.RightsName && a.RightsTypeID == model.RightsTypeID && a.RightsID != model.RightsID))
                    {
                        ctx.AddFailure(new ValidationFailure("RightsCode", "该权限显示名称已存在"));
                    }
                }
            });
            //RuleFor(model => model).CustomAsync(async (model, ctx, cancel) =>
            //{
            //    if (model.RightsType != 0 && model.RightsType != 1)
            //    {
            //        ctx.AddFailure(new ValidationFailure("RightsType", "无效权限类型"));
            //    }
            //    if (!string.IsNullOrEmpty(model.RightsCode))
            //    {
            //        if (await service.ExistsAsync(a => a.RightsCode == model.RightsCode && a.RightsID != model.RightsID))
            //        {
            //            ctx.AddFailure(new ValidationFailure("RightsCode", "该权限代码已存在"));
            //        }
            //    }
            //    else if (!string.IsNullOrEmpty(model.RightsName))
            //    {
            //        if (await service.ExistsAsync(a => a.RightsName == model.RightsName && a.RightsTypeID == model.RightsTypeID && a.RightsID != model.RightsID))
            //        {
            //            ctx.AddFailure(new ValidationFailure("RightsCode", "该权限显示名称已存在"));
            //        }
            //    }
            //});
        }
    }

}
