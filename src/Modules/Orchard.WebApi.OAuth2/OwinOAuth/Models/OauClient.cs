using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.WebApi.OAuth2.OwinOAuth.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OauClient
    {

        ///<summary>
        /// 应用Id
        /// </summary>
        public virtual string AppId { get; set; }

        ///<summary>
        /// 应用密钥
        /// </summary>
        public virtual string AppSecret { get; set; }

        ///<summary>
        /// 应用名称
        /// </summary>
        public virtual string AppName { get; set; }

        ///<summary>
        /// 应用类型（0:JS 1:Android 2:IOS）
        /// </summary>
        public virtual int ApplicationType { get; set; }

        ///<summary>
        /// 令牌过期时长
        /// </summary>
        public virtual int RefreshTokenLifeTime { get; set; }

        ///<summary>
        /// 允许请求来源
        /// </summary>
        public virtual string AllowedOrigin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RedirectUrl { get; set; }
    }
}