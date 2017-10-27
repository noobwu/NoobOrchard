using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Orchard.OAuth2.OwinOAuth.Provider
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
        ///// <summary>
        ///// The content-type used on HTTP POST requests where the POST entity is a
        ///// URL-encoded series of key=value pairs.
        ///// </summary>
        //protected internal const string HttpFormUrlEncoded = "application/x-www-form-urlencoded";

        ///// <summary>
        ///// The content-type used for JSON serialized objects.
        ///// </summary>
        //protected internal const string JsonEncoded = "application/json";

        ///// <summary>
        ///// The "text/javascript" content-type that some servers return instead of the standard <see cref="JsonEncoded"/> one.
        ///// </summary>
        //protected internal const string JsonTextEncoded = "text/javascript";
        ///// <summary>
        ///// The content-type for plain text.
        ///// </summary>
        //protected internal const string PlainTextEncoded = "text/plain";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task RequestToken(OAuthRequestTokenContext context)
        {
            logger.Debug("RequestToken");
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
                await Task.Factory.StartNew(() => {
                    context.Token = accessToken;
                });
            }
        }
    }
}