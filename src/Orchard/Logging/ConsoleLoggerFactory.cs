
using System;

namespace Orchard.Logging
{
    /// <summary>
    /// Creates a Console Logger, that logs all messages to: System.Console
    /// 
    /// Made public so its testable
    /// </summary>
    public class ConsoleLoggerFactory : ILoggerFactory
    {

        public ConsoleLoggerFactory()
        {
            
        }

        public ILogger GetLogger(Type type)
        {
            return new ConsoleLogger(type);
        }

        public ILogger GetLogger(string typeName)
        {
            return new ConsoleLogger(typeName);
        }
    }
}

