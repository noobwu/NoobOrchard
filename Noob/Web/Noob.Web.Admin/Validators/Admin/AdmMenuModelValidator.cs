using FluentValidation;
using Noob.Web.Admin.Models;
using Orchard.Web.Mvc.Validators;
using Noob.IServices;
using FluentValidation.Results;
using Orchard.Web.Mvc.Infrastructure;
namespace Noob.Web.Admin.Validators
{

    /// <summary>
    /// 渠道商菜单管理验证
    /// </summary>
    public class AdmMenuModelValidator : BaseNopValidator<AdmMenuModel>
    {
        public AdmMenuModelValidator()
        {
            //RuleFor(a => a.MenuCode).NotEmpty().WithMessage("菜单编码不能为空");
            RuleFor(a => a.MenuName).NotEmpty().WithMessage("菜单名称不能为空");
            //RuleFor(a => a.MenuUrl).NotEmpty().WithMessage("菜单地址不能为空");
            IAdmMenuService service = ContainerContext.Current.Resolve<IAdmMenuService>();
            RuleFor(model => model).Custom( (model, ctx) =>
            {
                if (!string.IsNullOrEmpty(model.MenuName))
                {
                    if (service.Exists(a => a.MenuName == model.MenuName && a.MenuID != model.MenuID && a.ParentID == model.ParentID))
                    {
                        ctx.AddFailure(new ValidationFailure("MenuName", "该名称已存在"));
                    }
                }
                if (!string.IsNullOrEmpty(model.MenuCode))
                {
                    if (service.Exists(a => a.MenuCode == model.MenuCode && a.MenuID != model.MenuID))
                    {
                        ctx.AddFailure(new ValidationFailure("MenuCode", "该菜单编码已存在"));
                    }
                }
                if (model.MenuType == 1)
                {
                    if (string.IsNullOrEmpty(model.MenuUrl))
                    {
                        ctx.AddFailure(new ValidationFailure("MenuUrl", "该菜单菜单地址不能为空"));
                    }
                }
                if (model.ParentID > 0)
                {
                    if (service.Exists(a => a.MenuID == model.ParentID&&a.MenuType==1))
                    {
                        ctx.AddFailure(new ValidationFailure("ParentID", "该菜单无法添加子菜单"));
                    }
                }
            });
            //RuleFor(model => model).CustomAsync(async (model, ctx, cancel) =>
            //{
            //    if (!string.IsNullOrEmpty(model.MenuName))
            //    {
            //        if (await service.ExistsAsync(a => a.MenuName == model.MenuName && a.MenuID != model.MenuID && a.ParentID == model.ParentID))
            //        {
            //            ctx.AddFailure(new ValidationFailure("MenuName", "该名称已存在"));
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(model.MenuCode))
            //    {
            //        if (await service.ExistsAsync(a => a.MenuCode == model.MenuCode && a.MenuID != model.MenuID))
            //        {
            //            ctx.AddFailure(new ValidationFailure("MenuCode", "该菜单编码已存在"));
            //        }
            //    }
            //    if (model.MenuType == 1)
            //    {
            //        if (string.IsNullOrEmpty(model.MenuUrl))
            //        {
            //            ctx.AddFailure(new ValidationFailure("MenuUrl", "该菜单菜单地址不能为空"));
            //        }
            //    }
            //    if (model.ParentID > 0)
            //    {
            //        if (await service.ExistsAsync(a => a.MenuID == model.ParentID && a.MenuType == 1))
            //        {
            //            ctx.AddFailure(new ValidationFailure("ParentID", "该菜单无法添加子菜单"));
            //        }
            //    }
            //});
        }
    }

}
