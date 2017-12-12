using Orchard.Security;
using Orchard.Web.Mvc.Security;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Noob.Web.Admin.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AdminAuthorizeAttribute : RBACAuthorizeAttribute
    {
        // 摘要: 
        //     初始化 System.Web.Mvc.AuthorizeAttribute 类的新实例。
        public AdminAuthorizeAttribute()
            : base()
        {
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
            //var user = filterContext.HttpContext.Session["CurrentUser"] as User;
            var controller = filterContext.RouteData.Values["controller"];
            var action = filterContext.RouteData.Values["action"];
            // logger.Debug("controller:" + controller + ",action:" + action + ",RawUrl:" + filterContext.HttpContext.Request.RawUrl);

            if (filterContext.HttpContext.User == null || filterContext.HttpContext.User.Identity == null
                || string.IsNullOrEmpty(filterContext.HttpContext.User.Identity.Name)
                || !filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                byte[] url = System.Text.Encoding.UTF8.GetBytes(filterContext.HttpContext.Request.RawUrl);
                filterContext.Result = new RedirectResult("~/Account/Login?Url=" + filterContext.HttpContext.Server.UrlTokenEncode(url));
                return;
            }
            //没有权限进行跳转
            if (filterContext.Result is HttpStatusCodeResult)
            {
                HttpStatusCodeResult statusCodeResult = filterContext.Result as HttpStatusCodeResult;
                if (statusCodeResult.StatusCode == 403)
                {
                    filterContext.Result = new RedirectResult("/Home/NoAuthorize");
                }
            }

          
        }
        // Methods
        protected override PermissionAuthorize GetPermissionAuthorize(HttpContextBase httpContext)
        {
            return new AdminPermissionAuthorize(httpContext);
        }
    }
}
