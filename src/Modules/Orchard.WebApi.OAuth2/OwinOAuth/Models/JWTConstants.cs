using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.WebApi.OAuth2.OwinOAuth.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class JWTConstants
    {
        /// <summary>
        /// Gets or sets the allowed audiences an inbound JWT will be checked against.
        /// </summary>
        public const string Audience = "";
        /// <summary>
        /// The issuer of a JWT token.
        /// </summary>
        public const string Issuer = "https://github.com/noobwu";
        /// <summary>
        ///  The Secret of a JWT token.
        /// </summary>
        public const string Secret = "71F4C57F60971A26A12F7B11B7C82119";//md5(NoobJWTToken)
    }
}