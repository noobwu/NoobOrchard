#if !NETFX_CORE
using System;
using System.Text;

namespace Orchard.Logging
{
    /// <summary>
    /// StringBuilderLog writes to shared StringBuffer.
    /// Made public so its testable
    /// </summary>
    public class StringBuilderLogFactory : ILoggerFactory
    {
        private StringBuilder sb;
        private readonly bool debugEnabled;

        public StringBuilderLogFactory(bool debugEnabled = true)
        {
            sb = new StringBuilder();
            this.debugEnabled = debugEnabled;
        }

        public ILogger GetLogger(Type type)
        {
            return new StringBuilderLogger(type, sb) { IsDebugEnabled = debugEnabled };
        }

        public ILogger GetLogger(string typeName)
        {
            return new StringBuilderLogger(typeName, sb) { IsDebugEnabled = debugEnabled };
        }

        public string GetLogs()
        {
            lock (sb)
                return sb.ToString();
        }

        public void ClearLogs()
        {
            lock (sb)
                sb.Remove(0, sb.Length - 1);
        }
    }

    public class StringBuilderLogger : ILogger
    {
        const string DEBUG = "DEBUG: ";
        const string ERROR = "ERROR: ";
        const string FATAL = "FATAL: ";
        const string INFO = "INFO: ";
        const string WARN = "WARN: ";
        private readonly StringBuilder logs;

        public StringBuilderLogger(string type, StringBuilder logs)
        {
            this.logs = logs;
        }

        public StringBuilderLogger(Type type, StringBuilder logs)
        {
            this.logs = logs;
        }

        public bool IsDebugEnabled { get; set; }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        private void Log(object message, Exception exception)
        {
            string msg = message == null ? string.Empty : message.ToString();
            if (exception != null)
            {
                msg += ", Exception: " + exception.Message;
            }
            lock (logs)
                logs.AppendLine(msg);
        }

        /// <summary>
        /// Logs the format.
        /// </summary>
        private void LogFormat(string message, params object[] args)
        {
            string msg = message == null ? string.Empty : message.ToString();
            lock (logs)
            {
                logs.AppendFormat(msg, args);
                logs.AppendLine();
            }
        }
        /// <summary>
        /// Logs the format.
        /// </summary>
        private void LogFormat(Exception exception, string message, params object[] args)
        {
            string msg = message == null ? string.Empty : message.ToString();
            lock (logs)
            {
                logs.AppendFormat(msg, args);
                if (exception != null)
                {
                    logs.Append(", Exception: " + exception.Message);
                }
                logs.AppendLine();
            }
        }
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void Log(object message)
        {
            string msg = message == null ? string.Empty : message.ToString();
            lock (logs)
            {
                logs.AppendLine(msg);
            }
        }

        public void Debug(object message, Exception exception)
        {
            Log(DEBUG + message, exception);
        }

        public void Debug(object message)
        {
            Log(DEBUG + message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            LogFormat(DEBUG + format, args);
        }
        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            Log(DEBUG + string.Format(format,args), exception);
        }

        public void Error(object message, Exception exception)
        {
            Log(ERROR + message, exception);
        }

        public void Error(object message)
        {
            Log(ERROR + message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            LogFormat(ERROR + format, args);
        }
        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            LogFormat(exception,ERROR + format, args);
        }

        public void Fatal(object message, Exception exception)
        {
            Log(FATAL + message, exception);
        }

        public void Fatal(object message)
        {
            Log(FATAL + message);
        }

        public void FatalFormat(string format, params object[] args)
        {
            LogFormat(FATAL + format, args);
        }

        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            Log(FATAL + string.Format(format,args),exception);
        }
        public void Info(object message, Exception exception)
        {
            Log(INFO + message, exception);
        }

        public void Info(object message)
        {
            Log(INFO + message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            LogFormat(INFO + format, args);
        }
        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            LogFormat(exception,INFO + format, args);
        }
        public void Warn(object message, Exception exception)
        {
            Log(WARN + message, exception);
        }

        public void Warn(object message)
        {
            Log(WARN + message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            LogFormat(WARN + format, args);
        }


        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            LogFormat(exception,WARN + format, args);
        }

        public bool IsEnabled(LogLevel level)
        {
            return true;
        }

      
    }
}
#endif
