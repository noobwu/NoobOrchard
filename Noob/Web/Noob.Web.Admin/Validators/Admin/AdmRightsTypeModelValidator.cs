using FluentValidation;
using Noob.Web.Admin.Models;
using Orchard.Web.Mvc.Validators;
using Noob.IServices;
using FluentValidation.Results;
using Orchard.Web.Mvc.Infrastructure;
namespace Noob.Web.Admin.Validators
{

    /// <summary>
    /// 渠道商权限分类表验证
    /// </summary>
    public class AdmRightsTypeModelValidator : BaseNopValidator<AdmRightsTypeModel>
    {
        public AdmRightsTypeModelValidator()
        {
            RuleFor(a => a.TypeName).NotEmpty().WithMessage("权限类别名称不能为空");

            IAdmRightsTypeService service = ContainerContext.Current.Resolve<IAdmRightsTypeService>();
            RuleFor(model => model).Custom((model, ctx) =>
            {
                if (!string.IsNullOrEmpty(model.TypeName))
                {
                    if (service.Exists(a => a.TypeName == model.TypeName && a.RightsTypeID != model.RightsTypeID && a.ParentID == model.ParentID))
                    {
                        ctx.AddFailure(new ValidationFailure("TypeName", "该名称已存在"));
                    }
                }
            });

            //RuleFor(model => model).CustomAsync(async (model, ctx, cancel) =>
            //{
            //    if (!string.IsNullOrEmpty(model.TypeName))
            //    {
            //        if (await service.ExistsAsync(a => a.TypeName == model.TypeName && a.RightsTypeID != model.RightsTypeID && a.ParentID == model.ParentID))
            //        {
            //            ctx.AddFailure(new ValidationFailure("TypeName", "该名称已存在"));
            //        }
            //    }
            //});
        }
    }

}
