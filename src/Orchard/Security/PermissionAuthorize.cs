using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Orchard.Security
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PermissionAuthorize
    {
        // Constructors
        /// <summary>
        /// 
        /// </summary>
        public PermissionAuthorize() { }

        // Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        public PermissionAuthorize(HttpContextBase httpContext)
        {

        }

        // Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permissionName"></param>
        /// <returns></returns>
        public abstract bool Authorize(IPrincipal user, string permissionName);
    }
}
