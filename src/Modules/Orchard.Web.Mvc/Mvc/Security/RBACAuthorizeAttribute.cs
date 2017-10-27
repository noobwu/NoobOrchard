using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Security;

namespace Orchard.Web.Mvc.Security
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public abstract class RBACAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        protected ILogger logger;
        // 摘要: 
        //     初始化 System.Web.Mvc.AuthorizeAttribute 类的新实例。
        /// <summary>
        /// 
        /// </summary>
        public RBACAuthorizeAttribute()
        {
            logger = NullLogger.Instance;
        }
        // Fields
        private string _permission = string.Empty;


        // Properties
        /// <summary>
        /// 
        /// </summary>
        public string Permission
        {
            get { return _permission ?? String.Empty; }
            set { _permission = value; }
        }

        /// <summary>
        /// 处理未能授权的 HTTP 请求。
        /// </summary>
        /// <param name="filterContext">  封装有关使用 System.Web.Mvc.AuthorizeAttribute 的信息。filterContext 对象包括控制器、HTTP 上下文、请求上下文、操作结果和路由数据。</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            #region Contracts

            if (filterContext == null) throw new ArgumentNullException();

            #endregion

            // HttpStatus - 403 Forbidden
            if (filterContext.HttpContext.User.Identity.IsAuthenticated == true)
            {
                filterContext.Result = new HttpStatusCodeResult(403);
                return;
            }

            // Base
            base.HandleUnauthorizedRequest(filterContext);
        }

        /// <summary>
        ///  提供一个入口点用于进行自定义授权检查。
        /// </summary>
        /// <param name="httpContext">  HTTP 上下文，它封装有关单个 HTTP 请求的所有 HTTP 特定的信息。</param>
        /// <returns> 如果用户已经过授权，则为 true；否则为 false。</returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            #region Contracts

            if (httpContext == null) throw new ArgumentNullException();

            #endregion

            // Base
            //if (base.AuthorizeCore(httpContext) == false) return false;

            // PermissionAuthorize
            var permissionAuthorize = this.GetPermissionAuthorize(httpContext);
            if (permissionAuthorize == null) return true;

            // Authorize
            return permissionAuthorize.Authorize(httpContext.User, this.Permission);
        }

        /// <summary>
        ///  在过程请求授权时调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文，它封装有关使用 System.Web.Mvc.AuthorizeAttribute 的信息。</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //如果action或controler 允许匿名用户跳出
            if (CheckAllowAnonymous(filterContext.ActionDescriptor))
            {
                return;
            }
            base.OnAuthorization(filterContext);
        }

        /// <summary>
        /// 判断是否允许匿名访问
        /// </summary>
        /// <param name="actionDescriptor"></param>
        /// <returns></returns>
        protected bool CheckAllowAnonymous(ActionDescriptor actionDescriptor)
        {
            return (
                actionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
             || actionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true));
            
        }

        /// <summary>
        ///  在缓存模块请求授权时调用。
        /// </summary>
        /// <param name="httpContext">  HTTP 上下文，它封装有关单个 HTTP 请求的所有 HTTP 特定的信息。</param>
        /// <returns> 对验证状态的引用。</returns>
        protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            return base.OnCacheAuthorization(httpContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected abstract PermissionAuthorize GetPermissionAuthorize(HttpContextBase httpContext);
    }
}
