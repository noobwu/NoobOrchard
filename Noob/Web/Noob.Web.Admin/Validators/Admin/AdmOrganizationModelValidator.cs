using FluentValidation;
using FluentValidation.Results;
using Noob.Web.Admin.Models;
using Orchard.Web.Mvc.Validators;
using Noob.IServices;
using Orchard.Web.Mvc.Infrastructure;
namespace Noob.Web.Admin.Validators
{

    /// <summary>
    /// 组织机构验证
    /// </summary>
    public class AdmOrganizationModelValidator : BaseNopValidator<AdmOrganizationModel>
    {
        public AdmOrganizationModelValidator()
        {
            RuleFor(a => a.OrgName).NotEmpty().WithMessage("组织机构简称不能为空");
            RuleFor(a => a.OfficePhone).NotEmpty().WithMessage("办公电话不能为空").
                Must(ValidOfficePhone).WithMessage("办公电话不正确");
            RuleFor(a => a.Address).NotEmpty().WithMessage("部门地址不能为空");
            IAdmOrganizationService service = ContainerContext.Current.Resolve<IAdmOrganizationService>();
            RuleFor(model => model).Custom((model, ctx) =>
            {
                if (!string.IsNullOrEmpty(model.OrgName))
                {
                    if (service.Exists(a => a.OrgName == model.OrgName && a.OrgID != model.OrgID && a.ParentID == model.ParentID))
                    {
                        ctx.AddFailure(new ValidationFailure("OrgName", "该名称已存在"));
                    }
                }
                //if (model.OrgType >= 0 && model.OrgType <=2)
                //{
                //    return new ValidationFailure("GroupType", "无效用户组类型");
                //}
            });
            //RuleFor(model => model).CustomAsync(async (model, ctx, cancel) =>
            //{
            //    if (!string.IsNullOrEmpty(model.OrgName))
            //    {
            //        if (await service.ExistsAsync(a => a.OrgName == model.OrgName && a.OrgID != model.OrgID && a.ParentID == model.ParentID))
            //        {
            //            ctx.AddFailure(new ValidationFailure("OrgName", "该名称已存在"));
            //        }
            //    }
            //    //if (model.OrgType >= 0 && model.OrgType <=2)
            //    //{
            //    //    return new ValidationFailure("GroupType", "无效用户组类型");
            //    //}
            //});
        }
        private bool ValidOfficePhone(string officePhone)
        {
            // custom postcode validating logic goes here
            return true;// officePhone == "0755-12345678";
        }
    }

}
