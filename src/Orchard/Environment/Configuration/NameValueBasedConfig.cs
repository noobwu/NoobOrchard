using Orchard.Exceptions;
using Orchard.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Orchard.Environment.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NameValueBasedConfig
    {
        private string _configPath;
        private string _sectionName;
        private NameValueCollection _nameValues;
        public NameValueBasedConfig(string configPath, string sectionName)
        {
            _configPath = configPath;
            _sectionName = sectionName;
            Init();
        }

        /// <summary>
        /// Gets/sets a config value.
        /// Returns null if no config with given name.
        /// </summary>
        /// <param name="name">Name of the config</param>
        /// <returns>Value of the config</returns>
        private string this[string name]
        {
            get {
                if (_nameValues == null || _nameValues.Count == 0)
                {
                    return null;
                }
               return _nameValues.Get(name);
            }
        }
        /// <summary>
        /// Gets a configuration value as a specific type.
        /// </summary>
        /// <param name="name">Name of the config</param>
        /// <typeparam name="T">Type of the config</typeparam>
        /// <returns>Value of the configuration or null if not found</returns>
        public T Get<T>(string name,T defaultVal=default(T))
        {
            var value = this[name];
            return value == null
                ? defaultVal
                : (T)Convert.ChangeType(value, typeof(T));
        }
        /// <summary>
        /// Gets a configuration value as a specific type.
        /// </summary>
        /// <param name="name">Name of the config</param>
        /// <returns>Value of the configuration or null if not found</returns>
        public string Get(string name)
        {
            return this[name];
        }
        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            string configFullPath = Utils.GetMapPath(_configPath);
            if (!File.Exists(configFullPath))
            {
                throw GetInitException("file  not  exist");
            }
            //configPath ="PayConfigs.config";
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = configFullPath;
            var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

            ConfigurationSection configSection = null;
            configSection = config.GetSection(_sectionName);
            if (configSection == null)
            {
                throw GetInitException("configSection  is  null");

            }
            string configRawXml = configSection.SectionInformation.GetRawXml();
            XmlDocument sectionXmlDoc = new XmlDocument();
            sectionXmlDoc.Load(new StringReader(configRawXml));
            if (sectionXmlDoc == null)
            {
                throw GetInitException("configRawXml  is  null");
            }
            NameValueSectionHandler handler = new NameValueSectionHandler();
            _nameValues = handler.Create(null, null, sectionXmlDoc.DocumentElement) as NameValueCollection;
            if (_nameValues == null || _nameValues.Count == 0)
            {
                throw GetInitException("nameValues is null");
            }
        }
        private InitializationException GetInitException(string errMsg)
        {
            return new InitializationException("NameValueBasedConfig Init Error,msg:" + errMsg + ",configPath:" + _configPath + ",sectionName:" + _sectionName);

        }
    }
}
