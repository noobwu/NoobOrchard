using Autofac;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Orchard.Data.EntityFrameworkCore.Tests.Entities;
using Orchard.Tests.Common.Domain.Entities;
using Orchard.Utility;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace Orchard.Data.EntityFrameworkCore.Tests
{

    public abstract class EfCoreNUnitTestBase:EfCoreTestBase
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
            base.Cleanup();
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
        /// <returns></returns>
        protected AdmAreaTest CreateAdmAreaTest()
        {
            AdmAreaTest entity = new AdmAreaTest()
            {
                AreaId = RandomData.GetString(maxLength: 50),//AreaID
                AreaName = RandomData.GetString(maxLength: 50),//AreaName
                ParentId = RandomData.GetString(maxLength: 50),//ParentId
                ShortName = RandomData.GetString(maxLength: 50),//ShortName
                LevelType = default(byte),//LevelType
                CityCode = RandomData.GetString(maxLength: 50),//CityCode
                ZipCode = RandomData.GetString(maxLength: 50),//ZipCode
                AreaNamePath = RandomData.GetString(maxLength: 500),//AreaNamePath
                AreaIdPath = RandomData.GetString(maxLength: 500),//AreaIDPath
                Lng = RandomData.GetDecimal(-(int)Math.Pow(2, 18), (int)Math.Pow(2, 18)),//lng
                Lat = RandomData.GetDecimal(-(int)Math.Pow(2, 18), (int)Math.Pow(2, 18)),//Lat
                PinYin = RandomData.GetString(maxLength: 50),//PinYin
                ShortPinYin = RandomData.GetString(maxLength: 20),//ShortPinYin
                PYFirstLetter = RandomData.GetString(maxLength: 10),//PYFirstLetter
                SortOrder = RandomData.GetInt(),//SortOrder
                Status = default(byte),//Status
                CreateTime = DateTime.Now,//CreateTime
                CreateUser = RandomData.GetInt(),//CreateUser
                UpdateTime = RandomData.GetDateTime(),//UpdateTime
                UpdateUser = RandomData.GetInt(),//UpdateUser
                DeleteFlag = false,//DeleteFlag
                DeleteTime = RandomData.GetDateTime(),//DeleteTime
                DeleteUser = RandomData.GetInt(),//DeleteUser
            };
            return entity;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orderByExpressions"></param>
        /// <returns></returns>
        protected IQueryable<TEntity> ApplyOrderBy<TEntity>(IQueryable<TEntity> query,
params IOrderByExpression<TEntity>[] orderByExpressions)
            where TEntity : class
        {
            if (orderByExpressions == null || orderByExpressions.Count() == 0)
                return query;

            IOrderedQueryable<TEntity> output = null;
            foreach (var orderByExpression in orderByExpressions)
            {
                if (output == null)
                {
                    output = orderByExpression.ApplyOrderBy(query);
                }
                else
                {
                    output = orderByExpression.ApplyThenBy(output);
                }
            }
            return output ?? query;
        }
    }
}
