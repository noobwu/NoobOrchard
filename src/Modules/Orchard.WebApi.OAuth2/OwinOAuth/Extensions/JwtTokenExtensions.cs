using Microsoft.IdentityModel.Tokens;
using NLog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Orchard.WebApi.OAuth2.OwinOAuth.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class JwtTokenExtensions
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IEnumerable<Claim> ToClaims(this string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                //JwtSecurityToken jwtSecurityToken = tokenHandler.ReadJwtToken(token);
                //return jwtSecurityToken.Claims;
                JwtSecurityToken jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                if (jwtToken == null)
                    return null;
                return jwtToken.Claims;

            }
            catch (Exception ex)
            {
                logger.Error(ex, "token:"+token);
            }
            return null;
           
        }
    }
}