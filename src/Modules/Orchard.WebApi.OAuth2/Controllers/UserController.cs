using Orchard.WebApi.OAuth2.OwinOAuth.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace Orchard.WebApi.OAuth2.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class UserController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public string Info([FromBody]dynamic value)
        {
            if (value == null)
            {
                return "invalid token";
            }
            string token = value.token;
            var  claims = token.ToClaims();
            if (claims == null||claims.Count()==0)
            {
                return "invalid user";
            }
            else
            {
                var userNameClaim = claims.Where(a=>a.ValueType==ClaimTypes.Name).FirstOrDefault();
                if (userNameClaim != null)
                {
                    return userNameClaim.Value;
                }
                return "invalid user";
            }

        }
    }
}
