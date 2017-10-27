using System;

namespace Orchard.Logging {
    public interface ILoggerFactory {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        ILogger GetLogger(Type type);

        /// <summary>
        /// Gets the logger.
        /// </summary>
        ILogger GetLogger(string typeName);
    }
}