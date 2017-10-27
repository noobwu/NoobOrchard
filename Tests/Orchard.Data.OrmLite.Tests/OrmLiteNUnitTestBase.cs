using Autofac;
using NUnit.Framework;
using Orchard.Data.OrmLite.Tests.Entities;
using Orchard.Utility;
using ServiceStack;
using ServiceStack.DataAnnotations;
using ServiceStack.Logging;
using ServiceStack.OrmLite;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace Orchard.Data.OrmLite.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class OrmLiteNUnitTestBase : OrmLiteTestBase
    {
        [OneTimeSetUp]
        public void InitFixture()
        {
        }

        [OneTimeTearDown]
        public void TearDownFixture()
        {

        }

        [SetUp]
        protected override void Init()
        {
            base.Init();
        }

        [TearDown]
        protected override void Cleanup()
        {
            base.Init();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected AdmArea CreateAdmArea()
        {
            AdmArea entity = new AdmArea() { };
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orderByExpressions"></param>
        /// <returns></returns>
        protected SqlExpression<TEntity> ApplySqlExpressionOrderBy<TEntity>(SqlExpression<TEntity> query,
params IOrderByExpression<TEntity>[] orderByExpressions) where TEntity : class
        {
            if (orderByExpressions == null || orderByExpressions.Count() == 0)
                return query;
            SqlExpression<TEntity> output = null;
            foreach (var orderByExpression in orderByExpressions)
            {
                if (output == null)
                {
                    output = orderByExpression.ApplyCustomOrderBy(query);
                }
                else
                {
                    output = orderByExpression.ApplyCustomThenBy(output);
                }
            }
            return output ?? query;
        }
    }
}
