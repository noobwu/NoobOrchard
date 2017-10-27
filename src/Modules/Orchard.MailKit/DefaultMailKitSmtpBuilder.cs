using MailKit.Net.Smtp;
using Orchard.Net.Mail.Smtp;

namespace Orchard.MailKit
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultMailKitSmtpBuilder : IMailKitSmtpBuilder, ITransientDependency
    {
        private readonly ISmtpEmailSenderConfiguration _smtpEmailSenderConfiguration;

        public DefaultMailKitSmtpBuilder(ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration)
        {
            _smtpEmailSenderConfiguration = smtpEmailSenderConfiguration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual SmtpClient Build()
        {
            var client = new SmtpClient();

            try
            {
                ConfigureClient(client);
                return client;
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        protected virtual void ConfigureClient(SmtpClient client)
        {
            client.Connect(
                _smtpEmailSenderConfiguration.Host,
                _smtpEmailSenderConfiguration.Port,
                _smtpEmailSenderConfiguration.EnableSsl
            );

            var userName = _smtpEmailSenderConfiguration.UserName;
            if (!userName.IsNullOrEmpty())
            {
                client.Authenticate(
                    _smtpEmailSenderConfiguration.UserName, 
                    _smtpEmailSenderConfiguration.Password
                );
            }
        }
    }
}