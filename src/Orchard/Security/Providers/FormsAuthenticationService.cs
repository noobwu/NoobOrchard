using Orchard.Environment.Configuration;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Mvc.Extensions;
using Orchard.Services;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace Orchard.Security.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class FormsAuthenticationService : IAuthenticationService
    {
        private const int _cookieVersion = 3;

        private readonly ShellSettings _settings;
        private readonly IClock _clock;
        private readonly IMembershipService _membershipService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISslSettingsProvider _sslSettingsProvider;
        private readonly IMembershipValidationService _membershipValidationService;

        private IUser _signedInUser;
        private bool _isAuthenticated;

        // This fixes a performance issue when the forms authentication cookie is set to a
        // user name not mapped to an actual Orchard user content item. If the request is
        // authenticated but a null user is returned, multiple calls to GetAuthenticatedUser
        // will cause multiple DB invocations, slowing down the request. We therefore
        // remember if the current user is a non-Orchard user between invocations.
        private bool _isNonOrchardUser;

        public FormsAuthenticationService(
            ShellSettings settings,
            IClock clock,
            IMembershipService membershipService,
            IHttpContextAccessor httpContextAccessor,
            ISslSettingsProvider sslSettingsProvider,
            IMembershipValidationService membershipValidationService)
        {
            _settings = settings;
            _clock = clock;
            _membershipService = membershipService;
            _httpContextAccessor = httpContextAccessor;
            _sslSettingsProvider = sslSettingsProvider;
            _membershipValidationService = membershipValidationService;

            Logger = NullLogger.Instance;

        }

        public ILogger Logger { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="expireDate">有效期(单位分钟)</param>
        public void SignIn(IUser user, int expireDate)
        {
            TimeSpan expirationTimeSpan =  TimeSpan.FromMinutes(expireDate);
            var now = _clock.UtcNow.ToLocalTime();
            bool createPersistentCookie = expireDate > 0 ? true : false;
            // The cookie user data is "{userName.Base64};{tenant}".
            // The username is encoded to Base64 to prevent collisions with the ';' seprarator.
            var userData = user.UserId.ToString().ToBase64() + ";" + user.UserName.ToBase64() + ";" + _settings.Name;
            var ticket = new FormsAuthenticationTicket(
                _cookieVersion,
                user.UserName,
                now,
                now.Add(expirationTimeSpan),
                createPersistentCookie,
                userData,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = _sslSettingsProvider.GetRequiresSSL(),
                Path = FormsAuthentication.FormsCookiePath
            };

            var httpContext = _httpContextAccessor.Current();

            if (!String.IsNullOrEmpty(_settings.RequestUrlPrefix))
            {
                cookie.Path = GetCookiePath(httpContext);
            }

            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }
            if (createPersistentCookie)
            {
                cookie.Expires = ticket.Expiration;
            }
            httpContext.Response.Cookies.Add(cookie);

            _isAuthenticated = true;
            _isNonOrchardUser = false;
            _signedInUser = user;
        }
        /// <summary>
        /// 
        /// </summary>
        public void SignOut()
        {
            _signedInUser = null;
            _isAuthenticated = false;
            FormsAuthentication.SignOut();

            // overwritting the authentication cookie for the given tenant
            var httpContext = _httpContextAccessor.Current();
            var rFormsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
            {
                Expires = DateTime.Now.AddYears(-1),
            };

            if (!String.IsNullOrEmpty(_settings.RequestUrlPrefix))
            {
                rFormsCookie.Path = GetCookiePath(httpContext);
            }

            httpContext.Response.Cookies.Add(rFormsCookie);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public void SetAuthenticatedUserForRequest(IUser user)
        {
            _signedInUser = user;
            _isAuthenticated = true;
            _isNonOrchardUser = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async  Task<IUser> GetAuthenticatedUser()
        {

            if (_isNonOrchardUser)
                return null;

            if (_signedInUser != null || _isAuthenticated)
                return _signedInUser;

            var httpContext = _httpContextAccessor.Current();
            if (httpContext.IsBackgroundContext() || !httpContext.Request.IsAuthenticated || !(httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)httpContext.User.Identity;
            var userData = formsIdentity.Ticket.UserData ?? "";

            // The cookie user data is {userName.Base64};{tenant}.
            var userDataSegments = userData.Split(';');

            if (userDataSegments.Length < 2)
            {
                return null;
            }

            var userDataName = userDataSegments[0];
            var userDataTenant = userDataSegments[1];

            try
            {
                userDataName = userDataName.FromBase64();
            }
            catch
            {
                return null;
            }

            if (!String.Equals(userDataTenant, _settings.Name, StringComparison.Ordinal))
            {
                return null;
            }

            _signedInUser =await _membershipService.GetUser(userDataName);
            if (_signedInUser == null || !_membershipValidationService.CanAuthenticateWithCookie(_signedInUser))
            {
                _isNonOrchardUser = true;
                return null;
            }
            _isAuthenticated = true;
            return _signedInUser;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private string GetCookiePath(HttpContextBase httpContext)
        {
            var cookiePath = httpContext.Request.ApplicationPath;
            if (cookiePath != null && cookiePath.Length > 1)
            {
                cookiePath += '/';
            }
            cookiePath += _settings.RequestUrlPrefix;
            return cookiePath;
        }
    }
}
