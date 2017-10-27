
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin;
using Orchard.WebApi.OAuth2.OwinOAuth.Models;
using Newtonsoft.Json;
using System.Security.Principal;
using Microsoft.Owin.Security.Infrastructure;

namespace Orchard.WebApi.OAuth2.OwinOAuth.Provider
{
    /// <summary>
    /// Default implementation of IOAuthAuthorizationServerProvider used by Authorization
    /// Server to communicate with the web application while processing requests. OAuthAuthorizationServerProvider
    /// provides some default behavior, may be used as a virtual base class, and offers
    /// delegate properties which may be used to handle individual calls without declaring a new class type.
    /// </summary>
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private NLog.ILogger logger = null;
        /// <summary>
        /// 
        /// </summary>
        public AuthorizationServerProvider()
        {
            logger = NLog.LogManager.GetCurrentClassLogger();
        }
        /// <summary>
        /// 验证clientId与clientSecret
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            logger.Debug("ValidateClientAuthentication");
            string clientId;
            string clientSecret;
            int code = 0;
            string error = "invalid_client";
            string errorDescription = "";
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                errorDescription = "参数有误";
            }
            else
            {
                //获取附加的请求参数
                IReadableStringCollection parameters = context.Parameters;
                OauClient client = Clients.ApiClients.FirstOrDefault(a => a.AppId == clientId && a.AppSecret == clientSecret);
                if (client == null)
                {
                    errorDescription = "无效clientId或者无效的clientSecret";
                }
                else
                {
                    code = 1;
                    if (parameters != null)
                    {
                        //在OWIN环境中添加附加的请求参数
                        foreach (var item in parameters)
                        {
                            context.OwinContext.Set(OAuthKeys.Prefix + item.Key, string.Join(",", item.Value));
                        }
                        logger.Debug("ValidateClientAuthentication,context.Parameters:" + JsonConvert.SerializeObject(parameters));
                    }
                }
            }
            if (code == 1)
            {
                //设置客户机id并标记应用程序验证的上下文
                bool validateResult = context.Validated(clientId);
                logger.Debug("ValidateClientAuthentication，validateResult:" + validateResult);
                if (!validateResult)
                {
                    context.SetError(error, "validate 失败");
                }
            }
            else
            {
                //设置错误的响应信息
                context.SetError(error, errorDescription);
            }
            await base.ValidateClientAuthentication(context);
        }

        /// <summary>
        /// 当grant_type=client_credentials触发该事件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            logger.Debug("GrantClientCredentials");
            string error = "invalid_grant";
            if (string.IsNullOrEmpty(context.ClientId))
            {
                context.SetError(error, "无效clientId");
            }
            else
            {
                OauClient client = Clients.ApiClients.FirstOrDefault(a => a.AppId == context.ClientId);
                if (client != null)
                {
                    //获取附加的请求参数
                    IDictionary<string, object> parameters = context.OwinContext.Environment;

                    int tokenLifeTime = client.RefreshTokenLifeTime;
                    var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                    if (parameters != null)
                    {
                        //foreach (var item in parameters)
                        //{
                        //    //添加附加凭证信息
                        //   oAuthIdentity.AddClaim(new Claim(OAuthKeys.Prefix+item.Key,item.Value.ToString()));
                        //}
                    }
                    bool validateResult = false;
                    var oauthProps = new AuthenticationProperties(new Dictionary<string, string>
                    {
                        {
                           OAuthKeys.AuthPropClientId, context.ClientId
                        }
                    })
                    {
                        // Gets or sets the time at which the authentication ticket was issued.
                        IssuedUtc = DateTime.UtcNow,
                        // Gets or sets the time at which the authentication ticket expires.
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(client.RefreshTokenLifeTime)
                    };
                    //替换此上下文中的票据信息，并将其标记为已验证
                    var ticket = new AuthenticationTicket(oAuthIdentity, oauthProps);
                    validateResult = context.Validated(ticket);
                    if (!validateResult)
                    {
                        context.SetError(error, "validate失败");
                    }
                }
                else
                {
                    context.SetError(error, "无效clientId");
                }
            }
            await base.GrantClientCredentials(context);
        }

        /// <summary>
        ///  当grant_type=password 触发该事件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(
            OAuthGrantResourceOwnerCredentialsContext context)
        {
            logger.Debug("GrantResourceOwnerCredentials");
            string error = "invalid_grant";
            OauClient client = Clients.ApiClients.FirstOrDefault(a => a.AppId == context.ClientId);
            if (client == null)
            {
                context.SetError(error, "无效clientId");
                return;
            }
            if (string.IsNullOrEmpty(context.UserName) || string.IsNullOrEmpty(context.Password))
            {
                context.SetError(error, "用户名或密码不能为空");
                return;
            }
            //调用后台的登录服务验证用户名与密码
            if (!CheckUser(context.UserName, context.Password))
            {
                context.SetError(error, "用户名或密码不能为空");
                return;
            }
            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            //获取附加的请求参数
            IDictionary<string, object> parameters = context.OwinContext.Environment;
            if (parameters != null)
            {
                //foreach (var item in parameters)
                //{
                //    //添加附加凭证信息
                //    oAuthIdentity.AddClaim(new Claim(OAuthKeys.Prefix+item.Key,item.Value.ToString()));
                //}
            }
            var oauthProps = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                       OAuthKeys.AuthPropClientId, context.ClientId
                    }
                })
            {
                // Gets or sets the time at which the authentication ticket was issued.
                IssuedUtc = DateTime.UtcNow,
                // Gets or sets the time at which the authentication ticket expires.
                ExpiresUtc = DateTime.UtcNow.AddMinutes(client.RefreshTokenLifeTime)
            };
            var ticket = new AuthenticationTicket(oAuthIdentity, oauthProps);
            bool validateResult = context.Validated(ticket);
            if (!validateResult)
            {
                context.SetError(error, "validate 失败");
            }
            await base.GrantResourceOwnerCredentials(context);
        }
        private bool CheckUser(string userName, string Password)
        {
            return true;
        }

        /// <summary>
        ///  Called when a request to the Token endpoint arrives with a "grant_type" of "refresh_token"
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            logger.Debug("GrantRefreshToken");
            if (context.Ticket.Properties != null)
            {
                logger.Debug("GrantResourceOwnerCredentials,context.Ticket.Properties:" + JsonConvert.SerializeObject(context.Ticket.Properties));
            }
            string error = "invalid_grant";
            var originalClientId = context.OwinContext.Get<string>(OAuthKeys.ClientId);
            var currentClientId = context.ClientId;
            if (!string.IsNullOrEmpty(originalClientId) && !string.IsNullOrEmpty(currentClientId))
            {
                if (originalClientId != currentClientId)
                {
                    context.SetError(error, "Refresh token is issued to a different clientId.");

                }
                else
                {
                    // Change auth ticket for refresh token requests
                    var newOAuthIdentity = new ClaimsIdentity(context.Ticket.Identity);

                    //获取附加的请求参数
                    IDictionary<string, object> parameters = context.OwinContext.Environment;
                    if (parameters != null)
                    {
                        //foreach (var item in parameters)
                        //{
                        //    //添加附加凭证信息
                        //    newOAuthIdentity.AddClaim(new Claim(OAuthKeys.Prefix + item.Key, item.Value.ToString()));
                        //}
                    }
                    newOAuthIdentity.AddClaim(new Claim(OAuthKeys.RefreshTokenClaim, "refreshToken"));
                    var newTicket = new AuthenticationTicket(newOAuthIdentity, context.Ticket.Properties);
                    bool validateResult = context.Validated(newTicket);
                    if (!validateResult)
                    {
                        context.SetError(error, "validate 失败");
                    }
                }
            }
            await base.GrantRefreshToken(context);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            logger.Debug("TokenEndpointResponse");
            //获取附加的请求参数
            IDictionary<string, object> parameters = context.OwinContext.Environment;
            if (context.Properties.Dictionary != null)
            {
                logger.Debug("TokenEndpointResponse,context.Properties.Dictionary:" + JsonConvert.SerializeObject(context.Properties.Dictionary));
            }
            if (CheckRefreshToken(context))
            {
                //context.AccessToken
                //调用后台的Token服务更新Tokenas:client_id
            }
            return base.TokenEndpointResponse(context);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool CheckRefreshToken(OAuthTokenEndpointResponseContext context)
        {
            return (context.Properties != null && context.Properties.Dictionary != null
              && context.Properties.Dictionary.Count > 0
              && context.Properties.Dictionary.ContainsKey(OAuthKeys.RefreshToken));
        }

        /// <summary>
        /// 生成 authorization_code（authorization code 授权方式）、生成 access_token （implicit 授权模式）
        /// </summary>
        public override async Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
        {
            string redirectUri = string.Empty, clientId = string.Empty, clientSecret = string.Empty;
            IFormCollection formColls = await context.Request.ReadFormAsync();
            //form类型的参数
            if (formColls != null && formColls.Count() > 0)
            {
                redirectUri = formColls["redirect_uri"];
                clientId = formColls["client_id"];
                clientSecret = formColls["client_secret"];
            }
            else
            {
                //url类型的参数
                redirectUri = context.Request.Query["redirect_uri"];
                clientId = context.Request.Query["client_id"];
                clientSecret = context.Request.Query["client_secret"];
            }
            OauClient client = Clients.ApiClients.FirstOrDefault(a => a.AppId == clientId && a.AppSecret == clientSecret);
            if (context.AuthorizeRequest.IsImplicitGrantType)
            {
                logger.Debug("AuthorizeEndpoint,implicit");
                //implicit 授权方式
                HandleImplicitGrant(context,client,redirectUri);
            }
            else if (context.AuthorizeRequest.IsAuthorizationCodeGrantType)
            {
                //authorization code 授权方式
                if (client != null)
                {
                    await HandleAuthorizationCodeGrant(context,client,redirectUri);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private  void HandleImplicitGrant(OAuthAuthorizeEndpointContext context,OauClient client, string redirectUri)
        {
            var oAuthIdentity = new ClaimsIdentity("Bearer");
            context.OwinContext.Authentication.SignIn(oAuthIdentity);
            context.RequestCompleted();
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="client"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        private async Task HandleAuthorizationCodeGrant(OAuthAuthorizeEndpointContext context
            ,OauClient client,string redirectUri)
        {
            #region Settin Token
            context.OwinContext.Set(OAuthKeys.ClientId, client.AppId);
            context.OwinContext.Set(OAuthKeys.Prefix + "client_secret", client.AppSecret);

            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, Guid.NewGuid().ToString("n")));
            var oauthProps = new AuthenticationProperties(new Dictionary<string, string>
                    {
                        {
                            OAuthKeys.AuthPropClientId,client.AppId
                        }
                    })
            {
                // Gets or sets the time at which the authentication ticket was issued.
                IssuedUtc = DateTime.UtcNow,
                // Gets or sets the time at which the authentication ticket expires.
                ExpiresUtc = DateTime.UtcNow.AddMinutes(client.RefreshTokenLifeTime),
                //  Gets or sets the full path or absolute URI to be used as an http redirect response
                RedirectUri = redirectUri
            };
            //替换此上下文中的票据信息，并将其标记为已验证
            var ticket = new AuthenticationTicket(oAuthIdentity, oauthProps);
            var authorizeCodeContext = new AuthenticationTokenCreateContext(
                context.OwinContext,
                context.Options.AuthorizationCodeFormat,
               ticket);

            await context.Options.AuthorizationCodeProvider.CreateAsync(authorizeCodeContext);
            #endregion Setting Token

            string token = authorizeCodeContext.Token ?? string.Empty;//由于获取的token与设置的token不一致
                                                                      //string token = authorizeCodeContext.OwinContext.Get<string>(OAuthKeys.AuthorizeCodeToken) ?? string.Empty;
            var url = redirectUri + "?code=" + Uri.EscapeDataString(token);
            logger.Debug("AuthorizeEndpoint,authorization_code,url:" + url);
            context.Response.Redirect(url);
            context.RequestCompleted();
        }

        /// <summary>
        /// 验证 authorization_code 的请求
        /// </summary>
        public override async Task ValidateAuthorizeRequest(OAuthValidateAuthorizeRequestContext context)
        {
            logger.Debug("ValidateAuthorizeRequest");
            var client = Clients.ApiClients.Where(a => a.AppId == context.AuthorizeRequest.ClientId);
            if (client != null &&
                (context.AuthorizeRequest.IsAuthorizationCodeGrantType
                || context.AuthorizeRequest.IsImplicitGrantType))
            {
                /*
                  Marks this context as validated by the application. IsValidated becomes true
                   and HasError becomes false as a result of calling.
                 */
                var validateResult = await Task.FromResult(context.Validated());
            }
            else
            {
                /* Marks this context as not validated by the application. IsValidated and HasError
                 become false as a result of calling.
                 */
                context.Rejected();
            }
        }

        /// <summary>
        /// 验证 redirect_uri
        /// </summary>
        public override async Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            string redirectUri = context.RedirectUri;
            logger.Debug("ValidateClientRedirectUri,redirectUri:" + redirectUri);
            /*
            Checks the redirect URI to determine whether it equals Microsoft.Owin.Security.OAuth.OAuthValidateClientRedirectUriContext.RedirectUri.
            */
            var validateResult = await Task.FromResult(context.Validated(redirectUri));

        }

        /// <summary>
        /// 验证 access_token 的请求
        /// </summary>
        public override async Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            logger.Debug("ValidateTokenRequest");

            if (
                context.TokenRequest.IsAuthorizationCodeGrantType
                || context.TokenRequest.IsClientCredentialsGrantType
                  || context.TokenRequest.IsRefreshTokenGrantType
                  || context.TokenRequest.IsResourceOwnerPasswordCredentialsGrantType
                  )
            {
                /*
                  Marks this context as validated by the application. IsValidated becomes true
                   and HasError becomes false as a result of calling.
                 */
                var validateResult = await Task.FromResult(context.Validated());
            }
            else
            {
                context.Rejected();
            }
        }
    }

}