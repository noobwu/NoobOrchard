using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Logging
{
    /// <summary>
    /// The Logger sending everything to the standard output streams.
    /// This is mainly for the cases when you have a utility that
    /// does not have a logger to supply.
    /// </summary>
    public class ConsoleLogger : ILogger
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLogger"/> class.
        /// </summary>
        public ConsoleLogger(string type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLogger"/> class.
        /// </summary>
		public ConsoleLogger(Type type)
        {
        }
        /// <summary>
        ///   A Common method to log.
        /// </summary>
        /// <param name = "loggerLevel">The level of logging</param>
        /// <param name = "message">The Message</param>
        private void Log(LogLevel loggerLevel, object message)
        {
            Log(loggerLevel, message, null);
        }
        /// <summary>
        ///   A Common method to log.
        /// </summary>
        /// <param name = "loggerLevel">The level of logging</param>
        /// <param name = "message">The Message</param>
        /// <param name = "exception">The Exception</param>
        private void Log(LogLevel loggerLevel, object message, Exception exception)
        {
            try
            {
                string msg = string.Empty;
                if (message != null)
                {
                    msg = message.ToString();
                }
                string loggerName = this.GetType().Name;
                Console.Out.WriteLine("[{0}] '{1}' {2}", loggerLevel, loggerName, msg);

                if (exception != null)
                {
                    Console.Out.WriteLine("[{0}] '{1}' {2}: {3}", loggerLevel, loggerName, exception.GetType().FullName,
                                         exception);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        /// <summary>
        /// Logs the format.
        /// </summary>
        private void LogFormat(LogLevel loggerLevel, string format, params object[] args)
        {
            LogFormat(loggerLevel, null, format, args);
        }
        /// <summary>
        /// Logs the format.
        /// </summary>
        private void LogFormat(LogLevel loggerLevel, Exception exception, string format, params object[] args)
        {
            try
            {
                string message = string.Format(format, args);
                string loggerName = this.GetType().Name;
                Console.Out.WriteLine("[{0}] '{1}' {2}", loggerLevel, loggerName, message);

                if (exception != null)
                {
                    Console.Out.WriteLine("[{0}] '{1}' {2}: {3}", loggerLevel, loggerName, exception.GetType().FullName,
                                          exception);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
    }
}
