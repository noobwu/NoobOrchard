using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Orchard.WebApi.OAuth2.OwinOAuth.Provider
{
    /// <summary>
    /// The object provided by the application to process events raised by the bearer
    /// authentication middleware. The application may implement the interface fully,
    /// or it may create an instance of OAuthBearerAuthenticationProvider and assign
    /// delegates only to the events it wants to process.
    /// </summary>
    public class OAuthBearerProvider : OAuthBearerAuthenticationProvider
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task RequestToken(OAuthRequestTokenContext context)
        {
            await Task.Factory.StartNew(() => {
                logger.Debug("RequestToken");
            });
            IHeaderDictionary headers = context.Request.Headers;
            if (headers != null && headers.Count > 0)
            {
                //如果是Authorization=Bearer {access_token} 不做处理
                if (headers.ContainsKey("Authorization")|| headers.ContainsKey("authorization"))
                {
                    return;
                }
            }
            //?access_token={access_token}
            var accessToken = context.Request.Query.Get("access_token");
            //if (string.IsNullOrEmpty(accessToken))
            //{
            //    //form类型的参数
            //    IFormCollection formColls = await context.Request.ReadFormAsync();
            //    if (formColls != null || formColls.Count() > 0)
            //    {
            //        accessToken = formColls["access_token"];
            //    }
            //}
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
        }
    }
}