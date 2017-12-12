using Noob.Web.Admin.Models;
using FluentValidation;
using FluentValidation.Results;
using Noob.IServices;
using Orchard.Web.Mvc.Infrastructure;
using Orchard.Web.Mvc.Validators;

namespace Noob.Web.Admin.Validators
{

    /// <summary>
    /// 资讯类别验证
    /// </summary>
    public class CmsNewsCategoryModelValidator : BaseNopValidator<CmsNewsCategoryModel>
    {
        public CmsNewsCategoryModelValidator()
        {
            RuleFor(a => a.CategoryName).NotEmpty().WithMessage("类别名称不能为空");
            RuleFor(a => a.CategoryCode).NotEmpty().WithMessage("类别标识不能为空");
            //RuleFor(a => a.ImageUrl).NotEmpty().WithMessage("图片地址不能为空");
            ICmsNewsCategoryService service = ContainerContext.Current.Resolve<ICmsNewsCategoryService>();
            RuleFor(model => model).Custom((model, ctx) =>
            {
                if (string.IsNullOrEmpty(model.ImageUrl))
                {
                    model.ImageUrl = string.Empty;
                }
                if (!string.IsNullOrEmpty(model.CategoryName) && !string.IsNullOrEmpty(model.CategoryName))
                {
                    if (service.Exists(a => a.CategoryCode == model.CategoryCode && a.CategoryId != model.CategoryId))
                    {
                        ctx.AddFailure(new ValidationFailure("CategoryCode", "类别标识已存在"));
                    }
                    if (service.Exists(a => a.CategoryName == model.CategoryName && a.CategoryId != model.CategoryId))
                    {
                        ctx.AddFailure(new ValidationFailure("CategoryName", "类别名称已存在"));
                    }
                }
            });
        }
    }

}
