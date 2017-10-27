using Orchard.Logging;
using System;

namespace Orchard.Caching.Memcached.Logger
{
    public class EnyimLogFactoryWrapper : Enyim.Caching.ILogFactory 
    {
        public Enyim.Caching.ILog GetLogger(string name)
        {
            return new EnyimLoggerWrapper(LoggerManager.GetLogger(name));
        }

        public Enyim.Caching.ILog GetLogger(Type type)
        {
            return new EnyimLoggerWrapper(LoggerManager.GetLogger(type));
        }
    }
}
