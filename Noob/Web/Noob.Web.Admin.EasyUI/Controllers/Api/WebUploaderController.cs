using Noob.Web.Admin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Noob.Web.Admin.EasyUI.Controllers.Api
{
    [AdminApiAuthorize]
    /// <summary>
    /// 
    /// </summary>
    public class WebUploaderController : WebUploader.WebApi.Controllers.WebUploaderBaseController
    {

    }
}
