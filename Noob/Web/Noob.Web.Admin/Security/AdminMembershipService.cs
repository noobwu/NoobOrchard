using Noob.IServices;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Services;
using System;
using System.Threading.Tasks;
using System.Web.Security;

namespace Noob.Web.Admin.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class AdminMembershipService : IMembershipService
    {
        private const string PBKDF2 = "PBKDF2";
        private const string DefaultHashAlgorithm = PBKDF2;
        private readonly IEncryptionService _encryptionService;
        private readonly IClock _clock;
        private readonly IAdmUserService _service;
        public AdminMembershipService(
            IClock clock,
            IEncryptionService encryptionService,IAdmUserService service)
        {
            _service = service;
            _encryptionService = encryptionService;
            _clock = clock;
            Logger = NullLogger.Instance;
        }
        public ILogger Logger { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<IUser> GetUser(string userName)
        {
            var entity = await _service.SingleAsync(a => a.UserName == userName);
            if (entity == null) return null;
            return new AdminMembership {
                UserName=entity.UserName,
                Email=entity.Email,
                Status=entity.Status,
                UserId=entity.UserID
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userNameOrEmail"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IUser> ValidateUser(string userNameOrEmail, string password)
        {
            var entity = await _service.SingleAsync(a => a.UserName == userNameOrEmail&&a.Password==password);
            if (entity == null) return null;
            return new AdminMembership
            {
                UserName = entity.UserName,
                Email = entity.Email
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public bool PasswordIsExpired(IUser user, int days)
        {
            return user.As<AdminMembership>().LastPasswordChangeTime.Value.AddDays(days) < _clock.Now;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public void SetPassword(IUser user, string password)
        {
            var membership = user.As<AdminMembership>();

            MembershipPasswordFormat passwordFormat = MembershipPasswordFormat.Hashed;
            switch (passwordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Hashed:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    break;
                default:
                    throw new ApplicationException("Unexpected password format value");
            }
            membership.LastPasswordChangeTime = _clock.Now;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IMembershipSettings GetSettings()
        {
            throw new NotImplementedException();
        }
    }
}