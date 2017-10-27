using NLog;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Orchard.OAuth2.OwinOAuth
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationBase : HttpApplication
    {
        /// <summary>
        ///  异常处理
        /// </summary>
        protected virtual bool HandleException(ILogger logger)
        {
            HttpContext context = HttpContext.Current;
            // 在出现未处理的错误时运行的代码
            System.Exception exception = context.Server.GetLastError();
            if (exception is OperationCanceledException)
            {
                context.Server.ClearError();
                return false;
            }
            string url = context.Request.Url.AbsoluteUri;
            if (!string.IsNullOrEmpty(url) && url.Contains("UEditor"))
            {
                context.Server.ClearError();
                return false;
            }
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            // 获得前一个异常的实例
            StringBuilder errorBuilder = new StringBuilder("请求地址：" + url + ",原始请求地址：" + context.Request.RawUrl + ",原始请求地址：" + context.Request.UrlReferrer);
            if (context.Request.ContentLength <= 2 * 1024 * 1024)
            {
                try
                {
                    string contentType = context.Request.ContentType;
                    string[] contentTypeArray = new string[] { "application/json", "text/xml", "application/x-www-form-urlencoded", "application/xml" };
                    if (contentTypeArray.ToList().Exists(a => a == contentType))
                    {
                        var stream = context.Request.InputStream;
                        using (var streamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
                        {
                            errorBuilder.Append(",请求内容：" + streamReader.ReadToEnd());
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, errorBuilder.ToString());
                }
            }
            if (exception is HttpException)
            {
                HttpException httpException = exception as HttpException;
                int errCode = httpException.GetHttpCode();
                if (errCode >= 500)
                {
                    if (logger != null)
                    {
                        logger.Error(errorBuilder.ToString(), httpException);
                    }
                    context.Server.ClearError();
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (logger != null)
                {
                    logger.Error(exception, errorBuilder.ToString());
                }
                context.Server.ClearError();
            }
            return true;
        }
    }
}
