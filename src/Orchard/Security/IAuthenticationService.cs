using System.Threading.Tasks;

namespace Orchard.Security {
    public interface IAuthenticationService : IDependency {
        /// <summary>
        /// 账户登录
        /// </summary>
        /// <param name="user">账户信息</param>
        /// <param name="expireDate">有效期(单位分钟)</param>
        void SignIn(IUser user, int expireDate);
        /// <summary>
        /// 
        /// </summary>
        void SignOut();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        void SetAuthenticatedUserForRequest(IUser user);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IUser> GetAuthenticatedUser();
    }
}
