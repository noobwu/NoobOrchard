using System.Web.Mvc;

namespace Orchard.Web.Mvc.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class NopModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            try
            {
                var model = base.BindModel(controllerContext, bindingContext);
                if (model is BaseNopModel)
                {
                    ((BaseNopModel)model).BindModel(controllerContext, bindingContext);
                }
                return model;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }
    }
}
