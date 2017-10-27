using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Orchard.WebApi.OAuth2.OwinOAuth.Models;
using Orchard.WebApi.OAuth2.OwinOAuth.Provider;
using Owin;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Orchard.WebApi.OAuth2
{
    public partial class Startup
    {

        // 有关配置身份验证的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            #region OAuthAuthorizationServer
            //https://docs.microsoft.com/en-us/aspnet/aspnet/overview/owin-and-katana/owin-oauth-20-authorization-server
            var oAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString(TokenConstants.TokenPath),//获取 access_token 认证服务请求地址
                AuthorizeEndpointPath = new PathString(TokenConstants.AuthorizePath),//获取 authorization_code 认证服务请求地址
                //AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(TokenConstants.AccessTokenExpireTime),//access_token 过期时间
                ApplicationCanDisplayErrors = true,
                //在生产模式下设 AllowInsecureHttp = false
                AllowInsecureHttp = true,

                Provider = new AuthorizationServerProvider(),//access_token 相关认证服务
                AuthorizationCodeProvider = new AuthorizationCodeProvider(),////authorization_code 认证服务
                RefreshTokenProvider = new RefreshTokenProvider(),//refresh_token 认证服务

                // AccessTokenFormat = new TicketDataFormat(app.CreateDataProtector(
                //typeof(OAuthAuthorizationServerMiddleware).Namespace,
                //"Access_Token", "v1")),
                // RefreshTokenFormat = new TicketDataFormat(app.CreateDataProtector(
                // typeof(OAuthAuthorizationServerMiddleware).Namespace,
                // "Refresh_Token", "v1")),
            };
            app.UseOAuthAuthorizationServer(oAuthOptions);
            #endregion OAuthAuthorizationServer

            #region UseOAuthBearerTokens
            var oAuthBearerOptions = new OAuthBearerAuthenticationOptions()
            {
                Provider = new OAuthBearerProvider(),//Bearer相关认证服务
                AccessTokenProvider = new AccessTokenProvider()// Receives the bearer token
            };
            app.UseOAuthBearerAuthentication(oAuthBearerOptions);
            //表示 token_type 使用 bearer 方式
            #endregion  UseOAuthBearerTokens

        }
    }
}
