using FluentValidation;
using FluentValidation.Results;
using Noob.Web.Admin.Models;
using Orchard.Web.Mvc.Validators;
using Noob.IServices;
using Orchard.Web.Mvc.Infrastructure;

namespace Noob.Web.Admin.Validators
{

    /// <summary>
    /// 系统配置验证
    /// </summary>
    public class AdmConfigsModelValidator : BaseNopValidator<AdmConfigsModel>
    {
        public AdmConfigsModelValidator()
        {

            RuleFor(a => a.ConfigCode).NotEmpty().WithMessage("配置标识名称不能为空");
            RuleFor(a => a.ConfigName).NotEmpty().WithMessage("配置名称不能为空");
            RuleFor(a => a.ConfigValue).NotEmpty().WithMessage("配置值不能为空");
            RuleFor(a => a.ConfigGroupName).NotEmpty().WithMessage("配置组名称不能为空");
            //RuleFor(a => a.RightsID).InclusiveBetween(1, 10);
            IAdmConfigsService service = ContainerContext.Current.Resolve<IAdmConfigsService>();
            RuleFor(model => model).Custom((model, ctx) =>
                   {
                       if (!string.IsNullOrEmpty(model.ConfigCode))
                       {
                           if (service.Exists(a => a.ConfigGroupName == model.ConfigGroupName && a.ConfigCode == model.ConfigCode && a.ConfigID != model.ConfigID))
                           {
                               ctx.AddFailure(new ValidationFailure("ConfigCode", "配置标识名称已存在"));
                           }
                       }
                       if (!string.IsNullOrEmpty(model.ConfigName))
                       {
                           if (service.Exists(a => a.ConfigGroupName == model.ConfigGroupName && a.ConfigName == model.ConfigName && a.ConfigID != model.ConfigID))
                           {
                               ctx.AddFailure(new ValidationFailure("ConfigName", "配置名称已存在"));
                           }
                       }
                   });
            //RuleFor(model=>model).CustomAsync(async (model, ctx, cancel)=>
            //     {
            //        if (!string.IsNullOrEmpty(model.ConfigCode))
            //        {
            //            if (await service.ExistsAsync(a => a.ConfigGroupName == model.ConfigGroupName && a.ConfigCode == model.ConfigCode && a.ConfigID != model.ConfigID))
            //            {
            //                 ctx.AddFailure(new ValidationFailure("ConfigCode", "配置标识名称已存在"));
            //            }
            //        }
            //        if (!string.IsNullOrEmpty(model.ConfigName))
            //        {
            //            if (await service.ExistsAsync(a => a.ConfigGroupName == model.ConfigGroupName && a.ConfigName == model.ConfigName && a.ConfigID != model.ConfigID))
            //            {
            //                 ctx.AddFailure(new ValidationFailure("ConfigName", "配置名称已存在"));
            //            }
            //        }
            //    });
        }
    }

}
