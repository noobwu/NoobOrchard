using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace Orchard.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsFatal(this Exception ex)
        {
            return
                ex is StackOverflowException ||
                ex is OutOfMemoryException ||
                ex is AccessViolationException ||
                ex is AppDomainUnloadedException ||
                ex is ThreadAbortException ||
                ex is SecurityException ||
                ex is SEHException;
        }

        /// <summary>
        /// Uses <see cref="ExceptionDispatchInfo.Capture"/> method to re-throws exception
        /// while preserving stack trace.
        /// </summary>
        /// <param name="exception">Exception to be re-thrown</param>
        public static void ReThrow(this Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static Exception GetInnermostException(this Exception exception)
        {
            if (exception == null)
                return null;

            Exception current = exception;
            while (current.InnerException != null)
                current = current.InnerException;

            return current;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetMessage(this Exception exception)
        {
            if (exception == null)
                return String.Empty;

            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
                return String.Join(System.Environment.NewLine, aggregateException.Flatten().InnerExceptions.Where(ex => !String.IsNullOrEmpty(ex.GetInnermostException().Message)).Select(ex => ex.GetInnermostException().Message));

            return exception.GetInnermostException().Message;
        }
    }
}
