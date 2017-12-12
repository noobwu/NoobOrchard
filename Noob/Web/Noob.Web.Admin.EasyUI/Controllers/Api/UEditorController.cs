using Noob.Web.Admin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Noob.Web.Admin.EasyUI.Controllers.Api
{
    [AdminApiAuthorize]
    /// <summary>
    /// 
    /// </summary>
    public class UEditorController : UEditor.WebApi.Controllers.UEditorBaseApiController
    {
        /// <summary>
        /// 
        /// </summary>
        public UEditorController()
        {
            InitUploadDir("Uploads", "~/App_Data/Configs/UEditorConfig.json");
        }
      
    }
}
