using System.Threading.Tasks;
using Orchard.Net.Mail;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.Net.Mail;

namespace Orchard.MailKit
{
    /// <summary>
    /// 
    /// </summary>
    public class MailKitEmailSender : EmailSenderBase
    {
        private readonly IMailKitSmtpBuilder _smtpBuilder;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="smtpEmailSenderConfiguration"></param>
        /// <param name="smtpBuilder"></param>
        public MailKitEmailSender(
            IEmailSenderConfiguration smtpEmailSenderConfiguration,
            IMailKitSmtpBuilder smtpBuilder)
            : base(
                  smtpEmailSenderConfiguration)
        {
            _smtpBuilder = smtpBuilder;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        public override async Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            using (var client = BuildSmtpClient())
            {
                var message = BuildMimeMessage(from, to, subject, body, isBodyHtml);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isBodyHtml"></param>
        public override void Send(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            using (var client = BuildSmtpClient())
            {
                var message = BuildMimeMessage(from, to, subject, body, isBodyHtml);
                client.Send(message);
                client.Disconnect(true);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        protected override async Task SendEmailAsync(MailMessage mail)
        {
            using (var client = BuildSmtpClient())
            {
                var message = mail.ToMimeMessage();
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mail"></param>
        protected override void SendEmail(MailMessage mail)
        {
            using (var client = BuildSmtpClient())
            {
                var message = mail.ToMimeMessage();
                client.Send(message);
                client.Disconnect(true);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual SmtpClient BuildSmtpClient()
        {
            return _smtpBuilder.Build();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        private static MimeMessage BuildMimeMessage(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            var bodyType = isBodyHtml ? "html" : "plain";
            var message = new MimeMessage
            {
                Subject = subject,
                Body = new TextPart(bodyType)
                {
                    Text = body
                }
            };

            message.From.Add(new MailboxAddress(from));
            message.To.Add(new MailboxAddress(to));
            
            return message;
        }
    }
}