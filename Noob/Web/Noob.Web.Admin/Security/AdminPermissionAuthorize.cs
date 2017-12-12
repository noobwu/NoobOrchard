using Noob.Domain;
using Orchard.Logging;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Noob.Web.Admin.Security
{
    public class AdminPermissionAuthorize : PermissionAuthorize
    {
        protected ILogger logger;
        HttpContextBase httpContext;
        // Constructors
        public AdminPermissionAuthorize(HttpContextBase httpContext)
            : base(httpContext)
        {
            logger = NullLogger.Instance;
            this.httpContext = httpContext;
        }
        public override bool Authorize(System.Security.Principal.IPrincipal user, string permissionName)
        {
            //logger.Debug("permissionName:" + permissionName + ",Identity:" + user.Identity.Name);
            return CheckPermission(permissionName);
        }
        /// <summary>
        /// 验证用户是否具有某个子系统的某个权限
        /// </summary>
        /// <param name="rightsCode">权限</param>
        /// <returns>true：有权；false：无</returns>
        protected bool CheckPermission(string permissionName)
        {
            if (string.IsNullOrEmpty(permissionName)) return true;
            if (UserRights == null)
            {
                return false;
            }
            string[] permissionArray = permissionName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var tmpUserRights = (from userRight in UserRights.Elements()
                                 join tmp in permissionArray on userRight.Element("RightsCode").Value equals tmp 
                                 select userRight);
            if (tmpUserRights == null||tmpUserRights.Count()==0) return false;
            return true;
        }

        public XElement UserRights
        {
            get
            {
                return httpContext.Session[SessionKeys.ADMIN_USER_PERMISSIONS] as XElement;
            }
        }
    }
}