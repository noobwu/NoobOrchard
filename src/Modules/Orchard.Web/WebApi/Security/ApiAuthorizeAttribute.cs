using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Security;

namespace Orchard.Web.WebApi.Security
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ApiAuthorizeAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                //如果action或controler 允许匿名用户跳出
                if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0
                    ||actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0)   // 允许匿名访问
                {
                    return;
                }
                var cookie = actionContext.Request.Headers.GetCookies();
                if (cookie == null || cookie.Count < 1)
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    return;
                }
                FormsAuthenticationTicket ticket = null;
                foreach (var perCookie in cookie[0].Cookies)
                {
                    if (perCookie.Name == FormsAuthentication.FormsCookieName)
                    {
                        ticket = FormsAuthentication.Decrypt(perCookie.Value);
                        break;
                    }
                }
                if (ticket == null)
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    return;
                }
                // TODO: 添加其它验证方法
                base.OnActionExecuting(actionContext);
            }
            catch
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            }
        }
    }
}