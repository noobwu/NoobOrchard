using Orchard.Utility;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Xml;
namespace Orchard.Environment.Configuration
{
    /// <summary>
    /// 基本设置描述类, 加[Serializable]标记为可序列化
    /// </summary>
    [Serializable]
    public class DbConfig : NameValueBasedConfig
    {
        //Data Source=127.0.0.1;User ID=root;Password=root;Port=3306;DataBase=wawifi;Allow Zero Datetime=true;Charset=gbk;Max Pool Size=10; Min Pool Size=5;pooling = true; Connection Timeout=300;
        /// <summary>
        /// 数据库连接串-格式(中文为用户修改的内容)：Data Source=数据库服务器地址;User ID=您的数据库用户名;Password=您的数据库用户密码;Initial Catalog=数据库名称;Pooling=true
        /// </summary>
        public string DbConnectString = "Data Source=;User ID=sa;Password=;Initial Catalog=;Pooling=true";
        /// <summary>
        ///  数据库中表的前缀
        /// </summary>
        public string TablePrefix = "cnt_";
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType = "";
        /// <summary>
        /// Sql语句执行最长时间
        /// </summary>
        public int CommandTimeout = 30;

        public DbConfig(string configPath, string sectionName) : base(configPath, sectionName)
        {
            Init();
        }
        private void Init()
        {
            CommandTimeout = Get("CommandTimeout",30);
            DbConnectString = Get("DbConnectString");
            DbType = Get("DbType");
            TablePrefix = Get("TablePrefix");
        }

    }
}
