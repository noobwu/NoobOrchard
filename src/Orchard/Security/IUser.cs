using Orchard.ContentManagement;

namespace Orchard.Security {
    /// <summary>
    /// Interface provided by the "User" model. 
    /// </summary>
    public interface IUser {
        /// <summary>
        /// 
        /// </summary>
        string UserName { get; }
        /// <summary>
        /// 
        /// </summary>
        string Email { get; }
        /// <summary>
        /// 
        /// </summary>
        int UserId { get; }
        /// <summary>
        /// 
        /// </summary>
        byte Status { get; }
    }
}
