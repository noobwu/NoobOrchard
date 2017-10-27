using FluentValidation;

namespace Orchard.Web.Mvc.Validators
{

    /// <summary>
    /// 验证类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public  class BaseNopValidator<T> : AbstractValidator<T> where T : class
    {
        /// <summary>
        /// 验证类构造方法 
        /// </summary>
        protected BaseNopValidator()
        {
            PostInitialize();
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
            
        }
    }
}