using log4net.Core;
using log4net.Util;
using System;
using System.Globalization;

namespace Orchard.Logging.Log4Net
{
    /// <summary>
    /// Wrapper over the log4net.1.2.10 and above logger 
    /// </summary>
	public class Log4NetLogger : ILogger
    {
        private static readonly Type declaringType = typeof(Log4NetLogger);
        private readonly log4net.ILog _logger;

        public Log4NetLogger(string typeName)
        {
            _logger = log4net.LogManager.GetLogger(typeName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLogger"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public Log4NetLogger(Type type)
        {
            _logger = log4net.LogManager.GetLogger(type);
        }

        public bool IsDebugEnabled { get { return _logger.IsDebugEnabled; } }

        /// <summary>
        /// Logs a Debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(object message)
        {
            if (_logger.IsDebugEnabled)
                _logger.Debug(message);
        }


        /// <summary>
        /// Logs a Debug message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(object message, Exception exception)
        {
            if (_logger.IsDebugEnabled)
                _logger.Debug(message, exception);
        }

        /// <summary>
        /// Logs a Debug format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void DebugFormat(string format, params object[] args)
        {
            if (_logger.IsDebugEnabled)
                _logger.DebugFormat(format, args);
        }
        /// <summary>
        /// Logs a  Debug format message.
        /// </summary>
        /// <param name="exception">The exception. </param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            if (_logger.IsErrorEnabled)
            {
                _logger.Logger.Log(declaringType, Level.Debug, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
            }
        }
        /// <summary>
        /// Logs a Error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(object message)
        {
            if (_logger.IsErrorEnabled)
                _logger.Error(message);
        }

        /// <summary>
        /// Logs a Error message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(object message, Exception exception)
        {
            if (_logger.IsErrorEnabled)
                _logger.Error(message, exception);
        }

        /// <summary>
        /// Logs a Error format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void ErrorFormat(string format, params object[] args)
        {
            if (_logger.IsErrorEnabled)
                _logger.ErrorFormat(format, args);
        }
        /// <summary>
        /// Logs a Error format message.
        /// </summary>
        /// <param name="exception">The exception. </param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            if (_logger.IsErrorEnabled)
            {
                _logger.Logger.Log(declaringType, Level.Error, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
            }
        }

        /// <summary>
        /// Logs a Fatal message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Fatal(object message)
        {
            if (_logger.IsFatalEnabled)
                _logger.Fatal(message);
        }

        /// <summary>
        /// Logs a Fatal message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(object message, Exception exception)
        {
            if (_logger.IsFatalEnabled)
                _logger.Fatal(message, exception);
        }

        /// <summary>
        /// Logs a Error format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void FatalFormat(string format, params object[] args)
        {
            if (_logger.IsFatalEnabled)
                _logger.FatalFormat(format, args);
        }
        /// <summary>
        /// Logs a Warning format message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            if (_logger.IsFatalEnabled)
                _logger.Logger.Log(declaringType, Level.Fatal, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
        }

        /// <summary>
        /// Logs an Info message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(object message)
        {
            if (_logger.IsInfoEnabled)
                _logger.Info(message);
        }

        /// <summary>
        /// Logs an Info message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(object message, Exception exception)
        {
            if (_logger.IsInfoEnabled)
                _logger.Info(message, exception);
        }

        /// <summary>
        /// Logs an Info format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void InfoFormat(string format, params object[] args)
        {
            if (_logger.IsInfoEnabled)
                _logger.InfoFormat(format, args);
        }

        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            if (_logger.IsInfoEnabled)
            {
                _logger.Logger.Log(declaringType, Level.Info, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
            }
        }

        /// <summary>
        /// Logs a Warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(object message)
        {
            if (_logger.IsWarnEnabled)
                _logger.Warn(message);
        }

        /// <summary>
        /// Logs a Warning message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(object message, Exception exception)
        {
            if (_logger.IsWarnEnabled)
                _logger.Warn(message, exception);
        }

        /// <summary>
        /// Logs a Warning format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void WarnFormat(string format, params object[] args)
        {
            if (_logger.IsWarnEnabled)
                _logger.WarnFormat(format, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            if (_logger.IsWarnEnabled)
            {
                _logger.Logger.Log(declaringType, Level.Warn, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return _logger.IsDebugEnabled;
                case LogLevel.Info:
                    return _logger.IsInfoEnabled;
                case LogLevel.Warn:
                    return _logger.IsWarnEnabled;
                case LogLevel.Error:
                    return _logger.IsErrorEnabled;
                case LogLevel.Fatal:
                    return _logger.IsFatalEnabled;
            }
            return false;
        }
    }
}