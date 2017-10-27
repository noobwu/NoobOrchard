using Orchard.Logging;
using System;
using NUnit.Framework;
namespace Orchard.Tests {
    public abstract class TestWithLoggingBase {
        protected  ILogger _logger;
        public  TestWithLoggingBase()
        {
            //LogFactory = new NullLoggerFactory();
            LogFactory = new ConsoleLoggerFactory();
            _logger = LogFactory.GetLogger(GetType());
        }

        protected ILoggerFactory LogFactory { get; set; }
    }
}