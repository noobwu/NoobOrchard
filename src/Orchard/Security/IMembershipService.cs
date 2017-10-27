using System.Threading.Tasks;

namespace Orchard.Security
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMembershipService : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IMembershipSettings GetSettings();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<IUser> GetUser(string username);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userNameOrEmail"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<IUser> ValidateUser(string userNameOrEmail, string password);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        void SetPassword(IUser user, string password);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="days"></param>
        /// <returns></returns>

        bool PasswordIsExpired(IUser user, int days);
    }
}
