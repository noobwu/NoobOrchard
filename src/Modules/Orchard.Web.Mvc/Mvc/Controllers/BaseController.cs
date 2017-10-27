using Orchard.Logging;
using Orchard.Utility;
using Orchard.Web.Models;
using Orchard.Web.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Orchard.Web.Mvc.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseController : Controller
    {

        /// <summary>
        /// 
        /// </summary>
        protected ILogger logger;
        /// <summary>
        /// MethodInfo for currently executing action.
        /// </summary>
        private MethodInfo _currentMethodInfo;
        /// <summary>
        /// Constructor.
        /// </summary>
        protected BaseController(ILoggerFactory loggerFactory)
        {
            if (loggerFactory != null)
            {
                logger = loggerFactory.GetLogger(GetType());
            }
        }
        #region OnActionExecuting / OnActionExecuted

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SetCurrentMethodInfoAndWrapResultAttribute(filterContext);
            base.OnActionExecuting(filterContext);
        }

        private void SetCurrentMethodInfoAndWrapResultAttribute(ActionExecutingContext filterContext)
        {
            //Prevent overriding for child actions
            if (_currentMethodInfo != null)
            {
                return;
            }
            _currentMethodInfo = filterContext.ActionDescriptor.GetMethodInfoOrNull();
        }

        #endregion
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <returns>Result</returns>
        public virtual string RenderPartialViewToString()
        {
            return RenderPartialViewToString(null, null);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <returns>Result</returns>
        public virtual string RenderPartialViewToString(string viewName)
        {
            return RenderPartialViewToString(viewName, null);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        public virtual string RenderPartialViewToString(object model)
        {
            return RenderPartialViewToString(null, model);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        public virtual string RenderPartialViewToString(string viewName, object model)
        {
            //Original source code: http://craftycodeblog.com/2010/05/15/asp-net-mvc-render-partial-view-to-string/
            if (string.IsNullOrEmpty(viewName))
                viewName = this.ControllerContext.RouteData.GetRequiredString("action");

            this.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindPartialView(this.ControllerContext, viewName);
                var viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }


        /// <summary>
        /// Log exception
        /// </summary>
        /// <param name="ex">Exception</param>
        protected void LogException(Exception ex)
        {
            HandleException(ex);
        }
        /// <summary>
        /// Display success notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected virtual void SuccessNotification(string message, bool persistForTheNextRequest = true)
        {
            AddNotification(NotifyType.Success, message, persistForTheNextRequest);
        }
        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected virtual void ErrorNotification(string message, bool persistForTheNextRequest = true)
        {
            AddNotification(NotifyType.Error, message, persistForTheNextRequest);
        }
        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        /// <param name="logException">A value indicating whether exception should be logged</param>
        protected virtual void ErrorNotification(Exception exception, bool persistForTheNextRequest = true, bool logException = true)
        {
            if (logException)
            {
                LogException(exception);
            }
            AddNotification(NotifyType.Error, exception.Message, persistForTheNextRequest);
        }
        /// <summary>
        /// Display notification
        /// </summary>
        /// <param name="type">Notification type</param>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected virtual void AddNotification(NotifyType type, string message, bool persistForTheNextRequest)
        {
            string dataKey = string.Format("cntrade.notifications.{0}", type);
            if (persistForTheNextRequest)
            {
                if (TempData[dataKey] == null)
                    TempData[dataKey] = new List<string>();
                ((List<string>)TempData[dataKey]).Add(message);
            }
            else
            {
                if (ViewData[dataKey] == null)
                    ViewData[dataKey] = new List<string>();
                ((List<string>)ViewData[dataKey]).Add(message);
            }
        }



        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<string> GetErrorsFromModelState()
        {
            if (ModelState.IsValid) return default(IEnumerable<string>);
            var errors = from state in ModelState.Values
                         from error in state.Errors
                         where !string.IsNullOrEmpty(error.ErrorMessage)
                         select error.ErrorMessage;
            return errors;

        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        protected string GetErrorMsgFromModelState()
        {
            return string.Join("<br/>", GetErrorsFromModelState());
        }
        /// <summary>
        ///     Redirects a request to the specified URL and specifies whether execution
        //     of the current process should terminate.
        /// </summary>
        /// <param name="url">The target location.</param>
        /// <param name="endResponse">true to terminate the current process.</param>
        protected void RedirectUrl(string url, bool endResponse)
        {
            this.Response.Clear();//这里是关键，清除在返回前已经设置好的标头信息，这样后面的跳转才不会报错
            this.Response.BufferOutput = true;//设置输出缓冲
            if (!this.Response.IsRequestBeingRedirected)//在跳转之前做判断,防止重复
            {
                this.Response.Redirect(url, endResponse);
            }
        }
        /// <summary>
        ///  创建一个重定向到指定的 URL 的 System.Web.Mvc.RedirectResult 对象。
        /// </summary>
        /// <param name="url"> 要重定向到的 URL。</param>
        /// <returns>重定向结果对象</returns>
        protected override RedirectResult Redirect(string url)
        {
            this.Response.Clear();//这里是关键，清除在返回前已经设置好的标头信息，这样后面的跳转才不会报错
            this.Response.BufferOutput = true;//设置输出缓冲
            var result = base.Redirect(url);
            //this.Response.End();
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public T GetSession<T>(HttpContextBase context, string key, Func<T> action)
        {
            var result = (T)context.Session[key];
            if (result == null && action != null)
            {
                result = action();
                context.Session[key] = result;
                return result;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetSession<T>(HttpContextBase context, string key, T value)
        {
            var result = (T)context.Session[key];
            if (result == null)
            {
                context.Session[key] = value;
            }
        }

        #region Exception handling
        protected override void OnException(ExceptionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            //If exception handled before, do nothing.
            //If this is child action, exception should be handled by main action.
            if (context.ExceptionHandled || context.IsChildAction)
            {
                base.OnException(context);
                return;
            }

            //// If custom errors are disabled, we need to let the normal ASP.NET exception handler
            //// execute so that the user can see useful debugging information.
            //if (!context.HttpContext.IsCustomErrorEnabled)
            //{
            //    base.OnException(context);
            //    return;
            //}

            // If this is not an HTTP 500 (for example, if somebody throws an HTTP 404 from an action method),
            // ignore it.
            if (new HttpException(null, context.Exception).GetHttpCode() != 500)
            {
                base.OnException(context);
                return;
            }

            //We handled the exception!
            context.ExceptionHandled = true;
            //Return an error response to the client.
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.StatusCode = GetStatusCodeForException(context);

            // Certain versions of IIS will sometimes use their own error page when
            // they detect a server error. Setting this property indicates that we
            // want it to try to render ASP.NET MVC's error page instead.
            context.HttpContext.Response.TrySkipIisCustomErrors = true;
            Exception exception = context.Exception;
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            // 错误日志编写    
            string controllerNamer = context.RouteData.Values["controller"].ToString();
            string actionName = context.RouteData.Values["action"].ToString();

            // 获得前一个异常的实例
            StringBuilder errorBuilder = new StringBuilder("Url：" + Request.Url + ",RawUrl：" + Request.RawUrl + ",UrlReferrer：" + Request.UrlReferrer);
            errorBuilder.Append("controller:" + controllerNamer + ",action:" + actionName);
            if (Request.ContentLength < 2 * 1024 * 1024)
            {
                string contentType = Request.ContentType;
                string[] contentTypeArray = new string[] { "application/json", "text/xml", "application/x-www-form-urlencoded", "application/xml" };
                if (Utils.InArray(contentType, contentTypeArray))
                {
                    var stream = Request.InputStream;
                    using (var streamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
                    {
                        errorBuilder.Append(",请求内容：" + streamReader.ReadToEnd());
                    }
                }
            }
            if (logger != null)
            {
                logger.Error(errorBuilder.ToString(), exception);
            }
            if (_currentMethodInfo != null)
            {
                context.Result = MethodInfoHelper.IsJsonResult(_currentMethodInfo)
                    ? GenerateJsonExceptionResult(context)
                    : GenerateNonJsonExceptionResult(context);

            }
            //Trigger an event, so we can register it.
            //EventBus.Trigger(this, new AbpHandledExceptionData(context.Exception));
        }
        protected virtual ActionResult GenerateJsonExceptionResult(ExceptionContext context)
        {
            context.HttpContext.Items.Add("IgnoreJsonRequestBehaviorDenyGet", "true");
            JsonResult result = new JsonResult();
            BaseJsonResult data = new BaseJsonResult()
            {
                Code = 0,
                Msg = "处理你的请求时出错。"
            };
            result.Data = data;
            return result;
        }

        protected virtual ActionResult GenerateNonJsonExceptionResult(ExceptionContext context)
        {
            ViewDataDictionary dictionary = new ViewDataDictionary();
            dictionary.Add("msg", "处理你的请求时出错。");
            return new ViewResult
            {
                ViewName = "Error",
                MasterName = string.Empty,
                ViewData= dictionary
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual int GetStatusCodeForException(ExceptionContext context)
        {
            return (int)HttpStatusCode.InternalServerError;
        }
        /// <summary>
        ///  异常处理
        /// </summary>
        protected virtual bool HandleException(Exception exception)
        {
            // 在出现未处理的错误时运行的代码
            if (exception is OperationCanceledException)
            {
                Server.ClearError();
                return false;
            }
            string url = Request.Url.AbsoluteUri;
            if (!string.IsNullOrEmpty(url) && url.Contains("UEditor"))
            {
                Server.ClearError();
                return false;
            }
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            // 获得前一个异常的实例
            StringBuilder errorBuilder = new StringBuilder("请求地址：" + url + ",原始请求地址：" + Request.RawUrl + ",原始请求地址：" + Request.UrlReferrer);
            if (Request.ContentLength <= 2 * 1024 * 1024)
            {
                try
                {
                    string contentType = Request.ContentType;
                    string[] contentTypeArray = new string[] { "application/json", "text/xml", "application/x-www-form-urlencoded", "application/xml" };
                    if (Utils.InArray(contentType, contentTypeArray))
                    {
                        var stream = Request.InputStream;
                        using (var streamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
                        {
                            errorBuilder.Append(",请求内容：" + streamReader.ReadToEnd());
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(errorBuilder.ToString(), ex);
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
                    Server.ClearError();
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
                    logger.Error(errorBuilder.ToString(), exception);
                }
                Server.ClearError();
            }
            return true;
        }
        #endregion   Exception handling
    }
}
