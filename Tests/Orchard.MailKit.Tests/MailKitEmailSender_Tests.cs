using NSubstitute;
using NUnit.Framework;
using Orchard.Net.Mail.Smtp;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Orchard.MailKit.Tests
{
    [TestFixture]
    public class MailKitEmailSender_Tests
    {
        [Test]
        public void ShouldSend()
        {
            var mailSender = CreateMailKitEmailSender();

            mailSender.Send("huxinjishubu@126.com", "wu0527@gmail.com", "subject", "body", true);
        }

        [Test]
        public async Task ShouldSendAsync()
        {
            var mailSender = CreateMailKitEmailSender();

            await mailSender.SendAsync("huxinjishubu@126.com", "wu0527@gmail.com", "subject", "body", true);
        }

        [Test]
        public async Task ShouldSendMailMessage()
        {
            var mailSender = CreateMailKitEmailSender();
            var mailMessage = new MailMessage("huxinjishubu@126.com", "wu0527@gmail.com", "subject", "body")
            { IsBodyHtml = true };

            await mailSender.SendAsync(mailMessage);
        }

        [Test]
        public void ShouldSendMailMessageAsync()
        {
            var mailSender = CreateMailKitEmailSender();
            var mailMessage = new MailMessage("huxinjishubu@126.com", "wu0527@gmail.com", "subject", "body")
            { IsBodyHtml = true };

            mailSender.Send(mailMessage);
        }

        private static MailKitEmailSender CreateMailKitEmailSender()
        {
            var mailConfig = Substitute.For<ISmtpEmailSenderConfiguration>();

            mailConfig.Host.Returns("smtp.126.com");
            mailConfig.UserName.Returns("huxinjishubu@126.com");
            mailConfig.Password.Returns("huxinjishubu365");
            mailConfig.Port.Returns(25);
            mailConfig.EnableSsl.Returns(false);

            var mailSender = new MailKitEmailSender(mailConfig, new DefaultMailKitSmtpBuilder(mailConfig));
            return mailSender;
        }
    }
}
