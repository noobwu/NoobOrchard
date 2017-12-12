using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Threading;
using Orchard.Web.WebApi.Security;
using Orchard.Web.WebApi;
using Orchard.Caching;
using Orchard.Logging;
using Orchard.Environment.Configuration;
using Noob.Services.Caching;
using Noob.IServices;

namespace Noob.Web.Admin.EasyUI.Controllers
{
    [ApiAuthorize]
    public class BaseAdminApiController : BaseApiController
    {
        public BaseAdminApiController()
        {
        }
        #region Members
        private readonly IAdmUserService  _service;
        private readonly ICacheManager _cacheManager;
        protected readonly string UploadDir="Uploads";
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="groupRightsService"></param>
        /// <param name="userGroupService"></param>
        /// <param name="logFactory"></param>
        public BaseAdminApiController(IAdmUserService  service, ICacheManager cacheManager,ILoggerFactory loggerFactory)
        {
            _service = service;
            _cacheManager = cacheManager;
            if (loggerFactory != null)
            {
                logger = loggerFactory.GetLogger(this.GetType());
            }
            WebConfig webConfig = WebConfigService.GetWebConfig("~/App_Data/Configs/GeneralConfigs.config",
                "GeneralConfigs/AdminConfig", cacheManager);
            UploadDir = webConfig.UploadDir;
            if (!string.IsNullOrEmpty(UploadDir))
            {
                UploadDir = UploadDir.Trim('/');
            }
        }

    }
}
