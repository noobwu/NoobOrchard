using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Infrastructure;
using Orchard.WebApi.OAuth2.OwinOAuth.Models;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Orchard.WebApi.OAuth2.OwinOAuth.Provider
{
    /// <summary>
    /// Produces a refresh token which may be used to produce a new access token when
    /// needed. If not provided the authorization server will not return refresh tokens
    /// from the /Token endpoint.
    /// </summary>
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, string> refreshTokens = new ConcurrentDictionary<string, string>();
        private NLog.ILogger logger = null;
        public RefreshTokenProvider()
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
            logger.Debug("CreateAsync");
            if (context.Ticket.Properties != null)
            {
                logger.Debug("CreateAsync,context.Ticket.Properties:" + JsonConvert.SerializeObject(context.Ticket.Properties));
            }
            var clientId = context.OwinContext.Get<string>(OAuthKeys.ClientId);
            if (!string.IsNullOrEmpty(clientId))
            {
                OauClient client = Clients.ApiClients.FirstOrDefault(a => a.AppId == clientId);
                if (client != null)
                {
                    var refreshTokenModel = GetRefreshToken(context, clientId, client);
                    //调用RefreshToken服务保存Token
                    var insertResult = await HandleCreateToken(refreshTokenModel);
                    if (insertResult > 0)
                    {
                        #region  Setting Token
                        // Gets or sets the time at which the authentication ticket was issued.
                        context.Ticket.Properties.IssuedUtc = DateTime.UtcNow;
                        // Gets or sets the time at which the authentication ticket expires.
                        context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddMinutes(client.RefreshTokenLifeTime);
                        refreshTokens[refreshTokenModel.RefreshToken] = refreshTokenModel.ProtectedTicket;
                        context.Ticket.Properties.Dictionary.Add(OAuthKeys.RefreshToken, refreshTokenModel.RefreshToken);
                        //设置RefreshToken的key
                        context.SetToken(refreshTokenModel.RefreshToken);

                        if (refreshTokens != null)
                        {
                            logger.Debug("CreateAsync,HandleCreateToken,refreshTokens:" + JsonConvert.SerializeObject(refreshTokens));
                        }
                        #endregion  Setting Token
                    }

                }
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="clientId"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private OauRefreshToken GetRefreshToken(AuthenticationTokenCreateContext context, string clientId, OauClient client)
        {
            var identity = context.Ticket.Identity;
            if (identity == null) return null;
            int userId = 0;
            string userName = identity.Name;
            string customTokenType = string.Empty;
            //var userExtInfo = context.OwinContext.Get<string>("as:userExtInfo");
            //string deviceId = context.OwinContext.Get<string>("as:deviceId");
            //string customTokenType = context.OwinContext.Get<string>("as:customTokenType");
            //if (string.IsNullOrEmpty(customTokenType))
            //{
            //    customTokenType = identity.AuthenticationType;
            //}
            //if (userId < 1 && identity.Claims != null)
            //{
            //    var tmpUserId = identity.Claims.FirstOrDefault(a => a.Type == ClaimTypes.PrimarySid);
            //    if (tmpUserId != null)
            //    {
            //        userId = Convert.ToInt32(tmpUserId.Value);
            //    }
            //}
            //if (string.IsNullOrEmpty(userExtInfo) && identity.Claims != null)
            //{
            //    var tmpUserData = identity.Claims.FirstOrDefault(a => a.Type == ClaimTypes.UserData);
            //    if (tmpUserData != null)
            //    {
            //        userExtInfo = tmpUserData.Value;
            //    }
            //}
            var refreshTokenId = Guid.NewGuid().ToString("n");
            var refreshToken = new OauRefreshToken()
            {
                AppId = clientId,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(client.RefreshTokenLifeTime),
                IssuedUtc = DateTime.UtcNow,
                ProtectedTicket = context.SerializeTicket(),
                RefreshToken = refreshTokenId,
                TokenType = identity.AuthenticationType,
                UserName = userName,
                UserID = userId,
                CustomTokenType = customTokenType,
                AccessToken = string.Empty
            };
            return refreshToken;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        private Task<int> HandleCreateToken(OauRefreshToken refreshToken)
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
            if (refreshTokens != null)
            {
                logger.Debug("Receive,HandleReceiveToken,refreshTokens:" + JsonConvert.SerializeObject(refreshTokens));
            }
            string value;
            if (refreshTokens.TryRemove(context.Token, out value))
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