using Autofac;
using Orchard.Collections;
using Orchard.Validation;
using System;
using System.Linq;

namespace Orchard.Logging
{
    /// <summary>
    /// This class can be used to write logs from somewhere where it's a hard to get a reference to the <see cref="ILogger"/>.
    /// Normally, use <see cref="ILogger"/> with property injection wherever it's possible.
    /// </summary>
    public static class LogHelper
    {
        /// <summary>
        /// A reference to the logger.
        /// </summary>
        public static ILogger Logger { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        static LogHelper()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();
            Logger = container.IsRegistered(typeof(ILoggerFactory))
                ? container.Resolve<ILoggerFactory>().GetLogger(typeof(LogHelper))
                : NullLogger.Instance;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public static void LogException(Exception ex)
        {
            LogException(Logger, ex);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        public static void LogException(ILogger logger, Exception ex)
        {
            logger.Error(null,ex);
            LogValidationErrors(logger, ex);
        }

        private static void LogValidationErrors(ILogger logger, Exception exception)
        {
            //Try to find inner validation exception
            if (exception is AggregateException && exception.InnerException != null)
            {
                var aggException = exception as AggregateException;
                if (aggException.InnerException != null)
                {
                    exception = aggException.InnerException;
                }
            }

            if (!(exception is DefaultValidationException))
            {
                return;
            }

            var validationException = exception as DefaultValidationException;
            if (validationException.ValidationErrors.IsNullOrEmpty())
            {
                return;
            }

            logger.Error("There are " + validationException.ValidationErrors.Count + " validation errors:");
            foreach (var validationResult in validationException.ValidationErrors)
            {
                var memberNames = "";
                if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                {
                    memberNames = " (" + string.Join(", ", validationResult.MemberNames) + ")";
                }

                logger.Error(validationResult.ErrorMessage + memberNames);
            }
        }
    }
}
