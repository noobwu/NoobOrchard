using Orchard.WebApi.OAuth2.OwinOAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Orchard.WebApi.OAuth2.Controllers
{
    public class CustomUserController : ApiController
    {
        //[HttpPost]
        //public string Info(FormDataCollection formData)
        //{
        //    string token = formData["token"];
        //    var principal = JwtManager.GetPrincipal(token);
        //    if (principal == null)
        //    {
        //        return "";
        //    }
        //    else
        //    {
        //        return principal.Identity.Name;
        //    }

        //}

        [HttpPost]
        public string Info([FromBody]dynamic value)
        {
            string token = value.token;
            var principal = JwtManager.GetPrincipal(token);
            if (principal == null)
            {
                return "";
            }
            else
            {
                return principal.Identity.Name;
            }

        }
    }
}
