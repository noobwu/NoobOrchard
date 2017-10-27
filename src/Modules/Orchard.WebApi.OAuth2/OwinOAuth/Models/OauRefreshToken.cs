using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.WebApi.OAuth2.OwinOAuth.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OauRefreshToken
    {
        /// <summary>
        /// 新令牌Id
        /// </summary>
        public virtual int RefreshTokenId { get; set; }

        ///<summary>
        /// 账户
        /// </summary>
        public virtual string UserName { get; set; }

        ///<summary>
        /// 用户ID
        /// </summary>
        public virtual int UserID { get; set; }

        ///<summary>
        /// 应用Id
        /// </summary>
        public virtual string AppId { get; set; }

        ///<summary>
        /// 访问令牌
        /// </summary>
        public virtual string AccessToken { get; set; }

        ///<summary>
        /// Token类型
        /// </summary>
        public virtual string TokenType { get; set; }

        ///<summary>
        /// 自定义Token类型
        /// </summary>
        public virtual string CustomTokenType { get; set; }

        ///<summary>
        /// 发布时间
        /// </summary>
        public virtual DateTime IssuedUtc { get; set; }

        ///<summary>
        /// 过期时间
        /// </summary>
        public virtual DateTime ExpiresUtc { get; set; }

        ///<summary>
        /// RefreshToken信息
        /// </summary>
        public virtual string RefreshToken { get; set; }

        ///<summary>
        /// ProtectedTicket
        /// </summary>
        public virtual string ProtectedTicket { get; set; }
    }
}