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
    public class DbConfig_Tests
    {
        private readonly DbConfig _config;

        public DbConfig_Tests()
        {
            _config = new DbConfig("/App_Data/Configs/GeneralConfigs.config", "GeneralConfigs/DbConfig");
        }
        [Test]
        public void DbConfig_Should_Get_Value_Test()
        {
            _config.CommandTimeout.ShouldBe(30);
            _config.DbConnectString.ShouldBe("Data Source=.;Database=CNTrade; User=sa;Password=123456; Pooling=True; Max Pool Size=50;Connect Timeout=120;MultipleActiveResultSets=True;Max Pool Size=10; Min Pool Size=5;pooling = true;");
            _config.DbType.ShouldBe("SQLServer");
            _config.TablePrefix.ShouldBe("tb_");
        }
    }
}
