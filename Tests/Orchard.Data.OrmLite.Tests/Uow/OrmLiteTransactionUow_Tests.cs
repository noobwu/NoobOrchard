using Autofac;
using NUnit.Framework;
using Orchard.Data.OrmLite.Tests.Entities;
using Orchard.Data.OrmLite.Uow;
using Orchard.Domain.Uow;
using Orchard.Domain.Uow.Transaction;
using Orchard.Tests.Common.Domain.Entities;
using Orchard.Utility;
using ServiceStack.OrmLite;
using System;
using System.Data;

namespace Orchard.Data.OrmLite.Tests.Uow
{
    [TestFixture]
    public class OrmLiteTransactionUow_Tests : OrmLiteNUnitTestBase
    {
        protected override void Register(ContainerBuilder builder)
        {
            builder.RegisterType<OrmLiteTransactionUnitOfWork>().As<ITransactionUnitOfWork<IDbConnection>>();
            builder.RegisterType<OrmLiteTransactionCompleteHandle>().As<ITransactionCompleteHandle>();
        }
        protected override void Init()
        {
            base.Init();

        }
        [Test]
        public void ITransactionCompleteHandle_Commit_Test()
        {
            // TODO: Add your test code here
            var transactionUnitOfWork = Container.Resolve<ITransactionUnitOfWork<IDbConnection>>();
            var entity = CreateAdmArea();
            var connection = DbFactory.OpenDbConnection();
            using (var transHandle= transactionUnitOfWork.Begin(connection))
            {
                var insertResult = connection.Insert(entity, entity.IsTransient());
                Console.WriteLine("insertResult:" + insertResult);
                var deleteResult = connection.Delete<AdmArea>(a => a.Id == insertResult);
                Console.WriteLine("deleteResult:" + deleteResult);
                transHandle.Commit();
            }
        }
        [Test]
        public void ITransactionCompleteHandle_Rollback_Test()
        {
            // TODO: Add your test code here
            var transactionUnitOfWork = Container.Resolve<ITransactionUnitOfWork<IDbConnection>>();
            var entity = CreateAdmArea();
            var connection = DbFactory.OpenDbConnection();
            using (var transHandle = transactionUnitOfWork.Begin(connection))
            {
                var insertResult = connection.Insert(entity, entity.IsTransient());
                Console.WriteLine("insertResult:" + insertResult);
                //var deleteResult = connection.Delete<AdmArea>(a => a.Id == insertResult);
                //Console.WriteLine("deleteResult:" + deleteResult);
                transHandle.Rollback();
            }
        }

        // <summary>
        /// 添加文章
        /// </summary>
        /// <param name="name"></param>
        [UowTransaction]
        public void UowTransaction_Test(string name)
        {
            var entity = CreateAdmAreaTest();
            var connection = DbFactory.OpenDbConnection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private AdmAreaTest CreateAdmAreaTest()
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
                CreateTime = RandomData.GetDateTime(),//CreateTime
                CreateUser = RandomData.GetInt(),//CreateUser
                UpdateTime = RandomData.GetDateTime(),//UpdateTime
                UpdateUser = RandomData.GetInt(),//UpdateUser
                DeleteFlag = false,//DeleteFlag
                DeleteTime = RandomData.GetDateTime(),//DeleteTime
                DeleteUser = RandomData.GetInt(),//DeleteUser
            };
            return entity;
        }
    }
}
