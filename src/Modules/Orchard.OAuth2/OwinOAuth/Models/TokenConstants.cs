using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.OAuth2.OwinOAuth.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenConstants
    {

        /// <summary>
        /// AuthorizationServer project should run on this URL
        /// </summary>
        public const string AuthorizationServerBaseAddress = "http://localhost:18519";

        /// <summary>
        /// ResourceServer project should run on this URL
        /// </summary>
        public const string ResourceServerBaseAddress = "http://localhost:18519";

        /// <summary>
        /// ImplicitGrant project should be running on this specific port '38515'
        /// </summary>
        public const string ImplicitGrantCallBackPath = "http://localhost:18519/ImplicitGrant/SignIn";

        /// <summary>
        /// AuthorizationCodeGrant project should be running on this URL.
        /// </summary>
        public const string AuthorizeCodeCallBackPath = "http://localhost:18519/Api/AuthorizeGrant/Index";
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

        /// <summary>
        /// 
        /// </summary>

        public const string MePath = "/Api/ResourceServer";
    }
}