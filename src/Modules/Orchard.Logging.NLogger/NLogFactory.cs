using NLog;
using NLog.Config;
using Orchard.Logging;
using Orchard.Utility;
using System;
using System.IO;

namespace Orchard.Logging.NLogger
{
    /// <summary>
    /// ILogFactory that creates an NLog ILog logger
    /// </summary>
    public class NLogFactory : ILoggerFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NLogFactory"/> class.
        /// </summary>
        public NLogFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogFactory"/> class.
        /// </summary>
        /// <param name="configFile">The NLog net configuration file to load and watch. If not found configures from App.Config.</param>
        public NLogFactory(string configFile)
        {
            var file = GetConfigFile(configFile);
            NLog.LogManager.Configuration = new XmlLoggingConfiguration(file.FullName);
        }
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ILogger GetLogger(Type type)
        {
            return new NLogLogger(type);
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public ILogger GetLogger(string typeName)
        {
            return new NLogLogger(typeName);
        }

        /// <summary>
        ///   Gets the configuration file.
        /// </summary>
        /// <param name = "fileName">i.e. log4net.config</param>
        /// <returns></returns>
        private  FileInfo GetConfigFile(string fileName)
        {
            FileInfo result;

            if (Path.IsPathRooted(fileName))
            {
                result = new FileInfo(fileName);
            }
            else
            {

                result = new FileInfo(Utils.GetMapPath(fileName));
            }

            return result;
        }
    }
}
