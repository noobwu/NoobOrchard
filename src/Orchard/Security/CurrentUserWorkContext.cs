using System;
using System.Threading.Tasks;

namespace Orchard.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class CurrentUserWorkContext : IWorkContextStateProvider
    {
        private readonly IAuthenticationService _authenticationService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authenticationService"></param>
        public CurrentUserWorkContext(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public Func<WorkContext, T> Get<T>(string name)
        {
            if (name == "CurrentUser")
            {
                var authTask = _authenticationService.GetAuthenticatedUser();
                Task.WaitAll(authTask);
                return ctx => (T)authTask.Result;
            }
            return null;
        }
    }
}
