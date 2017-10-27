using System.Web.Security;

namespace Orchard.Security {
    /// <summary>
    /// 
    /// </summary>
    public interface IMembershipSettings {
        /// <summary>
        /// 
        /// </summary>
        bool UsersCanRegister { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool UsersMustValidateEmail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string ValidateEmailRegisteredWebsite { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string ValidateEmailContactEMail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool UsersAreModerated { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool NotifyModeration { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string NotificationsRecipients { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool EnableLostPassword { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool EnableCustomPasswordPolicy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int MinimumPasswordLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool EnablePasswordUppercaseRequirement { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool EnablePasswordLowercaseRequirement { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool EnablePasswordNumberRequirement { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool EnablePasswordSpecialRequirement { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool EnablePasswordExpiration { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int PasswordExpirationTimeInDays { get; set; }
        /// <summary>
        /// 
        /// </summary>
        MembershipPasswordFormat PasswordFormat { get; set; }
    }
}