using Autofac;
using NUnit.Framework;
using Orchard.Data.EntityFramework.Tests.Entities;
using Orchard.Utility;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Orchard.Data.EntityFramework.Tests.Repositories
{
    public class AdmArea_Repository_Tests : EfNUnitTestBase
    {
        protected override void Register(ContainerBuilder builder)
        {

        }
        private Domain.Repositories.IRepository<AdmArea> _admAreaRepository;

        protected override void Init()
        {
            base.Init();
            _admAreaRepository = Container.Resolve<Domain.Repositories.IRepository<AdmArea>>();
        }

        #region Select/Get/Query Tests

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        //[Theory]
        [TestCase(1)]
        public void AdmArea_Single_Test(int id)
        {
            //exec sp_executesql N'SELECT admarea0_.Id as Id1_0_0_, admarea0_.AreaID as AreaID2_0_0_, admarea0_.AreaName as AreaNa3_0_0_, admarea0_.ParentId as Parent4_0_0_, admarea0_.ShortName as ShortN5_0_0_, admarea0_.LevelType as LevelT6_0_0_, admarea0_.CityCode as CityCo7_0_0_, admarea0_.ZipCode as ZipCod8_0_0_, admarea0_.AreaNamePath as AreaNa9_0_0_, admarea0_.AreaIDPath as AreaI10_0_0_, admarea0_.Lng as Lng11_0_0_, admarea0_.Lat as Lat12_0_0_, admarea0_.PinYin as PinYi13_0_0_, admarea0_.ShortPinYin as Short14_0_0_, admarea0_.PYFirstLetter as PYFir15_0_0_, admarea0_.SortOrder as SortO16_0_0_, admarea0_.Status as Statu17_0_0_, admarea0_.CreateTime as Creat18_0_0_, admarea0_.CreateUser as Creat19_0_0_, admarea0_.UpdateTime as Updat20_0_0_, admarea0_.UpdateUser as Updat21_0_0_ FROM wt_adm_area admarea0_ WHERE admarea0_.Id=@p0',N'@p0 int',@p0=1
            var admArea = CreateAdmArea();
            _admAreaRepository.Insert(admArea);
            id = admArea.Id;
            var  result = _admAreaRepository.Single(admArea.Id);
            Assert.AreEqual(result.Id, id);
            // Console.WriteLine(JsonSerializationHelper.SerializeByDefault(admArea));
        }

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        //[Theory]
        [TestCase(2)]
        public async Task AdmArea_SingleAsync_Test(int id)
        {
            var admArea = await _admAreaRepository.SingleAsync(id);
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
        public async Task AdmArea_SingleAsync_Predicate_Test(int id)
        {
            Expression<Func<AdmArea, bool>> predicate = x => x.Id == id;
            var admArea = await _admAreaRepository.SingleAsync(predicate);
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
        public async Task AdmArea_GetAllListAsync_Test()
        {
            var admAreaAllList = await _admAreaRepository.GetAllListAsync();
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
            /*SELECT 
    [Extent1].[Id] AS [Id], 
    [Extent1].[AreaID] AS [AreaID], 
    [Extent1].[AreaName] AS [AreaName], 
    [Extent1].[ParentId] AS [ParentId], 
    [Extent1].[ShortName] AS [ShortName], 
    [Extent1].[LevelType] AS [LevelType], 
    [Extent1].[CityCode] AS [CityCode], 
    [Extent1].[ZipCode] AS [ZipCode], 
    [Extent1].[AreaNamePath] AS [AreaNamePath], 
    [Extent1].[AreaIDPath] AS [AreaIDPath], 
    [Extent1].[lng] AS [lng], 
    [Extent1].[Lat] AS [Lat], 
    [Extent1].[PinYin] AS [PinYin], 
    [Extent1].[ShortPinYin] AS [ShortPinYin], 
    [Extent1].[PYFirstLetter] AS [PYFirstLetter], 
    [Extent1].[SortOrder] AS [SortOrder], 
    [Extent1].[Status] AS [Status], 
    [Extent1].[CreateTime] AS [CreateTime], 
    [Extent1].[CreateUser] AS [CreateUser], 
    [Extent1].[UpdateTime] AS [UpdateTime], 
    [Extent1].[UpdateUser] AS [UpdateUser]
    FROM [dbo].[wt_adm_area] AS [Extent1]
    WHERE 5 = [Extent1].[Id]
    ORDER BY [Extent1].[AreaID] ASC, [Extent1].[Id] DESC */
            Action<Orderable<AdmArea>> orderAction = (o =>
            {
                o.Desc(x => x.AreaID).Asc(x => x.CreateUser);
            });
            Expression<Func<AdmArea, bool>> predicate = x => x.Id == 5;
            var result = _admAreaRepository.GetList(predicate,
                new OrderByExpression<AdmArea, string>(u => u.AreaID),    // a string, asc
                new OrderByExpression<AdmArea, int>(u => u.Id, true));
            Assert.NotNull(result);
        }

        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
        [TestCase]
        public async Task AdmArea_GetListAsync_Predicate_Test()
        {
            /*SELECT 
    [Extent1].[Id] AS [Id], 
    [Extent1].[AreaID] AS [AreaID], 
    [Extent1].[AreaName] AS [AreaName], 
    [Extent1].[ParentId] AS [ParentId], 
    [Extent1].[ShortName] AS [ShortName], 
    [Extent1].[LevelType] AS [LevelType], 
    [Extent1].[CityCode] AS [CityCode], 
    [Extent1].[ZipCode] AS [ZipCode], 
    [Extent1].[AreaNamePath] AS [AreaNamePath], 
    [Extent1].[AreaIDPath] AS [AreaIDPath], 
    [Extent1].[lng] AS [lng], 
    [Extent1].[Lat] AS [Lat], 
    [Extent1].[PinYin] AS [PinYin], 
    [Extent1].[ShortPinYin] AS [ShortPinYin], 
    [Extent1].[PYFirstLetter] AS [PYFirstLetter], 
    [Extent1].[SortOrder] AS [SortOrder], 
    [Extent1].[Status] AS [Status], 
    [Extent1].[CreateTime] AS [CreateTime], 
    [Extent1].[CreateUser] AS [CreateUser], 
    [Extent1].[UpdateTime] AS [UpdateTime], 
    [Extent1].[UpdateUser] AS [UpdateUser]
    FROM [dbo].[wt_adm_area] AS [Extent1]
    WHERE 5 = [Extent1].[Id]
    ORDER BY [Extent1].[AreaID] ASC, [Extent1].[Id] DESC */
            Expression<Func<AdmArea, bool>> predicate = x => x.Id == 5;
            var admAreaAllList = await _admAreaRepository.GetListAsync(predicate,
                new OrderByExpression<AdmArea, string>(u => u.AreaID),    // a string, asc
                new OrderByExpression<AdmArea, int>(u => u.Id, true));
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
            /*SELECT 
    [Extent1].[Id] AS [Id], 
    [Extent1].[AreaID] AS [AreaID], 
    [Extent1].[AreaName] AS [AreaName], 
    [Extent1].[ParentId] AS [ParentId], 
    [Extent1].[ShortName] AS [ShortName], 
    [Extent1].[LevelType] AS [LevelType], 
    [Extent1].[CityCode] AS [CityCode], 
    [Extent1].[ZipCode] AS [ZipCode], 
    [Extent1].[AreaNamePath] AS [AreaNamePath], 
    [Extent1].[AreaIDPath] AS [AreaIDPath], 
    [Extent1].[lng] AS [lng], 
    [Extent1].[Lat] AS [Lat], 
    [Extent1].[PinYin] AS [PinYin], 
    [Extent1].[ShortPinYin] AS [ShortPinYin], 
    [Extent1].[PYFirstLetter] AS [PYFirstLetter], 
    [Extent1].[SortOrder] AS [SortOrder], 
    [Extent1].[Status] AS [Status], 
    [Extent1].[CreateTime] AS [CreateTime], 
    [Extent1].[CreateUser] AS [CreateUser], 
    [Extent1].[UpdateTime] AS [UpdateTime], 
    [Extent1].[UpdateUser] AS [UpdateUser]
    FROM [dbo].[wt_adm_area] AS [Extent1]
    WHERE ([Extent1].[Id] >= 100) AND ([Extent1].[Id] < 105)
    ORDER BY [Extent1].[AreaID] ASC, [Extent1].[Id] DESC
    OFFSET 0 ROWS FETCH NEXT 3 ROWS ONLY */
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
            /*SELECT 
    [Extent1].[Id] AS [Id], 
    [Extent1].[AreaID] AS [AreaID], 
    [Extent1].[AreaName] AS [AreaName], 
    [Extent1].[ParentId] AS [ParentId], 
    [Extent1].[ShortName] AS [ShortName], 
    [Extent1].[LevelType] AS [LevelType], 
    [Extent1].[CityCode] AS [CityCode], 
    [Extent1].[ZipCode] AS [ZipCode], 
    [Extent1].[AreaNamePath] AS [AreaNamePath], 
    [Extent1].[AreaIDPath] AS [AreaIDPath], 
    [Extent1].[lng] AS [lng], 
    [Extent1].[Lat] AS [Lat], 
    [Extent1].[PinYin] AS [PinYin], 
    [Extent1].[ShortPinYin] AS [ShortPinYin], 
    [Extent1].[PYFirstLetter] AS [PYFirstLetter], 
    [Extent1].[SortOrder] AS [SortOrder], 
    [Extent1].[Status] AS [Status], 
    [Extent1].[CreateTime] AS [CreateTime], 
    [Extent1].[CreateUser] AS [CreateUser], 
    [Extent1].[UpdateTime] AS [UpdateTime], 
    [Extent1].[UpdateUser] AS [UpdateUser]
    FROM [dbo].[wt_adm_area] AS [Extent1]
    WHERE ([Extent1].[Id] >= 100) AND ([Extent1].[Id] < 200)
    ORDER BY [Extent1].[AreaID] ASC, [Extent1].[Id] DESC
    OFFSET 10 ROWS FETCH NEXT 5 ROWS ONLY */
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
              orderByExpressions);
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

            /*
             * exec sp_executesql N'INSERT [dbo].[wt_adm_area]([AreaID], [AreaName], [ParentId], [ShortName], [LevelType], [CityCode], [ZipCode], [AreaNamePath], [AreaIDPath], [lng], [Lat], [PinYin], [ShortPinYin], [PYFirstLetter], [SortOrder], [Status], [CreateTime], [CreateUser], [UpdateTime], [UpdateUser])
VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, NULL, @11, @12, @13, @14, @15, @16, @17, @18)
SELECT [Id]
FROM [dbo].[wt_adm_area]
WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()',N'@0 varchar(50),@1 varchar(50),@2 varchar(50),@3 varchar(50),@4 tinyint,@5 varchar(50),@6 varchar(50),@7 nvarchar(500),@8 varchar(500),@9 decimal(18,2),@10 decimal(18,2),@11 varchar(20),@12 varchar(10),@13 int,@14 tinyint,@15 datetime2(7),@16 int,@17 datetime2(7),@18 int',@0='860580',@1='测试地区名称846',@2='100000',@3='地区简称',@4=0,@5='',@6='',@7=N'中国,测试地区名称846',@8='100000,860580',@9=0,@10=0,@11='',@12='',@13=79,@14=0,@15='2017-06-21 21:39:45.7710285',@16=5074,@17='2017-06-21 21:39:45.7710285',@18=3361
             */
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
        public async Task AdmArea_InsertAsync_Test()
        {
            var entity = CreateAdmArea();
            var result = await _admAreaRepository.InsertAsync(entity);
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
        public async Task AdmArea_InsertAndGetIdAsync_Test()
        {
            var entity = CreateAdmArea();
            var result = await _admAreaRepository.InsertAndGetIdAsync(entity);
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
        public async Task AdmArea_InsertOrUpdateAsync_Test()
        {
            var entity = CreateAdmArea();
            var result = await _admAreaRepository.InsertOrUpdateAsync(entity);
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
            /*
             * exec sp_executesql N'UPDATE [dbo].[wt_adm_area]
SET [AreaID] = @0, [AreaName] = @1, [ParentId] = @2, [ShortName] = @3, [LevelType] = @4, [CityCode] = @5, [ZipCode] = @6, [AreaNamePath] = @7, [AreaIDPath] = @8, [lng] = @9, [Lat] = @10, [PinYin] = NULL, [ShortPinYin] = @11, [PYFirstLetter] = @12, [SortOrder] = @13, [Status] = @14, [CreateTime] = @15, [CreateUser] = @16, [UpdateTime] = @17, [UpdateUser] = @18
WHERE ([Id] = @19)
',N'@0 varchar(50),@1 varchar(50),@2 varchar(50),@3 varchar(50),@4 tinyint,@5 varchar(50),@6 varchar(50),@7 nvarchar(500),@8 varchar(500),@9 decimal(18,2),@10 decimal(18,2),@11 varchar(20),@12 varchar(10),@13 int,@14 tinyint,@15 datetime2(7),@16 int,@17 datetime2(7),@18 int,@19 int',@0='856519',@1='测试地区名称915',@2='100000',@3='地区简称',@4=0,@5='',@6='',@7=N'中国,测试地区名称915',@8='100000,856519',@9=0,@10=0,@11='',@12='',@13=96,@14=0,@15='2017-06-21 22:10:06.9312448',@16=3811,@17='2017-06-21 22:10:06.9312448',@18=3000,@19=3756
             */
            var entity = CreateAdmArea();
            entity.Id = 3756;
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
        public async Task AdmArea_InsertOrUpdateAndGetIdAsync_Test()
        {
            var entity = CreateAdmArea();
            var result = await _admAreaRepository.InsertOrUpdateAndGetIdAsync(entity);
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
            /*
             * exec sp_executesql N'UPDATE [dbo].[wt_adm_area]
SET [AreaID] = @0, [AreaName] = @1, [ParentId] = @2, [ShortName] = @3, [LevelType] = @4, [CityCode] = @5, [ZipCode] = @6, [AreaNamePath] = @7, [AreaIDPath] = @8, [lng] = @9, [Lat] = @10, [PinYin] = NULL, [ShortPinYin] = @11, [PYFirstLetter] = @12, [SortOrder] = @13, [Status] = @14, [CreateTime] = @15, [CreateUser] = @16, [UpdateTime] = @17, [UpdateUser] = @18
WHERE ([Id] = @19)
',N'@0 varchar(50),@1 varchar(50),@2 varchar(50),@3 varchar(50),@4 tinyint,@5 varchar(50),@6 varchar(50),@7 nvarchar(500),@8 varchar(500),@9 decimal(18,2),@10 decimal(18,2),@11 varchar(20),@12 varchar(10),@13 int,@14 tinyint,@15 datetime2(7),@16 int,@17 datetime2(7),@18 int,@19 int',@0='409878',@1='测试地区名称259 Update',@2='100000',@3='地区简称',@4=0,@5='',@6='',@7=N'中国,测试地区名称259',@8='100000,409878',@9=0,@10=0,@11='',@12='',@13=92,@14=0,@15='2017-06-21 22:13:50.3718472',@16=8442,@17='2017-06-21 22:13:51.2108839',@18=1430,@19=3759
             */
            var entity = CreateAdmArea();
            var insertResult = _admAreaRepository.Insert(entity);
            insertResult.AreaName = insertResult.AreaName + " Update";
            insertResult.UpdateTime = DateTime.Now;
            var result = _admAreaRepository.Update(insertResult);
            Assert.AreEqual(result.AreaName, insertResult.AreaName);
        }

        /// <summary>
        /// Updates an existing entity. 
        /// </summary>
        /// <param name="entity">Entity</param>
        [TestCase]
        public async Task AdmArea_UpdateAsync_Test()
        {
            var entity = CreateAdmArea();
            var insertResult = _admAreaRepository.Insert(entity);
            entity.AreaName += " UpdateAsync";
            entity.UpdateTime = DateTime.Now;
            var result = await _admAreaRepository.UpdateAsync(entity);
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
            var entity = CreateAdmArea();
            var insertResult = _admAreaRepository.Insert(entity);
            var result = _admAreaRepository.Update(entity.Id,
                (a) =>
                {
                    a.AreaName += " UpdateAction";
                    return a;
                });
            Assert.AreEqual(result.AreaName, entity.AreaName);

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
            Assert.AreEqual(result.AreaName, entity.AreaName);
        }
        private Task<AdmArea> UpdateAdmAreaAction(AdmArea entity)
        {
            entity.AreaName += " UpdateAction";
            return Task.FromResult(entity);
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
            //System.Threading.Thread.Sleep(500);
            _admAreaRepository.Delete(entity);
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        [Test]
        public async Task AdmArea_DeleteAsync_Test()
        {
            var entity = CreateAdmArea();
            var insertResult = _admAreaRepository.Insert(entity);
            await _admAreaRepository.DeleteAsync(entity);
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
        public async Task AdmArea_DeleteAsync_TPrimaryKey_Test()
        {
            var entity = CreateAdmArea();
            var insertResult = _admAreaRepository.Insert(entity);
            await _admAreaRepository.DeleteAsync(entity.Id);
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
            /*
             * exec sp_executesql N'DELETE [dbo].[wt_adm_area]
WHERE ([Id] = @0)',N'@0 int',@0=3761
             */
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
        public async Task AdmArea_DeleteAsync_Predicate_Test()
        {
            var entity = CreateAdmArea();
            var insertResult = _admAreaRepository.Insert(entity);
            await _admAreaRepository.DeleteAsync(x => x.Id == entity.Id && x.AreaID == entity.AreaID);
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
            /*
             SELECT 
    [GroupBy1].[A1] AS [C1]
    FROM ( SELECT 
        COUNT(1) AS [A1]
        FROM [dbo].[wt_adm_area] AS [Extent1]
    )  AS [GroupBy1]
           */
            var result = _admAreaRepository.Count();
            Assert.GreaterOrEqual(result, 0);
            Console.WriteLine(result);
        }

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        [Test]
        public async Task AdmArea_CountAsync_Test()
        {
            var result = await _admAreaRepository.CountAsync();
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
            /*SELECT 
    [GroupBy1].[A1]
        AS[C1]
FROM(SELECT
 COUNT(1) AS[A1]
 FROM[dbo].[wt_adm_area]
        AS[Extent1]
WHERE([Extent1].[Id] > 100) AND([Extent1].[Id] < 1000)
    )  AS[GroupBy1]
    */
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
        public async Task AdmArea_CountAsync_Predicate_Test()
        {
            Expression<Func<AdmArea, bool>> predicate = x => x.Id > 100 && x.Id < 1000;
            var result = await _admAreaRepository.CountAsync(predicate);
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
        public async Task AdmArea_LongCountAsync_Test()
        {
            var result = await _admAreaRepository.LongCountAsync();
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
            /*
             SELECT 
                [GroupBy1].[A1] AS [C1]
                FROM ( SELECT 
                    COUNT_BIG(1) AS [A1]
                    FROM [dbo].[wt_adm_area] AS [Extent1]
                    WHERE ([Extent1].[Id] > 100) AND ([Extent1].[Id] < 1000)
                )  AS [GroupBy1]
             */
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
        public async Task AdmArea_LongCountAsync_Predicate_Test()
        {

            Expression<Func<AdmArea, bool>> predicate = x => x.Id > 100 && x.Id < 1000;
            var result = await _admAreaRepository.LongCountAsync(predicate);
            Assert.GreaterOrEqual(result, 10);
        }
        #endregion  Aggregates

    }
}
