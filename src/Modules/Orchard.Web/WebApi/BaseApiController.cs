using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Orchard.Logging;

namespace Orchard.Web.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseApiController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        protected ILogger logger;
        /// <summary>
        /// 
        /// </summary>
        public BaseApiController()
        { }
    }
}
