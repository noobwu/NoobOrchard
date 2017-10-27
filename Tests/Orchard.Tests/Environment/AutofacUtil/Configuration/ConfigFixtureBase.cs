using Autofac;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Orchard.Environment.AutofacUtil;
using System;

namespace Orchard.Tests.Environment.AutofacUtil.Configuration
{
    [SetUpFixture]
    public class ConfigFixtureBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        protected IConfiguration LoadJson(string configFile)
        {
            configFile = "App_Data/Configs/Autofac/" + configFile;
            return EmbeddedConfiguration.LoadJson(configFile);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        protected  ContainerBuilder ConfigureContainerWithJson(string configFile)
        {
            configFile = "App_Data/Configs/Autofac/"+configFile;
            return EmbeddedConfiguration.ConfigureContainerWithJson(configFile);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        protected  ContainerBuilder ConfigureContainerWithXml(string configFile)
        {
            configFile = "App_Data/Configs/Autofac/" + configFile;
            return EmbeddedConfiguration.ConfigureContainerWithXml(configFile);
        }
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // TODO: Add code here that is run before
            //  all tests in the assembly are run            
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // TODO: Add code here that is run after
            //  all tests in the assembly have been run
        }
    }
}