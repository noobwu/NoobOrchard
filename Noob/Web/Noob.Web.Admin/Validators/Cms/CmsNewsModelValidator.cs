using FluentValidation;
using FluentValidation.Results;
using Noob.IServices;
using Noob.Web.Admin.Models;
using Orchard.Web.Mvc.Infrastructure;
using Orchard.Web.Mvc.Validators;

namespace Noob.Web.Admin.Validators
{

    /// <summary>
    /// 资讯验证
    /// </summary>
    public class CmsNewsModelValidator:BaseNopValidator<CmsNewsModel>
    {
        public CmsNewsModelValidator()
        {
            RuleFor(a => a.Title).NotEmpty().WithMessage("标题不能为空");
            RuleFor(a => a.ImageUrl).NotEmpty().WithMessage("图片地址不能为空");
            RuleFor(a => a.NaviContent).NotEmpty().WithMessage("导读内容不能为空");
            RuleFor(a => a.ContentSource).NotEmpty().WithMessage("内容来源不能为空");
            RuleFor(a => a.Author).NotEmpty().WithMessage("作者不能为空");
            RuleFor(a => a.Tag).NotEmpty().WithMessage("标签不能为空");
            RuleFor(a => a.NewsContent).NotEmpty().WithMessage("新闻内容不能为空");
            ICmsNewsService service = ContainerContext.Current.Resolve<ICmsNewsService>();
            RuleFor(model => model).Custom((model, ctx) =>
            {
                    if (!string.IsNullOrEmpty(model.Title) && !string.IsNullOrEmpty(model.ImageUrl)
                        && !string.IsNullOrEmpty(model.NaviContent) && !string.IsNullOrEmpty(model.ContentSource)
                         && !string.IsNullOrEmpty(model.Author) && !string.IsNullOrEmpty(model.Tag)
                         && !string.IsNullOrEmpty(model.NewsContent))
                    {
                        if (service.Exists(a => a.Title == model.Title && a.NewsId != model.NewsId))
                        {
                            ctx.AddFailure(new ValidationFailure("Title", "资讯标题已存在"));
                        }
                    }
                });
        }
    }	
   
}
