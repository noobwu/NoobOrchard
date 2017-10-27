using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.WebApi.OAuth2.OwinOAuth.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenConstants
    {
        /// <summary>
        /// 
        /// </summary>
        public const string LoginPath = "";
        /// <summary>
        /// 
        /// </summary>

        public const string LogoutPath = "";

        /// <summary>
        /// 
        /// </summary>
        public const string AuthorizePath = "/Api/OAuth/Authorize";
        /// <summary>
        /// 
        /// </summary>
        public const string TokenPath = "/Api/OAuth/Token";

        /// <summary>
        /// The period of time the access token remains valid after being issued(unit for minute) 
        /// </summary>

        public const int AccessTokenExpireTime = 7 * 24 * 60;
    }
}