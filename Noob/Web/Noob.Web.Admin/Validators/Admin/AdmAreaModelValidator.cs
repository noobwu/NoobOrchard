using FluentValidation;
using FluentValidation.Results;
using Noob.Web.Admin.Models;
using Orchard.Web.Mvc.Validators;
using System.Threading.Tasks;
using Noob.IServices;
using Orchard.Web.Mvc.Infrastructure;

namespace Noob.Web.Admin.Validators
{

    /// <summary>
    /// 地区验证
    /// </summary>
    public class AdmAreaModelValidator : BaseNopValidator<AdmAreaModel>
    {
        public AdmAreaModelValidator()
        {
            RuleFor(a => a.AreaID).NotEmpty().WithMessage("地区ID不能为空");
            RuleFor(a => a.AreaName).NotEmpty().WithMessage("地区名称不能为空");
            RuleFor(a => a.ParentId).NotEmpty().WithMessage("父级ID不能为空");
            RuleFor(a => a.ShortName).NotEmpty().WithMessage("地区简称不能为空");
            //RuleFor(a => a.AreaNamePath).NotEmpty().WithMessage("完整地区名称不能为空");
            // RuleFor(a => a.AreaIDPath).NotEmpty().WithMessage("完整地区名称不能为空");
            IAdmAreaService service = ContainerContext.Current.Resolve<IAdmAreaService>();
            RuleFor(model => model).Custom((model, ctx) =>
           {
               if (!string.IsNullOrEmpty(model.AreaName))
               {
                   if (service.Exists(a => a.AreaName == model.AreaName && a.AreaID != model.AreaID))
                   {
                       ctx.AddFailure(new ValidationFailure("AreaName", "地区名称已存在"));
                       return;
                   }
               }
               if (!string.IsNullOrEmpty(model.AreaID))
               {
                   if (service.Exists(a => a.AreaID == model.AreaID && a.Id != model.Id))
                   {
                       ctx.AddFailure(new ValidationFailure("AreaID", "地区ID已存在"));
                       return;
                   }
               }
           });
            //model会出错 暂时不用
            //RuleFor(model => model).CustomAsync(async (model, ctx, cancel) =>
            //{
            //    if (!string.IsNullOrEmpty(model.AreaName))
            //    {
            //        if (await service.ExistsAsync(a => a.AreaName == model.AreaName && a.AreaID != model.AreaID))
            //        {
            //            ctx.AddFailure(new ValidationFailure("AreaName", "地区名称已存在"));
            //            return;
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(model.AreaID))
            //    {
            //        if (await service.ExistsAsync(a => a.AreaID == model.AreaID && a.Id != model.Id))
            //        {
            //            ctx.AddFailure(new ValidationFailure("AreaID", "地区ID已存在"));
            //            return;
            //        }
            //    }
            //});
        }
    }

}
