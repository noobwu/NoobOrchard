using NUnit.Framework;
using Orchard.Environment.Configuration;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Tests.Environment.Configuration
{
    [TestFixture]
    public class WebConfig_Tests
    {
        private readonly WebConfig _config;

        public WebConfig_Tests()
        {
            _config = new WebConfig("/App_Data/Configs/GeneralConfigs.config", "GeneralConfigs/WebConfig");
        }
        [Test]
        public void WebConfig_Should_Get_Value_Test()
        {
            _config.WebTitle.ShouldBe("互联网站内容管理系统");
        }
    }
}
