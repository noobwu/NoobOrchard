using System.Threading.Tasks;
using System.Web.Http.Filters;
using Orchard.Logging;

namespace Orchard.Web.WebApi.Filters {
    public class UnhandledApiExceptionFilter : ExceptionFilterAttribute, IApiFilterProvider {
        public UnhandledApiExceptionFilter() {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public override void OnException(HttpActionExecutedContext actionExecutedContext) {
            if (actionExecutedContext.Exception is TaskCanceledException)
                Logger.Warn("A pending API operation was canceled by the client.",actionExecutedContext.Exception);
            else
                Logger.Error("An unhandled exception was thrown in an API operation.", actionExecutedContext.Exception);
        }
    }
}
