using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Net;
using System.Text;

namespace Orchard.WebApi.OAuth2.OwinOAuth
{
    public class OAuthAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);
            var unauthorizeMessage = new HttpResponseMessage {
                StatusCode = HttpStatusCode.Unauthorized,
                Content =new StringContent("{}", Encoding.GetEncoding("UTF-8"), "application/json") 
            };
            unauthorizeMessage.Headers.Add("Authenticate", "basic");
            throw new HttpResponseException(unauthorizeMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private bool IsResourceOwner(string userName, System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var routeData = actionContext.Request.GetRouteData();
            var resourceUserName = routeData.Values["userName"] as string;
            if (resourceUserName == userName)
            {
                return true;
            }
            return false;
        }

    }
}