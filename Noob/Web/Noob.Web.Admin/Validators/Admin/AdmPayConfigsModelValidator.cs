using FluentValidation;
using FluentValidation.Results;
using Noob.Web.Admin.Models;
using Orchard.Web.Mvc.Validators;
using Noob.IServices;
using Orchard.Web.Mvc.Infrastructure;
namespace Noob.Web.Admin.Validators
{

    /// <summary>
    /// 支付方式配置验证
    /// </summary>
    public class AdmPayConfigsModelValidator : BaseNopValidator<AdmPayConfigsModel>
    {
        public AdmPayConfigsModelValidator()
        {
            RuleFor(a => a.PayName).NotEmpty().WithMessage("支付方式名称不能为空");
            RuleFor(a => a.PayCode).NotEmpty().WithMessage("支付编号不能为空");
            RuleFor(a => a.PayConfig).NotEmpty().WithMessage("支付配置信息不能为空");
            //RuleFor(a => a.Logo).NotEmpty().WithMessage("logo图标不能为空");
            RuleFor(a => a.Remark).NotEmpty().WithMessage("描述不能为空");
            RuleFor(a => a.Note).NotEmpty().WithMessage("支付帮助说明（提示用户）不能为空");
            IAdmPayConfigsService service = ContainerContext.Current.Resolve<IAdmPayConfigsService>();
            RuleFor(model => model).Custom((model, ctx) =>
            {
                if (!string.IsNullOrEmpty(model.PayCode))
                {
                    if (service.Exists(a => a.PayCode == model.PayCode && a.PayConfigID != model.PayConfigID))
                    {
                        ctx.AddFailure(new ValidationFailure("PayCode", "支付标识已存在"));
                    }
                }
                if (!string.IsNullOrEmpty(model.PayName))
                {
                    if (service.Exists(a => a.PayName == model.PayName && a.PayConfigID != model.PayConfigID))
                    {
                        ctx.AddFailure(new ValidationFailure("AdvName", "支付名称已存在"));
                    }
                }
            });
            //RuleFor(model => model).CustomAsync(async (model, ctx, cancel) =>
            //{
            //    if (!string.IsNullOrEmpty(model.PayCode))
            //    {
            //        if (await service.ExistsAsync(a => a.PayCode == model.PayCode && a.PayConfigID != model.PayConfigID))
            //        {
            //            ctx.AddFailure(new ValidationFailure("PayCode", "支付标识已存在"));
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(model.PayName))
            //    {
            //        if (await service.ExistsAsync(a => a.PayName == model.PayName && a.PayConfigID != model.PayConfigID))
            //        {
            //            ctx.AddFailure(new ValidationFailure("AdvName", "支付名称已存在"));
            //        }
            //    }
            //});
        }
    }

}
