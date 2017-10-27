using System;

namespace Orchard.Logging
{
    /// <summary>
    /// Creates a test Logger, that stores all log messages in a member list
    /// </summary>
	public class TestLogFactory : ILoggerFactory
    {
        private readonly bool debugEnabled;

        public TestLogFactory(bool debugEnabled = true)
        {
            this.debugEnabled = debugEnabled;
        }

        public ILogger GetLogger(Type type)
        {
            return new TestLogger(type) { IsDebugEnabled = debugEnabled };
        }

        public ILogger GetLogger(string typeName)
        {
            return new TestLogger(typeName) { IsDebugEnabled = debugEnabled };
        }
    }
}
