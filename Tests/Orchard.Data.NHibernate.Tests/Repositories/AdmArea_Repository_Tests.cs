using Autofac;
using NUnit.Framework;
using Orchard.Data.NHibernate.Tests.Entities;
using Orchard.Utility;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Orchard.Data.NHibernate.Tests.Repositories
{
    public class AdmArea_Repository_Tests : NHibernateTestsBase
    {
        public override void Register(ContainerBuilder builder)
        {

        }
        private Domain.Repositories.IRepository<AdmArea> _admAreaRepository;

        public override void Init()
        {
            base.Init();
            _admAreaRepository = _container.Resolve<Domain.Repositories.IRepository<AdmArea>>();
        }

        #region Select/Get/Query Tests

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        //[Theory]
        [TestCase(2)]
        public void AdmArea_Single_Test(int id)
        {
            //exec sp_executesql N'SELECT admarea0_.Id as Id1_0_0_, admarea0_.AreaID as AreaID2_0_0_, admarea0_.AreaName as AreaNa3_0_0_, admarea0_.ParentId as Parent4_0_0_, admarea0_.ShortName as ShortN5_0_0_, admarea0_.LevelType as LevelT6_0_0_, admarea0_.CityCode as CityCo7_0_0_, admarea0_.ZipCode as ZipCod8_0_0_, admarea0_.AreaNamePath as AreaNa9_0_0_, admarea0_.AreaIDPath as AreaI10_0_0_, admarea0_.Lng as Lng11_0_0_, admarea0_.Lat as Lat12_0_0_, admarea0_.PinYin as PinYi13_0_0_, admarea0_.ShortPinYin as Short14_0_0_, admarea0_.PYFirstLetter as PYFir15_0_0_, admarea0_.SortOrder as SortO16_0_0_, admarea0_.Status as Statu17_0_0_, admarea0_.CreateTime as Creat18_0_0_, admarea0_.CreateUser as Creat19_0_0_, admarea0_.UpdateTime as Updat20_0_0_, admarea0_.UpdateUser as Updat21_0_0_ FROM wt_adm_area admarea0_ WHERE admarea0_.Id=@p0',N'@p0 int',@p0=1
            var admArea = _admAreaRepository.Single(id);
            Assert.AreEqual(admArea.Id, id);
            Console.WriteLine(admArea.AreaID);
            // Console.WriteLine(JsonSerializationHelper.SerializeByDefault(admArea));
        }

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        //[Theory]
        [TestCase(2)]
        public void AdmArea_SingleAsync_Test(int id)
        {
            var admArea = _admAreaRepository.SingleAsync(id).Result;
            Assert.AreEqual(admArea.Id, id);
        }

        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// </summary>
        /// <param name="predicate">Entity</param>
        [TestCase(3)]
        public void AdmArea_Single_Predicate_Test(int id)
        {
            //exec sp_executesql N'select admarea0_.Id as Id1_0_, admarea0_.AreaID as AreaID2_0_, admarea0_.AreaName as AreaNa3_0_, admarea0_.ParentId as Parent4_0_, admarea0_.ShortName as ShortN5_0_, admarea0_.LevelType as LevelT6_0_, admarea0_.CityCode as CityCo7_0_, admarea0_.ZipCode as ZipCod8_0_, admarea0_.AreaNamePath as AreaNa9_0_, admarea0_.AreaIDPath as AreaI10_0_, admarea0_.Lng as Lng11_0_, admarea0_.Lat as Lat12_0_, admarea0_.PinYin as PinYi13_0_, admarea0_.ShortPinYin as Short14_0_, admarea0_.PYFirstLetter as PYFir15_0_, admarea0_.SortOrder as SortO16_0_, admarea0_.Status as Statu17_0_, admarea0_.CreateTime as Creat18_0_, admarea0_.CreateUser as Creat19_0_, admarea0_.UpdateTime as Updat20_0_, admarea0_.UpdateUser as Updat21_0_ from wt_adm_area admarea0_ where admarea0_.Id=@p0',N'@p0 int',@p0=3
            Expression<Func<AdmArea, bool>> predicate = x => x.Id == id;
            var admArea = _admAreaRepository.Single(predicate);
            Assert.AreEqual(admArea.Id, id);
        }

        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// </summary>
        /// <param name="predicate">Entity</param>
        [TestCase(3)]
        public void AdmArea_SingleAsync_Predicate_Test(int id)
        {
            Expression<Func<AdmArea, bool>> predicate = x => x.Id == id;
            var admArea = _admAreaRepository.SingleAsync(predicate).Result;
            Assert.AreEqual(admArea.Id, id);
        }

        /// <summary>
        /// Creates an entity with given primary key without database access.
        /// </summary>
        /// <param name="id">Primary key of the entity to load</param>
        /// <returns>Entity</returns>
        [TestCase(4)]
        public void AdmArea_Load_Test(int id)
        {
            //exec sp_executesql N'SELECT admarea0_.Id as Id1_0_0_, admarea0_.AreaID as AreaID2_0_0_, admarea0_.AreaName as AreaNa3_0_0_, admarea0_.ParentId as Parent4_0_0_, admarea0_.ShortName as ShortN5_0_0_, admarea0_.LevelType as LevelT6_0_0_, admarea0_.CityCode as CityCo7_0_0_, admarea0_.ZipCode as ZipCod8_0_0_, admarea0_.AreaNamePath as AreaNa9_0_0_, admarea0_.AreaIDPath as AreaI10_0_0_, admarea0_.Lng as Lng11_0_0_, admarea0_.Lat as Lat12_0_0_, admarea0_.PinYin as PinYi13_0_0_, admarea0_.ShortPinYin as Short14_0_0_, admarea0_.PYFirstLetter as PYFir15_0_0_, admarea0_.SortOrder as SortO16_0_0_, admarea0_.Status as Statu17_0_0_, admarea0_.CreateTime as Creat18_0_0_, admarea0_.CreateUser as Creat19_0_0_, admarea0_.UpdateTime as Updat20_0_0_, admarea0_.UpdateUser as Updat21_0_0_ FROM wt_adm_area admarea0_ WHERE admarea0_.Id=@p0',N'@p0 int',@p0=4
            AdmArea admArea = _admAreaRepository.Load(id);
            Assert.AreEqual(admArea.Id, id);
            Console.WriteLine(admArea.AreaID);
            //Console.WriteLine(JsonSerializationHelper.SerializeByDefault(admArea));
        }
        /// <summary>
        /// Used to get a IQueryable that is used to retrieve entities from entire table.
        /// </summary>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public void AdmArea_GetAll_Test()
        {
            var admAreaAll = _admAreaRepository.GetAll();
            Assert.NotNull(admAreaAll);
        }

        /// <summary>
        /// Used to get a IQueryable that is used to retrieve entities from entire table.
        /// One or more 
        /// </summary>
        /// <param name="propertySelectors">A list of include expressions.</param>
        /// <returns>IQueryable to be used to select entities from database</returns>
        [TestCase]
        public void AdmArea_GetAllIncluding_Test()
        {
            //返回Id,Age,LastName,FirstName 字段
            //Expression<Func<AdmArea, object>>[] propertySelectors = {
            //    p => new { Id = p.Id, AreaID = p.AreaID },
            //    x => new { LastName = x.ShortName, FirstName = x.AreaName },
            //};
            //Expression<Func<AdmArea, object>>[] propertySelectors = {
            //    p => p.AreaID,
            //};
            Expression<Func<AdmArea, object>>[] propertySelectors = null;
            var admAreaAllIncluding = _admAreaRepository.GetAllIncluding(propertySelectors).ToList();
            Assert.NotNull(admAreaAllIncluding);
        }

        /// <summary>
        /// Used to get all entities.
        /// </summary>
        /// <returns>List of all entities</returns>
        [TestCase]
        public void AdmArea_GetAllList_Test()
        {
            var admAreaAllList = _admAreaRepository.GetAllList();
            Assert.NotNull(admAreaAllList);
        }

        /// <summary>
        /// Used to get all entities.
        /// </summary>
        /// <returns>List of all entities</returns>
        [TestCase]
        public void AdmArea_GetListAsync_Test()
        {
            var admAreaAllList = _admAreaRepository.GetAllListAsync().Result;
            Assert.NotNull(admAreaAllList);
        }

        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
        [TestCase]
        public void AdmArea_GetList_Predicate_Test()
        {
            /*exec sp_executesql N'select admarea0_.Id as Id1_0_, admarea0_.AreaID as AreaID2_0_, admarea0_.AreaName as AreaNa3_0_, admarea0_.ParentId as Parent4_0_, admarea0_.ShortName as ShortN5_0_, admarea0_.LevelType as LevelT6_0_, admarea0_.CityCode as CityCo7_0_, admarea0_.ZipCode as ZipCod8_0_, admarea0_.AreaNamePath as AreaNa9_0_, admarea0_.AreaIDPath as AreaI10_0_, admarea0_.lng as lng11_0_, admarea0_.Lat as Lat12_0_, admarea0_.PinYin as PinYi13_0_, admarea0_.ShortPinYin as Short14_0_, admarea0_.PYFirstLetter as PYFir15_0_, admarea0_.SortOrder as SortO16_0_, admarea0_.Status as Statu17_0_, admarea0_.CreateTime as Creat18_0_, admarea0_.CreateUser as Creat19_0_, admarea0_.UpdateTime as Updat20_0_, admarea0_.UpdateUser as Updat21_0_ from wt_adm_area admarea0_ where admarea0_.Id>=@p0 and admarea0_.Id<@p1 
             * order by admarea0_.AreaID asc, admarea0_.Id desc',N'@p0 int,@p1 int',@p0=100,@p1=105*/
            Expression<Func<AdmArea, bool>> predicate = x => x.Id >= 100 && x.Id < 105;
            Action<Orderable<AdmArea>> orderAction = (o =>
            {
                o.Desc(x => x.AreaID).Asc(x => x.CreateUser);
            });
            var admAreaAllList = _admAreaRepository.GetList(predicate,
                new OrderByExpression<AdmArea, string>(u => u.AreaID),    // a string, asc
                new OrderByExpression<AdmArea, int>(u => u.Id, true));
            Assert.NotNull(admAreaAllList);
        }

        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
        [TestCase]
        public void AdmArea_GetListAsync_Predicate_Test()
        {
            Expression<Func<AdmArea, bool>> predicate = x => x.Id == 5;
            var admAreaAllList = _admAreaRepository.GetListAsync(predicate,
                new OrderByExpression<AdmArea, object>(u => u.AreaID),    // a string, asc
                new OrderByExpression<AdmArea, object>(u => u.Id, true)).Result;
            Assert.NotNull(admAreaAllList);
        }

        /// <summary>
        /// Used to run a query over entire entities.
        /// <see cref="UnitOfWorkAttribute"/> attribute is not always necessary (as opposite to <see cref="GetAll"/>)
        /// if <paramref name="queryMethod"/> finishes IQueryable with ToList, FirstOrDefault etc..
        /// </summary>
        /// <typeparam name="T">Type of return value of this method</typeparam>
        /// <param name="queryMethod">This method is used to query over entities</param>
        /// <returns>Query result</returns>
        [TestCase]
        public void AdmArea_Query_Test()
        {
            //exec sp_executesql N'select admarea0_.Id as Id1_0_, admarea0_.AreaID as AreaID2_0_, admarea0_.AreaName as AreaNa3_0_, admarea0_.ParentId as Parent4_0_, admarea0_.ShortName as ShortN5_0_, admarea0_.LevelType as LevelT6_0_, admarea0_.CityCode as CityCo7_0_, admarea0_.ZipCode as ZipCod8_0_, admarea0_.AreaNamePath as AreaNa9_0_, admarea0_.AreaIDPath as AreaI10_0_, admarea0_.Lng as Lng11_0_, admarea0_.Lat as Lat12_0_, admarea0_.PinYin as PinYi13_0_, admarea0_.ShortPinYin as Short14_0_, admarea0_.PYFirstLetter as PYFir15_0_, admarea0_.SortOrder as SortO16_0_, admarea0_.Status as Statu17_0_, admarea0_.CreateTime as Creat18_0_, admarea0_.CreateUser as Creat19_0_, admarea0_.UpdateTime as Updat20_0_, admarea0_.UpdateUser as Updat21_0_ from wt_adm_area admarea0_ where admarea0_.Id>=@p0 and admarea0_.Id<@p1 ORDER BY CURRENT_TIMESTAMP OFFSET 0 ROWS FETCH FIRST 1 ROWS ONLY',N'@p0 int,@p1 int',@p0=100,@p1=105
            Func<IQueryable<AdmArea>, AdmArea> queryMethod = q => q.Where(x => x.Id >= 100 && x.Id < 105).FirstOrDefault();
            var result = _admAreaRepository.Query(queryMethod);
            Assert.NotNull(result);
            //Console.WriteLine(result.AreaID);
        }
        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
        [TestCase]
        public void AdmArea_GetPaggingList_Predicate_Test()
        {
            /*exec sp_executesql N'SELECT TOP (@p0) Id1_0_, AreaID2_0_, AreaNa3_0_, Parent4_0_, ShortN5_0_, LevelT6_0_, CityCo7_0_, ZipCod8_0_, AreaNa9_0_, AreaI10_0_, lng11_0_, Lat12_0_, PinYi13_0_, Short14_0_, PYFir15_0_, SortO16_0_, Statu17_0_, Creat18_0_, Creat19_0_, Updat20_0_, Updat21_0_ FROM (select admarea0_.Id as Id1_0_, admarea0_.AreaID as AreaID2_0_, admarea0_.AreaName as AreaNa3_0_, admarea0_.ParentId as Parent4_0_, admarea0_.ShortName as ShortN5_0_, admarea0_.LevelType as LevelT6_0_, admarea0_.CityCode as CityCo7_0_, admarea0_.ZipCode as ZipCod8_0_, admarea0_.AreaNamePath as AreaNa9_0_, admarea0_.AreaIDPath as AreaI10_0_, admarea0_.lng as lng11_0_, admarea0_.Lat as Lat12_0_, admarea0_.PinYin as PinYi13_0_, admarea0_.ShortPinYin as Short14_0_, admarea0_.PYFirstLetter as PYFir15_0_, admarea0_.SortOrder as SortO16_0_, admarea0_.Status as Statu17_0_, admarea0_.CreateTime as Creat18_0_, admarea0_.CreateUser as Creat19_0_, admarea0_.UpdateTime as Updat20_0_, admarea0_.UpdateUser as Updat21_0_, ROW_NUMBER() OVER(ORDER BY admarea0_.AreaID, admarea0_.Id DESC) as __hibernate_sort_row from wt_adm_area admarea0_ where admarea0_.Id>=@p1 and admarea0_.Id<@p2) as query WHERE query.__hibernate_sort_row > @p3 ORDER BY query.__hibernate_sort_row',N'@p0 int,@p1 int,@p2 int,@p3 int',@p0=5,@p1=100,@p2=200,@p3=5 */
            Expression<Func<AdmArea, bool>> predicate = x => x.Id >= 100 && x.Id < 200;
            Action<Orderable<AdmArea>> orderAction = (o =>
            {
                o.Desc(x => x.AreaID).Asc(x => x.CreateUser);
            });
            var pageIndex = 2;
            var pageSize = 5;
            IOrderByExpression<AdmArea>[] orderByExpressions = {
                  new OrderByExpression<AdmArea, string>(u => u.AreaID),    // a string, asc
                new OrderByExpression<AdmArea, int>(u => u.Id, true)
            };
            var result = _admAreaRepository.GetPaggingList(predicate,
                pageIndex,
                pageSize,
              orderByExpressions);
            //Console.WriteLine(Utility.Json.JsonSerializationHelper.SerializeByDefault(result));
            Assert.NotNull(result);
        }
        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
        [TestCase]
        public async Task AdmArea_GetPaggingListAsync_Predicate_Test()
        {
            /*exec sp_executesql N'SELECT TOP (@p0) Id1_0_, AreaID2_0_, AreaNa3_0_, Parent4_0_, ShortN5_0_, LevelT6_0_, CityCo7_0_, ZipCod8_0_, AreaNa9_0_, AreaI10_0_, lng11_0_, Lat12_0_, PinYi13_0_, Short14_0_, PYFir15_0_, SortO16_0_, Statu17_0_, Creat18_0_, Creat19_0_, Updat20_0_, Updat21_0_ FROM (select admarea0_.Id as Id1_0_, admarea0_.AreaID as AreaID2_0_, admarea0_.AreaName as AreaNa3_0_, admarea0_.ParentId as Parent4_0_, admarea0_.ShortName as ShortN5_0_, admarea0_.LevelType as LevelT6_0_, admarea0_.CityCode as CityCo7_0_, admarea0_.ZipCode as ZipCod8_0_, admarea0_.AreaNamePath as AreaNa9_0_, admarea0_.AreaIDPath as AreaI10_0_, admarea0_.lng as lng11_0_, admarea0_.Lat as Lat12_0_, admarea0_.PinYin as PinYi13_0_, admarea0_.ShortPinYin as Short14_0_, admarea0_.PYFirstLetter as PYFir15_0_, admarea0_.SortOrder as SortO16_0_, admarea0_.Status as Statu17_0_, admarea0_.CreateTime as Creat18_0_, admarea0_.CreateUser as Creat19_0_, admarea0_.UpdateTime as Updat20_0_, admarea0_.UpdateUser as Updat21_0_, ROW_NUMBER() OVER(ORDER BY admarea0_.AreaID, admarea0_.Id DESC) as __hibernate_sort_row from wt_adm_area admarea0_ where admarea0_.Id>=@p1 and admarea0_.Id<@p2) as query WHERE query.__hibernate_sort_row > @p3 ORDER BY query.__hibernate_sort_row',N'@p0 int,@p1 int,@p2 int,@p3 int',@p0=5,@p1=100,@p2=200,@p3=10 */
            Expression<Func<AdmArea, bool>> predicate = x => x.Id >= 100 && x.Id < 200;
            Action<Orderable<AdmArea>> orderAction = (o =>
            {
                o.Desc(x => x.AreaID).Asc(x => x.CreateUser);
            });
            var pageIndex = 3;
            var pageSize = 5;
            IOrderByExpression<AdmArea>[] orderByExpressions = {
                  new OrderByExpression<AdmArea, string>(u => u.AreaID),    // a string, asc
                new OrderByExpression<AdmArea, int>(u => u.Id, true)
            };
            var result =await _admAreaRepository.GetPaggingListAsync(predicate,
                pageIndex,
                pageSize,
                orderByExpressions
              );
            //Console.WriteLine(Utility.Json.JsonSerializationHelper.SerializeByDefault(result));
            Assert.NotNull(result);
        }
        #endregion
        #region Insert

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        [TestCase]
        public void AdmArea_Insert_Test()
        {
            //exec sp_executesql N'INSERT INTO wt_adm_area (AreaID, AreaName, ParentId, ShortName, LevelType, CityCode, ZipCode, AreaNamePath, AreaIDPath, lng, Lat, PinYin, ShortPinYin, PYFirstLetter, SortOrder, Status, CreateTime, CreateUser, UpdateTime, UpdateUser) VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16, @p17, @p18, @p19); select SCOPE_IDENTITY()',N'@p0 nvarchar(4000),@p1 nvarchar(4000),@p2 nvarchar(4000),@p3 nvarchar(4000),@p4 tinyint,@p5 nvarchar(4000),@p6 nvarchar(4000),@p7 nvarchar(4000),@p8 nvarchar(4000),@p9 decimal(28,5),@p10 decimal(28,5),@p11 nvarchar(4000),@p12 nvarchar(4000),@p13 nvarchar(4000),@p14 int,@p15 tinyint,@p16 datetime,@p17 int,@p18 datetime,@p19 int',@p0=N'921376',@p1=N'测试地区名称774',@p2=N'100000',@p3=N'地区简称',@p4=0,@p5=NULL,@p6=NULL,@p7=N'中国,测试地区名称774',@p8=N'100000,921376',@p9=0,@p10=0,@p11=NULL,@p12=NULL,@p13=NULL,@p14=87,@p15=0,@p16='2017-06-21 17:34:21',@p17=4367,@p18='2017-06-21 17:34:21',@p19=4324
            //---Insert operation should work and tenant, creation audit properties must be set---------------------
            var entity = CreateAdmArea();
            var result = _admAreaRepository.Insert(entity);
            Console.WriteLine(result.Id);
            Assert.AreEqual(entity.AreaID, result.AreaID);
        }

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        [TestCase]
        public void AdmArea_InsertAsync_Test()
        {
            var entity = CreateAdmArea();
            var result = _admAreaRepository.InsertAsync(entity).Result;
            Assert.AreEqual(entity.AreaID, result.AreaID);
        }

        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        [TestCase]
        public void AdmArea_InsertAndGetId_Test()
        {
            //exec sp_executesql N'INSERT INTO wt_adm_area (AreaID, AreaName, ParentId, ShortName, LevelType, CityCode, ZipCode, AreaNamePath, AreaIDPath, lng, Lat, PinYin, ShortPinYin, PYFirstLetter, SortOrder, Status, CreateTime, CreateUser, UpdateTime, UpdateUser) VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16, @p17, @p18, @p19); select SCOPE_IDENTITY()',N'@p0 nvarchar(4000),@p1 nvarchar(4000),@p2 nvarchar(4000),@p3 nvarchar(4000),@p4 tinyint,@p5 nvarchar(4000),@p6 nvarchar(4000),@p7 nvarchar(4000),@p8 nvarchar(4000),@p9 decimal(28,5),@p10 decimal(28,5),@p11 nvarchar(4000),@p12 nvarchar(4000),@p13 nvarchar(4000),@p14 int,@p15 tinyint,@p16 datetime,@p17 int,@p18 datetime,@p19 int',@p0=N'151282',@p1=N'测试地区名称187',@p2=N'100000',@p3=N'地区简称',@p4=0,@p5=NULL,@p6=NULL,@p7=N'中国,测试地区名称187',@p8=N'100000,151282',@p9=0,@p10=0,@p11=NULL,@p12=NULL,@p13=NULL,@p14=52,@p15=0,@p16='2017-06-21 17:46:14',@p17=5884,@p18='2017-06-21 17:46:14',@p19=4616
            var entity = CreateAdmArea();
            var result = _admAreaRepository.InsertAndGetId(entity);
            Assert.Greater(result, 0);
            Console.WriteLine("AdmArea,id:" + entity.Id);
        }

        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        [TestCase]
        public void AdmArea_InsertAndGetIdAsync_Test()
        {
            var entity = CreateAdmArea();
            var result = _admAreaRepository.InsertAndGetIdAsync(entity).Result;
            Assert.Greater(result, 0);
        }

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="entity">Entity</param>
        [TestCase]
        public void AdmArea_InsertOrUpdate_Test()
        {
            var entity = CreateAdmArea();
            entity.Id = 3757;
            var result = _admAreaRepository.InsertOrUpdate(entity);
            Assert.AreEqual(result.AreaID, result.AreaID);
        }

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="entity">Entity</param>
        [TestCase]
        public void AdmArea_InsertOrUpdateAsync_Test()
        {
            var entity = CreateAdmArea();
            var result = _admAreaRepository.InsertOrUpdateAsync(entity).Result;
            Assert.AreEqual(result.AreaID, result.AreaID);
        }

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// Also returns Id of the entity.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        [TestCase]
        public void AdmArea_InsertOrUpdateAndGetId_Test()
        {
            var entity = CreateAdmArea();
            var result = _admAreaRepository.InsertOrUpdateAndGetId(entity);
            Assert.Greater(result, 0);
        }

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// Also returns Id of the entity.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        [TestCase]
        public void AdmArea_InsertOrUpdateAndGetIdAsync_Test()
        {
            var entity = CreateAdmArea();
            var result = _admAreaRepository.InsertOrUpdateAndGetIdAsync(entity).Result;
            Assert.Greater(result, 0);
        }

        #endregion

        #region Update  

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        [Test]
        public void AdmArea_Update_Test()
        {
            var entity = _admAreaRepository.Single(a=>a.Id==5396);
            entity.AreaName = entity.AreaName + " Update";
            entity.UpdateTime = DateTime.Now;
            var result = _admAreaRepository.Update(entity);
            Assert.AreEqual(result.AreaName, entity.AreaName);
        }

        /// <summary>
        /// Updates an existing entity. 
        /// </summary>
        /// <param name="entity">Entity</param>
        [TestCase]
        public void AdmArea_UpdateAsync_Test()
        {
            var entity = CreateAdmArea();
            var insertResult = _admAreaRepository.Insert(entity);
            entity.AreaName += " UpdateAsync";
            entity.UpdateTime = DateTime.Now;
            var result = _admAreaRepository.UpdateAsync(entity).Result;
            Assert.AreEqual(result.AreaName, entity.AreaName);
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="updateAction">Action that can be used to change values of the entity</param>
        /// <returns>Updated entity</returns>
        [TestCase]
        public void AdmArea_Update_UpdateAction_Test()
        {
            /*
             * exec sp_executesql N'UPDATE wt_adm_area SET AreaID = @p0, AreaName = @p1, ParentId = @p2, ShortName = @p3, LevelType = @p4, CityCode = @p5, ZipCode = @p6, AreaNamePath = @p7, AreaIDPath = @p8, lng = @p9, Lat = @p10, PinYin = @p11, ShortPinYin = @p12, PYFirstLetter = @p13, SortOrder = @p14, Status = @p15, CreateTime = @p16, CreateUser = @p17, UpdateTime = @p18, UpdateUser = @p19 WHERE Id = @p20',N'@p0 nvarchar(4000),@p1 nvarchar(4000),@p2 nvarchar(4000),@p3 nvarchar(4000),@p4 tinyint,@p5 nvarchar(4000),@p6 nvarchar(4000),@p7 nvarchar(4000),@p8 nvarchar(4000),@p9 decimal(28,5),@p10 decimal(28,5),@p11 nvarchar(4000),@p12 nvarchar(4000),@p13 nvarchar(4000),@p14 int,@p15 tinyint,@p16 datetime,@p17 int,@p18 datetime,@p19 int,@p20 int',@p0=N'877326',@p1=N'测试地区名称177Async UpdateAction',@p2=N'100000',@p3=N'地区简称',@p4=0,@p5=N'',@p6=N'',@p7=N'中国,测试地区名称177',@p8=N'100000,877326',@p9=0,@p10=0,@p11=NULL,@p12=N'',@p13=N'',@p14=43,@p15=0,@p16='2017-07-20 17:05:16',@p17=8964,@p18='2017-07-20 17:05:16',@p19=950,@p20=5415
             */
            var entity = CreateAdmArea();
            var insertResult = _admAreaRepository.Insert(entity);
            var result = _admAreaRepository.Update(entity.Id,
                (a) =>
                {
                    a.AreaName += " UpdateAction";
                    return a;
                });
            Assert.AreEqual(result.AreaName, entity.AreaName+ " UpdateAction");
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="updateAction">Action that can be used to change values of the entity</param>
        /// <returns>Updated entity</returns>
        [TestCase]
        public async Task AdmArea_UpdateAsync_UpdateAction_Test()
        {
            var entity = CreateAdmArea();
            var insertResult = _admAreaRepository.Insert(entity);
            var result = await _admAreaRepository.UpdateAsync(entity.Id,
               (a) =>
               {
                   return Task.Factory.StartNew(() =>
                   {
                       a.AreaName += "Async UpdateAction";
                       return a;
                   });
               }
              );
            Assert.AreEqual(result.AreaName, entity.AreaName+ "Async UpdateAction");
        }

        #endregion 

        #region Delete 

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        [Test]
        public void AdmArea_Delete_Test()
        {
            var entity = CreateAdmArea();
            var insertResult = _admAreaRepository.Insert(entity);
            _admAreaRepository.Delete(entity);
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        [Test]
        public void AdmArea_DeleteAsync_Test()
        {
            var entity = CreateAdmArea();
            var insertResult = _admAreaRepository.Insert(entity);
            _admAreaRepository.DeleteAsync(entity);
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        [Test]
        public void AdmArea_Delete_TPrimaryKey_Test()
        {
            var entity = CreateAdmArea();
            var insertResult = _admAreaRepository.Insert(entity);
            _admAreaRepository.Delete(entity.Id);
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        [Test]
        public void AdmArea_DeleteAsync_TPrimaryKey_Test()
        {
            var entity = CreateAdmArea();
            var insertResult = _admAreaRepository.Insert(entity);
            _admAreaRepository.DeleteAsync(entity.Id);
        }

        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        [Test]
        public void AdmArea_Delete_Predicate_Test()
        {
            var entity = CreateAdmArea();
            var insertResult = _admAreaRepository.Insert(entity);
            _admAreaRepository.Delete(x => x.Id == entity.Id && x.AreaID == entity.AreaID);
        }

        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        [Test]
        public void AdmArea_DeleteAsync_Predicate_Test()
        {
            var entity = CreateAdmArea();
            var insertResult = _admAreaRepository.Insert(entity);
            _admAreaRepository.Delete(x => x.Id == entity.Id && x.AreaID == entity.AreaID);
        }

        #endregion 
        #region Aggregates Tests

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        [Test]
        public void AdmArea_Count_Test()
        {
            //select cast(count(*) as INT) as col_0_0_ from wt_adm_area admarea0_
            var result = _admAreaRepository.Count();
            Assert.GreaterOrEqual(result, 0);
            Console.WriteLine(result);
        }

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        [Test]
        public void AdmArea_CountAsync_Test()
        {
            var result = _admAreaRepository.CountAsync().Result;
            Assert.GreaterOrEqual(result, 0);
            Console.WriteLine(result);
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        [Test]
        public void AdmArea_Count_Predicate_Test()
        {
            //exec sp_executesql N'select cast(count(*) as INT) as col_0_0_ from wt_adm_area admarea0_ where admarea0_.Id>@p0 and admarea0_.Id<@p1',N'@p0 int,@p1 int',@p0=100,@p1=1000
            Expression<Func<AdmArea, bool>> predicate = x => x.Id > 100 && x.Id < 1000;
            var result = _admAreaRepository.Count(predicate);
            Assert.GreaterOrEqual(result, 10);
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        [Test]
        public void AdmArea_CountAsync_Predicate_Test()
        {
            Expression<Func<AdmArea, bool>> predicate = x => x.Id > 100 && x.Id < 1000;
            var result = _admAreaRepository.CountAsync(predicate).Result;
            Assert.GreaterOrEqual(result, 10);
        }

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        [Test]
        public void AdmArea_LongCount_Test()
        {
            var result = _admAreaRepository.LongCount();
            Assert.GreaterOrEqual(result, 0);
            Console.WriteLine(result);
        }

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        [Test]
        public void AdmArea_LongCountAsync_Test()
        {
            var result = _admAreaRepository.LongCountAsync().Result;
            Assert.GreaterOrEqual(result, 0);
            Console.WriteLine(result);
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        [Test]
        public void AdmArea_LongCount_Predicate_Test()
        {
            //exec sp_executesql N'select cast(count(*) as BIGINT) as col_0_0_ from wt_adm_area admarea0_ where admarea0_.Id>@p0 and admarea0_.Id<@p1',N'@p0 int,@p1 int',@p0=100,@p1=1000
            Expression<Func<AdmArea, bool>> predicate = x => x.Id > 100 && x.Id < 1000;
            var result = _admAreaRepository.LongCount(predicate);
            Assert.GreaterOrEqual(result, 10);
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        [Test]
        public void AdmArea_LongCountAsync_Predicate_Test()
        {

            Expression<Func<AdmArea, bool>> predicate = x => x.Id > 100 && x.Id < 1000;
            var result = _admAreaRepository.LongCountAsync(predicate).Result;
            Assert.GreaterOrEqual(result, 10);
        }
        #endregion  Aggregates

    }
}
