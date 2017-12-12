using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Xml.Linq;
using System.Configuration;
using System.Text;
using Noob.Web.Admin.Security;
using Orchard.Web.Mvc.Controllers;
using Orchard.Caching;
using Orchard.Logging;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Noob.Services.Caching;
using Orchard.Utility;
using Orchard.Security.Providers;
using Orchard.Security;
using Noob.Domain;
using Orchard.Web.Mvc.Infrastructure;

namespace Noob.Web.Admin.Bootstrap.Controllers
{
    //[AdminAuthorize]
    public class BaseAdminController : BaseController
    {
        #region Members
        private readonly ICacheManager _cacheManager;
        private readonly string uploadUrl;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="groupRightsService"></param>
        /// <param name="userGroupService"></param>
        /// <param name="logFactory"></param>
        public BaseAdminController(ICacheManager cacheManager,ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            _cacheManager = cacheManager;
            if (loggerFactory != null)
            {
                logger = loggerFactory.GetLogger(this.GetType());
            }
            string appPath = RequestHelper.GetApplicationPath();
            //WebConfig webConfig = WebConfigService.GetWebConfig("~/App_Data/Configs/GeneralConfigs.config", 
            //    "GeneralConfigs/AdminConfig", cacheManager);
            WebConfig webConfig= ContainerContext.Current.Resolve<WebConfig>();
            uploadUrl = webConfig.UploadUrl;
            if (!uploadUrl.Contains(appPath))
            {
                uploadUrl = appPath + uploadUrl;
            }
            string rootUrl = webConfig.RootUrl;
            if (!rootUrl.Contains(appPath))
            {
                rootUrl = appPath + rootUrl;
            }
            ViewBag.RootUrl = rootUrl;
            ViewBag.UploadUrl = uploadUrl;
        }
        private int userId = 0;
        private string userName;
        private XDocument _passportDoc;
        /// <summary>
        /// 当前用户功能权限
        /// </summary>
        /// <value>The user rights.</value>
        protected XDocument PassportDoc
        {
            get
            {
                if (_passportDoc == null)
                {
                    string dirPath = Utils.GetMapPath("/Files/CertifyPool/");
                    string filePath = dirPath + LoginUserID + ".cer";
                    try
                    {
                        _passportDoc = XDocument.Load(filePath);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
                return _passportDoc;
            }
        }

        private XElement UserRights
        {
            get
            {
                if (PassportDoc != null && PassportDoc.Root != null)
                {
                    return PassportDoc.Root.Element("RightsList");
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 开始执行指定的请求上下文
        /// </summary>
        /// <param name="requestContext">   请求上下文。</param>
        /// <param name="callback"> 异步回调。</param>
        /// <param name="state"> 状态。</param>
        /// <returns>返回一个 IAsyncController 实例。</returns>
        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            if (requestContext.HttpContext.User.Identity.IsAuthenticated &&
                !string.IsNullOrEmpty(requestContext.HttpContext.User.Identity.Name))
            {
                UserIdentifier userInfo = GetSession(requestContext.HttpContext,SessionKeys.ADMIN_USER, () =>
                {
                    //var cookie = requestContext.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                    //var ticket = FormsAuthentication.Decrypt(cookie.Value);
                    var ticket = FormsAuthenticationUtils.GetTicket(requestContext.HttpContext.Request);
                    userInfo = UserIdentifier.Parse(ticket.UserData);
                    userInfo.UserName = requestContext.HttpContext.User.Identity.Name;
                    return userInfo;
                    //logger.Debug("cookie,ticket:" + ticket + "userName:" + userName + ",userId:" + userId);
                });
                if (userInfo == null)
                {
                    logger.Error("Session:User为空" + Session[SessionKeys.ADMIN_USER]);
                }
                else
                {
                    userName = userInfo.UserName;
                    userId = userInfo.UserId;
                    //_cacheManager.Get(CacheKeys.ADMIN_USER_PREFIX + userInfo.UserId,
                    //    (a)=> { return UserRights; });
                    SetSession(requestContext.HttpContext,SessionKeys.ADMIN_USER_PERMISSIONS,UserRights);
   
                }
            }
            return base.BeginExecute(requestContext, callback, state);
        }


        /// <summary>
        /// 
        /// </summary>
        protected int LoginUserID
        {
            get { return userId; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string LoginUserName
        {
            get { return userName; }
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Mvc.JsonResult"/> object that serializes the specified object to JavaScript Object Notation (JSON) format using the content type, content encoding, and the JSON request behavior.
        /// </summary>
        /// 
        /// <returns>
        /// The result object that serializes the specified object to JSON format.
        /// </returns>
        /// <param name="data">The JavaScript object graph to serialize.</param>
        /// <param name="contentType">The content type (MIME type).</param>
        /// <param name="contentEncoding">The content encoding.</param>
        /// <param name="behavior">The JSON request behavior</param>
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            //Json fix for admin area
            //sometime our entities have big text values returned (e.g. product desriptions)
            //of course, we can set and return them as "empty" (we already do it so). Furthermore, it's a perfoemance optimization
            //but it's better to avoid exceptions for other entities and allow maximum JSON length
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = int.MaxValue
            };
            //return base.Json(data, contentType, contentEncoding, behavior);
        }

        /// <summary>
        /// 获取完整的上传Url
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <param name="defaultUrl"></param>
        /// <returns></returns>
        protected string GetUploadUrl(string fileUrl, string defaultUrl = "")
        {
            if (string.IsNullOrEmpty(fileUrl)) return defaultUrl;
            return fileUrl + fileUrl;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        protected void CheckData<T>(T data) where T : class
        {
            if (data == null)
            {
                RedirectToAction("NoFoundData", "Home");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected static long GetRandomGrainId()
        {
            return RandomHelper.GetRandom();
        }
    }
}