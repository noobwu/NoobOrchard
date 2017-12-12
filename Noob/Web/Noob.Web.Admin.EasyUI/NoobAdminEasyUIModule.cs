using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using FluentValidation.Mvc;
using Orchard.Caching;
using Orchard.Logging;
using Orchard.Web.Mvc.Validators;


namespace Noob.Web.Admin.EasyUI
{
    /// <summary>
    /// 
    /// </summary>
    public class NoobAdminEasyUIModule : Autofac.Module
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void Load(ContainerBuilder builder)
        {
            Init(builder);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        private void Init(ContainerBuilder builder)
        {
            //controllers
            //var controllerAssembly = Assembly.GetAssembly(typeof(Noob.Web.Admin.EasyUI.Controllers.BaseAdminController));
            var controllerAssembly = Assembly.GetExecutingAssembly();
            builder.RegisterControllers(controllerAssembly);

            //注册api容器的实现
            //builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);
            //builder.RegisterApiControllers(controllerAssembly);
            //builder.RegisterAssemblyTypes(controllerAssembly).AsImplementedInterfaces();
            //fluent validation
            //DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            //DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = true;
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new NopValidatorFactory()));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentRegistry"></param>
        /// <param name="registration"></param>
        protected override void AttachToComponentRegistration(Autofac.Core.IComponentRegistry componentRegistry, Autofac.Core.IComponentRegistration registration)
        {

        }
    }
}