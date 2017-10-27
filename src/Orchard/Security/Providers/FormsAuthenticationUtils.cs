using System;
using System.Web;
using System.Web.Security;

namespace Orchard.Security.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class FormsAuthenticationUtils
    {
        /// <summary>
        /// 写入Cookie票证
        /// </summary>
        /// <param name="context"></param>
        /// <param name="user"></param>
        /// <param name="expireDate">有效期(单位分钟)</param>
        public static void SignIn(HttpContextBase context, UserIdentifier user, int expireDate)
        {
            //<authentication mode="Forms" />
            /* 
             使用 cookie 名、版本、目录路径、发布日期、过期日期、持久性以及用户定义的数据初始化
           version:票证的版本号。 name:与票证关联的用户名。issueDate:票证发出时的本地日期和时间。
           expiration:票证过期时的本地日期和时间。
           isPersistent:如果票证将存储在持久性 Cookie 中（跨浏览器会话保存），则为true；否则为 false。 
           如果该票证存储在 URL 中，将忽略此值。userData: 存储在票证中的用户特定的数据。
           cookiePath:票证存储在 Cookie 中时的路径。
            */
            // The cookie user data is "{userId.Base64}".
            // The username is encoded to Base64 to prevent collisions with the ';' seprarator.
            var userData = user.UserId.ToString().ToBase64();
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    version: 3,
                    name: user.UserName,
                    issueDate: DateTime.Now,
                    expiration: DateTime.Now.AddMinutes(expireDate),
                    isPersistent: true,
                    userData: userData,
                    cookiePath: FormsAuthentication.FormsCookiePath
             );
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                FormsAuthentication.Encrypt(ticket))
            {
                HttpOnly = true,
                //Path = FormsAuthentication.FormsCookiePath
            };
            //if (!string.IsNullOrEmpty(FormsAuthentication.CookieDomain))
            //{
            //    cookie.Domain = FormsAuthentication.CookieDomain;
            //}
            cookie.Expires = ticket.Expiration;
            context.Response.Cookies.Add(cookie);
        }
        /// <summary>
        /// 获取票证（票证:用于 Forms 身份验证对用户进行标识）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static FormsAuthenticationTicket GetTicket(HttpRequestBase request)
        {
            var cookie = request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                return ticket;
            }
            return null;
        }
    }
}
