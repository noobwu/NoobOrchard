using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.WebApi.OAuth2.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        /// <summary>
        /// 
        /// </summary>

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        /// <summary>
        /// 
        /// </summary>

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        /// <summary>
        /// 
        /// </summary>

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
