using NSubstitute;
using NUnit.Framework;
using Orchard.Net.Mail.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Tests.Net.Mail
{
    [TestFixture]
    public class SmtpEmailSenderTest
    {
        private  SmtpEmailSender _smtpEmailSender;
        [OneTimeSetUp]
        public void SetUp()
        {
            var configuration = Substitute.For<ISmtpEmailSenderConfiguration>();

            configuration.DefaultFromAddress.Returns("huxinjishubu@126.com");
            configuration.DefaultFromDisplayName.Returns("huxinjishubu@126.com");

            configuration.Host.Returns("smtp.126.com");
            configuration.Port.Returns(25);

            //configuration.Domain.Returns("");
            configuration.UserName.Returns("huxinjishubu@126.com");
            configuration.Password.Returns("huxinjishubu365");

            //configuration.EnableSsl.Returns(false);
            //configuration.UseDefaultCredentials.Returns(false);

            _smtpEmailSender = new SmtpEmailSender(configuration);
        }
        [Test]
        public void SmtpEmailSender_Send_Test()
        {
            _smtpEmailSender.Send(
                "noobwu@126.com",
                "Test email",
                "An email body"
                );
        }
    }
}
