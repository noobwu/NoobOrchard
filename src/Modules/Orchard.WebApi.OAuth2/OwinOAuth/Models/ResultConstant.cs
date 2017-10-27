using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace  Orchard.WebApi.OAuth2.OwinOAuth.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ResultCodes
    {
        /// <summary>
        /// token过期或无效
        /// </summary>
        public const int UNAUTHORIZED = 9000;

        /// <summary>
        /// 未登录
        /// </summary>
        public const int NO_LOGIN = 9001;
    }

    public class ResultMsg
    {
        /// <summary>
        /// token过期或无效
        /// </summary>
        public const string UNAUTHORIZED = "很抱歉，您的token过期，请重新获取获取Token。";

        /// <summary>
        /// 未登录
        /// </summary>
        public const string NO_LOGIN = "很抱歉，您的还未登录，请重新登录。";
    }
}