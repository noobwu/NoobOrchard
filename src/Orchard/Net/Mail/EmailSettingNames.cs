namespace Orchard.Net.Mail
{
    /// <summary>
    /// Declares names of the settings defined by <see cref="EmailSettingProvider"/>.
    /// </summary>
    public  class EmailSettingNames
    {
        /// <summary>
        /// Orchard.Net.Mail.DefaultFromAddress
        /// </summary>
        public const string DefaultFromAddress = "Orchard.Net.Mail.DefaultFromAddress";

        /// <summary>
        /// Orchard.Net.Mail.DefaultFromDisplayName
        /// </summary>
        public const string DefaultFromDisplayName = "Orchard.Net.Mail.DefaultFromDisplayName";

        /// <summary>
        /// SMTP related email settings.
        /// </summary>
        public static class Smtp
        {
            /// <summary>
            /// Orchard.Net.Mail.Smtp.Host
            /// </summary>
            public const string Host = "Orchard.Net.Mail.Smtp.Host";

            /// <summary>
            /// Orchard.Net.Mail.Smtp.Port
            /// </summary>
            public const string Port = "Orchard.Net.Mail.Smtp.Port";

            /// <summary>
            /// Orchard.Net.Mail.Smtp.UserName
            /// </summary>
            public const string UserName = "Orchard.Net.Mail.Smtp.UserName";

            /// <summary>
            /// Orchard.Net.Mail.Smtp.Password
            /// </summary>
            public const string Password = "Orchard.Net.Mail.Smtp.Password";

            /// <summary>
            /// Orchard.Net.Mail.Smtp.Domain
            /// </summary>
            public const string Domain = "Orchard.Net.Mail.Smtp.Domain";

            /// <summary>
            /// Orchard.Net.Mail.Smtp.EnableSsl
            /// </summary>
            public const string EnableSsl = "Orchard.Net.Mail.Smtp.EnableSsl";

            /// <summary>
            /// Orchard.Net.Mail.Smtp.UseDefaultCredentials
            /// </summary>
            public const string UseDefaultCredentials = "Orchard.Net.Mail.Smtp.UseDefaultCredentials";
        }
    }
}