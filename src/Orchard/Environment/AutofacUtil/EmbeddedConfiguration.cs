using Autofac;
using Autofac.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Xml;
using Microsoft.Extensions.Configuration.Json;
using System.Collections.Generic;
using System.IO;

namespace Orchard.Environment.AutofacUtil
{
    /// <summary>
    /// 
    /// </summary>
    public static class EmbeddedConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ContainerBuilder ConfigureContainer(IConfiguration configuration)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigurationModule(configuration));
            return builder;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        public static ContainerBuilder ConfigureContainerWithJson(string configFile)
        {
            return ConfigureContainer(LoadJson(configFile));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        public static ContainerBuilder ConfigureContainerWithXml(string configFile)
        {
            return ConfigureContainer(LoadXml(configFile));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        public static Module ConfigureModuleWithJson(string configFile)
        {
            return new ConfigurationModule(LoadJson(configFile));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        public static Module ConfigureModuleWithXml(string configFile)
        {
            return new ConfigurationModule(LoadXml(configFile));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        public static IConfiguration LoadJson(string configFile)
        {
            using (var stream = GetEmbeddedFileStream(configFile))
            {
                var provider = new EmbeddedConfigurationProvider<JsonConfigurationSource>(stream);
                var config = new ConfigurationRoot(new List<IConfigurationProvider> { provider });
                return config;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        public static IConfiguration LoadXml(string configFile)
        {
            using (var stream = GetEmbeddedFileStream(configFile))
            {
                var provider = new EmbeddedConfigurationProvider<XmlConfigurationSource>(stream);
                var config = new ConfigurationRoot(new List<IConfigurationProvider> { provider });
                return config;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        private static Stream GetEmbeddedFileStream(string configFile)
        {
            string filePath = configFile;
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Utility.Utils.GetMapPath(configFile);
            }
            return  new FileStream(filePath, FileMode.Open);
        }
    }
}
