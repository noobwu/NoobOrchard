using System;
using FluentValidation;
using FluentValidation.Attributes;
using Orchard.Web.Mvc.Infrastructure;

namespace Orchard.Web.Mvc.Validators
{
    /// <summary>
    /// 验证
    /// </summary>
    public class NopValidatorFactory : AttributedValidatorFactory
    {

        //private readonly InstanceCache _cache = new InstanceCache();
        /// <summary>
        /// 获取验证类实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        //[Obsolete("由于会多次调用该方法,暂停使用")]
        //public override IValidator GetValidator(Type type)
        //{
        //    if (type != null)
        //    {
        //        var attribute = (ValidatorAttribute)Attribute.GetCustomAttribute(type, typeof(ValidatorAttribute));
        //        if ((attribute != null) && (attribute.ValidatorType != null))
        //        {
        //            //validators can depend on some customer specific settings (such as working language)
        //            //that's why we do not cache validators
        //            var instance = ContainerContext.Current.ResolveUnregistered(attribute.ValidatorType);
        //            return instance as IValidator;
        //        }
        //        return Activator.CreateInstance(type) as IValidator;
        //    }
        //    return null;
        //}
    }
}