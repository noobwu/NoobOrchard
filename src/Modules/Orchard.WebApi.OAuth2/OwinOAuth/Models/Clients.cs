using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.WebApi.OAuth2.OwinOAuth.Models
{

    /// <summary>
    /// 
    /// </summary>
    public class Clients
    {
        static Clients()
        {
            ApiClients = new List<OauClient>() {
                ApiCloudClient,
                ApiCodeGrantClient,
                ApiCodeGrantClient
            };
        }
        /// <summary>
        /// 
        /// </summary>
        public const int RefreshTokenLifeTime = 1 * 24 * 60;//1天
        /// <summary>
        /// 短信Token过期时间
        /// </summary>
        public const int SMSTokenLifeTime = 30;//30分钟
        /// <summary>
        /// 
        /// </summary>
        public readonly static List<OauClient> ApiClients;

        public readonly static OauClient ApiCloudClient = new OauClient
        {
            AllowedOrigin = "*",
            ApplicationType = 0,
            AppName = "ApiCloud应用",
            AppId = "72E1B303E13AB00D485BD9E0BBE6A093",// Utils.MD5("NoobApiCloud")
            AppSecret = "8F0D8CEBE0A0AE352732DE114172C3AC",// Utils.MD5("NoobApiCloudSecret")
            RefreshTokenLifeTime = 7 * 24 * 60
        };

        public readonly static OauClient ApiCodeGrantClient = new OauClient
        {
            AllowedOrigin = "*",
            ApplicationType = 0,
            AppName = "ApiCodeGrant应用",
            AppId = "E937A9C70E6B7AE8612B9887461A5353",// Utils.MD5("NoobApiCodeGrant")
            AppSecret = "E7ED5DE668C20E3414F92A31866E12C9",// Utils.MD5("NoobApiCodeGrantSecret")
            RefreshTokenLifeTime = 7 * 24 * 60,
            // AuthorizationCodeGrant project should be running on this URL.
            RedirectUrl = "http://localhost:38500/"
        };
        public readonly static OauClient ApiImplicitGrantClient = new OauClient
        {
            AllowedOrigin = "*",
            ApplicationType = 0,
            AppName = "ApiImplicitGrant应用",
            AppId = "2B7A5085613C1A12F4FDAC0A509126D1",// Utils.MD5("NoobApiImplicitGrant")
            AppSecret = "ED6A2CF52D57D4F1C4FC8B27A634D37B",// Utils.MD5("NoobApiImplicitGrantSecret")
            RefreshTokenLifeTime = 7 * 24 * 60,
            //ImplicitGrant project should be running on this specific port '38515'
            RedirectUrl = "http://localhost:38515/Home/SignIn"
        };

    }

}