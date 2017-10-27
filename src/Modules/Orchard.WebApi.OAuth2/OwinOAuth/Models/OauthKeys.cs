using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.WebApi.OAuth2.OwinOAuth.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OAuthKeys
    {
        /// <summary>
        /// 
        /// </summary>
        public const string Prefix = "as:";
        /// <summary>
        /// 
        /// </summary>
        public const string ClientId = Prefix+"client_id";
        /// <summary>
        /// 
        /// </summary>
        public const string RefreshToken = Prefix+ "refreshToken";
        /// <summary>
        /// 
        /// </summary>
        public const string RefreshTokenClaim = "newClaim";
        /// <summary>
        /// 
        /// </summary>
        public const string ClientAllowedOrigin = Prefix + "clientAllowedOrigin";

        /// <summary>
        /// 
        /// </summary>
        public const string AuthorizeCodeToken = Prefix + "authorizeCodeToken";

        /// <summary>
        /// The Key of Microsoft.Owin.Security.AuthenticationProperties 
        /// </summary>
        public const string AuthPropClientId ="client_id";
    }
}