using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Orchard.WebApi.OAuth2.OwinOAuth.Models;
using Newtonsoft.Json;

namespace Orchard.WebApi.OAuth2.OwinOAuth.Provider
{
    /// <summary>
    /// The period of time the access token remains valid after being issued. The default
    /// is twenty minutes. The client application is expected to refresh or acquire a
    /// new access token after the token has expired.
    /// </summary>
    public class AuthorizationCodeProvider : IAuthenticationTokenProvider
    {
        private readonly ConcurrentDictionary<string, string> authenticationCodes =
        new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
        private NLog.ILogger logger = null;
        public AuthorizationCodeProvider()
        {
            logger = NLog.LogManager.GetCurrentClassLogger();
        }
        #region Create
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Create(AuthenticationTokenCreateContext context)
        {
            logger.Debug("Create");
        }
        /// <summary>
        /// 生成 refresh_token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientId = context.OwinContext.Get<string>(OAuthKeys.ClientId);
            logger.Debug("CreateAsync,clientId:" + clientId);
            if (context.Ticket.Properties != null)
            {
                logger.Debug("CreateAsync,context.Ticket.Properties:" + JsonConvert.SerializeObject(context.Ticket.Properties));
            }
            if (!string.IsNullOrEmpty(clientId))
            {
                OauClient client = Clients.ApiClients.FirstOrDefault(a => a.AppId == clientId);
                if (client != null)
                {
                    //调用RefreshToken服务保存Token
                    var insertResult = await HandleCreateToken();
                    if (insertResult > 0)
                    {
                        #region Setting Token
                        // Gets or sets the time at which the authentication ticket was issued.
                        context.Ticket.Properties.IssuedUtc = DateTime.UtcNow;
                        // Gets or sets the time at which the authentication ticket expires.
                        context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddMinutes(client.RefreshTokenLifeTime);
                        var token = Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n");
                        context.SetToken(token);
                        authenticationCodes[token] = context.SerializeTicket();
                        //context.OwinContext.Set(OAuthKeys.AuthorizeCodeToken, token);
                        if (authenticationCodes != null)
                        {
                            logger.Debug("CreateAsync,HandleCreateToken,authenticationCodes:" + JsonConvert.SerializeObject(authenticationCodes));
                        }
                        #endregion Setting Token
                    }

                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Task<int> HandleCreateToken()
        {
            return Task.FromResult(new Random().Next(10000, 99999));
        }
        #endregion Create

        #region Receive
        public void Receive(AuthenticationTokenReceiveContext context)
        {
            logger.Debug("Receive");
            if (context.Ticket.Properties != null)
            {
                logger.Debug("Receive,context.Ticket.Properties:" + JsonConvert.SerializeObject(context.Ticket.Properties));
            }
        }

        /// <summary>
        /// 由 refresh_token 解析成 access_token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            //context.Token:refresh_token参数
            logger.Debug("ReceiveAsync");
            //var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

            if (!string.IsNullOrEmpty(context.Token))
            {
                await HandleReceiveToken(context);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task<int> HandleReceiveToken(AuthenticationTokenReceiveContext context)
        {
            if (authenticationCodes != null)
            {
                logger.Debug("Receive,HandleReceiveToken,authenticationCodes:" + JsonConvert.SerializeObject(authenticationCodes));
            }
            string value;
            if (authenticationCodes.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }
            //else
            //{
            //    context.DeserializeTicket(context.Token);
            //}
            return Task.FromResult(1);
        }
        #endregion Receive
    }
}