using System;
using MailKit.Net.Smtp;

namespace Orchard.MailKit
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMailKitSmtpBuilder
    {
        SmtpClient Build();
    }
}