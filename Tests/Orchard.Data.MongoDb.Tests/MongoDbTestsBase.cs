using Autofac;
using MongoDB.Driver;
using NUnit.Framework;
using Orchard.Data.MongoDb.Tests.Entities;
using Orchard.Utility;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Orchard.Data.MongoDb.Tests
{

    public abstract class MongoDbTestsBase
    {
        protected IContainer _container;
        protected MongoDatabase DataBase;
     [OneTimeSetUp]
        public void InitFixture()
        {
        }

        [OneTimeTearDown]
        public void TearDownFixture()
        {

        }

        [SetUp]
        public virtual void Init()
        {
            string url = "mongodb://127.0.0.1:27017";
            MongoUrl monogoUrl = new MongoUrl(url);
            MongoServer mongodb = new MongoServer(MongoServerSettings.FromUrl(monogoUrl));//连接数据库
            DataBase = mongodb.GetDatabase("Test");//选择数据库名
            var builder = new ContainerBuilder();
            builder.Register(c=> DataBase);
            builder.RegisterModule(new MongoDbModule());
            Register(builder);
            _container = builder.Build();
        }

        [TearDown]
        public void Cleanup()
        {
            if (_container != null)
                _container.Dispose();
        }

        public abstract void Register(ContainerBuilder builder);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected AdmArea CreateAdmArea()
        {
            AdmArea entity = new AdmArea();
            entity.Id = RandomHelper.GetRandom(100000, 999999);
            entity.AreaID = RandomHelper.GetRandom(100000, 999999).ToString();// 地区ID
            entity.AreaName = "测试地区名称" + RandomHelper.GetRandom(100, 999);// 地区名称
            entity.ParentId = "100000";// 父级ID
            entity.ShortName = "地区简称";// 地区简称
            entity.LevelType = default(byte);// 地区类型(0:国家,1:直辖市或省份 2:市 3:区或者县)
            entity.CityCode = string.Empty;// 区号
            entity.ZipCode = string.Empty;// 邮编
            entity.AreaNamePath = "中国," + entity.AreaName;// 完整地区名称
            entity.AreaIDPath = entity.ParentId + "," + entity.AreaID;// 完整地区名称
            entity.Lng = default(decimal);// 经度
            entity.Lat = default(decimal);// 纬度
            entity.PinYin = default(string);// 拼音
            entity.ShortPinYin = string.Empty;// 拼音缩写
            entity.PYFirstLetter = string.Empty;// 拼音第一个字母
            entity.SortOrder = RandomHelper.GetRandom(1, 100);// 排序值
            entity.Status = default(byte);// 状态(1:启用,0:禁用)
            entity.CreateTime = DateTime.Now;// 创建时间
            entity.CreateUser = RandomHelper.GetRandom(1, 10000);// 创建人
            entity.UpdateTime = DateTime.Now;// 最后更新时间
            entity.UpdateUser = RandomHelper.GetRandom(1, 10000);// 最后更新人
            entity.DeleteTime = new DateTime(1900, 01, 01);
            entity.DeleteFlag = false;
            entity.DeleteUser = 0;
            return entity;
        }
    }
}
