using Orchard.ContentManagement;
using Orchard.Events;
using Orchard.Security.Permissions;

namespace Orchard.Security
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuthorizationServiceEventHandler : IEventHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        void Checking(CheckAccessContext context);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        void Adjust(CheckAccessContext context);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        void Complete(CheckAccessContext context);
    }
    /// <summary>
    /// 
    /// </summary>
    public class CheckAccessContext
    {
        /// <summary>
        /// 
        /// </summary>
        public Permission Permission { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IUser User { get; set; }
        /// <summary>
        /// true if the permission has been granted to the user.
        /// </summary>
        public bool Granted { get; set; }
        /// <summary>
        ///if context.Permission was modified during an Adjust(context) in an event handler, Adjusted should be set to true.
        /// It means that the permission check will be done again by the framework.
        /// </summary>
        public bool Adjusted { get; set; }
    }
}
