using Orchard.ContentManagement;
using Orchard.Security.Permissions;

namespace Orchard.Security {
    /// <summary>
    /// Entry-point for configured authorization scheme. Role-based system
    /// provided by default. 
    /// </summary>
    public interface IAuthorizationService : IDependency {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="user"></param>
        void CheckAccess(Permission permission, IUser user);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        bool TryCheckAccess(Permission permission, IUser user);
    }
}