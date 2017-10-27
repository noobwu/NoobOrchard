using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Orchard.WebApi.OAuth2.OwinOAuth.Provider
{
    /// <summary>
    ///  Receives the bearer token the client application will be providing to web application.
    ///  If not provided the token produced on the server's default data protection by
    ///  using the AccessTokenFormat. If a different access token provider or format is
    ///  assigned, a compatible instance must be assigned to the OAuthAuthorizationServerOptions.AccessTokenProvider
    ///  and OAuthAuthorizationServerOptions.AccessTokenFormat of the authorization server.
    /// </summary>
    public class AccessTokenProvider : IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, string> accessTokenTokens = new ConcurrentDictionary<string, string>();
        private NLog.ILogger logger = null;
        public AccessTokenProvider()
        {
            logger = NLog.LogManager.GetCurrentClassLogger();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Create(AuthenticationTokenCreateContext context)
        {
            logger.Debug("Create");
           

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            logger.Debug("CreateAsync");
            await Task.Factory.StartNew(() =>
            {
                context.SetToken(context.SerializeTicket());
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Receive(AuthenticationTokenReceiveContext context)
        {
            logger.Debug("Receive");

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            logger.Debug("ReceiveAsync,Token:"+context.Token);
            if (string.IsNullOrEmpty(context.Token))
            {
                return;
            }
            await Task.Factory.StartNew(() =>
            {
                context.DeserializeTicket(context.Token);
                context.OwinContext.Environment["Properties"] = context.Ticket.Properties;
            });
        }
    }
}