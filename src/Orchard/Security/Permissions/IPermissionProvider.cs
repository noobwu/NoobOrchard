using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;

namespace Orchard.Security.Permissions {
    /// <summary>
    /// Implemented by modules to enumerate the types of permissions
    /// the which may be granted
    /// </summary>
    public interface IPermissionProvider : IDependency {
        /// <summary>
        /// 
        /// </summary>
        Feature Feature { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Permission> GetPermissions();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<PermissionStereotype> GetDefaultStereotypes();
    }
    /// <summary>
    /// 
    /// </summary>
    public class PermissionStereotype {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Permission> Permissions { get; set; }
    }
}