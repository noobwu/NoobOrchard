using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Orchard.WebApi.OAuth2.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthorizeRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string error { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class AuthorizeGrantController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Index(string code)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(code, Encoding.UTF8, "text/plain")
            };
        }
        [HttpGet]
        public HttpResponseMessage Token()
        {
            var url = Request.RequestUri;
            return new HttpResponseMessage()
            {
                Content = new StringContent("", Encoding.UTF8, "text/plain")
            };
        }
    }
}
