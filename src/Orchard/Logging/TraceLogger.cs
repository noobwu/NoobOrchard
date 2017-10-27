using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Logging
{

    /// <summary>
    ///   The TraceLogger sends all logging to the System.Diagnostics.TraceSource
    ///   built into the .net framework.
    /// </summary>
    /// <remarks>
    ///   Logging can be configured in the system.diagnostics configuration 
    ///   section. 
    /// 
    ///   If logger doesn't find a source name with a full match it will
    ///   use source names which match the namespace partially. For example you can
    ///   configure from all castle components by adding a source name with the
    ///   name "Castle". 
    /// 
    ///   If no portion of the namespace matches the source named "Default" will
    ///   be used.
    /// </remarks>
    public class TraceLogger
    {
        private static readonly Dictionary<string, TraceSource> cache = new Dictionary<string, TraceSource>();
        private TraceSource traceSource;
        public TraceLogger()
        {
            Initialize(LogLevel.Debug);
        }
        /// <summary>
        ///   A Common method to log.
        /// </summary>
        /// <param name = "loggerLevel">The level of logging</param>
        /// <param name = "message">The Message</param>
        private void Log(LogLevel loggerLevel, object message)
        {
            Log(loggerLevel, message);
        }
        /// <summary>
        ///   A Common method to log.
        /// </summary>
        /// <param name = "loggerLevel">The level of logging</param>
        /// <param name = "message">The Message</param>
        /// <param name = "exception">The Exception</param>
        private void Log(LogLevel loggerLevel, object message, Exception exception)
        {
            if (traceSource == null) return;
            string msg = string.Empty;
            if (message != null)
            {
                msg = message.ToString();
            }
            string loggerName = this.GetType().Name;
            if (exception == null)
            {
                traceSource.TraceEvent(MapTraceEventType(loggerLevel), 0, msg);
            }
            else
            {
                traceSource.TraceData(MapTraceEventType(loggerLevel), 0, msg, exception);
            }
        }
        /// <summary>
        /// Logs the format.
        /// </summary>
        private void LogFormat(LogLevel loggerLevel, string format, params object[] args)
        {
            LogFormat(loggerLevel, format, args);
        }
        /// <summary>
        /// Logs the format.
        /// </summary>
        private void LogFormat(LogLevel loggerLevel, Exception exception, string format, params object[] args)
        {
            if (traceSource == null) return;
            string msg = string.Format(format, args);
            string loggerName = this.GetType().Name;
            if (exception == null)
            {
                traceSource.TraceEvent(MapTraceEventType(loggerLevel), 0, msg);
            }
            else
            {
                traceSource.TraceData(MapTraceEventType(loggerLevel), 0, msg, exception);
            }
        }
        /// <summary>
        /// Logs a Debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(object message)
        {
            Log(LogLevel.Debug, message);
        }


        /// <summary>
        /// Logs a Debug message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(object message, Exception exception)
        {
            Log(LogLevel.Debug, message, exception);
        }

        /// <summary>
        /// Logs a Debug format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void DebugFormat(string format, params object[] args)
        {
            LogFormat(LogLevel.Debug, format, args);
        }

        /// <summary>
        /// Logs a Debug format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            LogFormat(LogLevel.Debug, exception, format, args);
        }
        /// <summary>
        /// Logs a Error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(object message)
        {
            Log(LogLevel.Error, message);
        }

        /// <summary>
        /// Logs a Error message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(object message, Exception exception)
        {
            Log(LogLevel.Error, message, exception);
        }

        /// <summary>
        /// Logs a Error format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void ErrorFormat(string format, params object[] args)
        {
            LogFormat(LogLevel.Error, format, args);
        }

        /// <summary>
        /// Logs a Error format message.
        /// </summary>
        /// <param name="exception">The exception. </param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            LogFormat(LogLevel.Error, exception, format, args);
        }
        /// <summary>
        /// Logs a Fatal message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Fatal(object message)
        {
            Log(LogLevel.Fatal, message);
        }

        /// <summary>
        /// Logs a Fatal message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(object message, Exception exception)
        {
            Log(LogLevel.Fatal, message, exception);
        }

        /// <summary>
        /// Logs a Fatal format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void FatalFormat(string format, params object[] args)
        {
            LogFormat(LogLevel.Fatal, format, args);
        }
        /// <summary>
        /// Logs a Fatal format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            LogFormat(LogLevel.Fatal, exception, format, args);
        }
        /// <summary>
        /// Logs an Info message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(object message)
        {
            Log(LogLevel.Info, message);
        }

        /// <summary>
        /// Logs an Info message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(object message, Exception exception)
        {
            Log(LogLevel.Info, message, exception);
        }

        /// <summary>
        /// Logs an Info format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void InfoFormat(string format, params object[] args)
        {
            LogFormat(LogLevel.Info, format, args);
        }

        /// <summary>
        /// Logs an Info format message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            LogFormat(LogLevel.Info, exception, format, args);
        }

        /// <summary>
        /// Logs a Warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(object message)
        {
            Log(LogLevel.Warn, message);
        }

        /// <summary>
        /// Logs a Warning message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(object message, Exception exception)
        {
            Log(LogLevel.Warn, message, exception);
        }

        /// <summary>
        /// Logs a Warning format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void WarnFormat(string format, params object[] args)
        {
            LogFormat(LogLevel.Warn, format, args);
        }
        /// <summary>
        /// Logs a Warning format message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            LogFormat(LogLevel.Warn, exception, format, args);
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is log enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is log enabled{} otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled(LogLevel level)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        private void Initialize(LogLevel logLevel)
        {
            var loggerName = GetType().Name;
            lock (cache)
            {
                // because TraceSource is meant to be used as a static member, and because
                // building up the configuraion inheritance is non-trivial, the instances
                // themselves are cached for so multiple TraceLogger instances will reuse
                // the named TraceSources which have been created

                if (cache.TryGetValue(loggerName, out traceSource))
                {
                    return;
                }

                var defaultLevel = MapSourceLevels(logLevel);
                traceSource = new TraceSource(loggerName, defaultLevel);

                // no further action necessary when the named source is configured
                if (IsSourceConfigured(traceSource))
                {
                    cache.Add(loggerName, traceSource);
                    return;
                }

                // otherwise hunt for a shorter source that been configured            
                var foundSource = new TraceSource("Default", defaultLevel);

                var searchName = ShortenName(loggerName);
                while (!string.IsNullOrEmpty(searchName))
                {
                    var searchSource = new TraceSource(searchName, defaultLevel);
                    if (IsSourceConfigured(searchSource))
                    {
                        foundSource = searchSource;
                        break;
                    }

                    searchName = ShortenName(searchName);
                }

                // reconfigure the created source to act like the found source
                traceSource.Switch = foundSource.Switch;
                traceSource.Listeners.Clear();
                foreach (TraceListener listener in foundSource.Listeners)
                {
                    traceSource.Listeners.Add(listener);
                }

                cache.Add(loggerName, traceSource);
            }
        }

        private static string ShortenName(string name)
        {
            var lastDot = name.LastIndexOf('.');
            if (lastDot != -1)
            {
                return name.Substring(0, lastDot);
            }
            return null;
        }

        private static bool IsSourceConfigured(TraceSource source)
        {
            if (source.Listeners.Count == 1 &&
                source.Listeners[0] is DefaultTraceListener &&
                source.Listeners[0].Name == "Default")
            {
                return false;
            }
            return true;
        }

        private static LogLevel MapLoggerLevel(SourceLevels level)
        {
            switch (level)
            {
                case SourceLevels.All:
                    return LogLevel.Debug;
                case SourceLevels.Verbose:
                    return LogLevel.Debug;
                case SourceLevels.Information:
                    return LogLevel.Info;
                case SourceLevels.Warning:
                    return LogLevel.Warn;
                case SourceLevels.Error:
                    return LogLevel.Error;
                case SourceLevels.Critical:
                    return LogLevel.Fatal;
                 default:
                    return LogLevel.Info;

            }
        }

        private static SourceLevels MapSourceLevels(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return SourceLevels.Verbose;
                case LogLevel.Info:
                    return SourceLevels.Information;
                case LogLevel.Warn:
                    return SourceLevels.Warning;
                case LogLevel.Error:
                    return SourceLevels.Error;
                case LogLevel.Fatal:
                    return SourceLevels.Critical;
            }
            return SourceLevels.Off;
        }

        private static TraceEventType MapTraceEventType(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return TraceEventType.Verbose;
                case LogLevel.Info:
                    return TraceEventType.Information;
                case LogLevel.Warn:
                    return TraceEventType.Warning;
                case LogLevel.Error:
                    return TraceEventType.Error;
                case LogLevel.Fatal:
                    return TraceEventType.Critical;
            }
            return TraceEventType.Verbose;
        }
    }
}
