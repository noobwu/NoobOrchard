using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Jwt;
using System;
using System.IdentityModel.Tokens.Jwt;
using Orchard.WebApi.OAuth2.OwinOAuth.Models;
using System.Security.Claims;

namespace CNTrade.Web.OwinOAuth.Jwt
{
    /// <summary>
    /// 
    /// </summary>
    public class OAuthJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly OAuthAuthorizationServerOptions _options;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public OAuthJwtFormat(OAuthAuthorizationServerOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
        private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Protect(AuthenticationTicket data)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = data.Identity,
                Expires = now.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);
            return token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="protectedText"></param>
        /// <returns></returns>
        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}