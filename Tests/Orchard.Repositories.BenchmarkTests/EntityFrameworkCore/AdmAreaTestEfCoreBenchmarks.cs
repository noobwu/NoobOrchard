using Autofac;
using Orchard.Tests.Common.Domain.Entities;
using Orchard.Utility;
using Orchard.Data;
using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Orchard.Repositories.BenchmarkTests.EntityFrameworkCore
{

    /// <summary>
    /// wt_adm_area_test测试
    /// </summary>
    public partial class AdmAreaTestEfCoreBenchmarks : EfCoreBenchmarkBase
    {
        private Domain.Repositories.IRepository<AdmAreaTest> repository;
        protected override void Register(ContainerBuilder builder)
        {

        }
        [GlobalSetup]
        public override void Init()
        {
            base.Init();
            repository = Container.Resolve<Domain.Repositories.IRepository<AdmAreaTest>>();
        }

        #region Select/Get/Query
        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        [Benchmark]
        public async Task SingleAsync()
        {
            AdmAreaTest entity = InsertData();
            int id = entity.Id;
            var result = await repository.SingleAsync(id);
        }

        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// Throws exception if no entity or more than one entity.
        /// </summary>
        /// <param name="predicate">Entity</param>
	    [Benchmark]
        public async Task SingleAsync_Predicate()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = a => a.Id == entity.Id;
            var result = await repository.SingleAsync(predicate);
        }


        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
		[Benchmark]

        public async Task GetListAsync()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = a => a.Id == entity.Id;
            IOrderByExpression<AdmAreaTest>[] orderByExpressions ={
                  new OrderByExpression<AdmAreaTest,int>(a=>a.Id)
            };
            var result = await repository.GetListAsync(predicate, orderByExpressions);
        }

        /// <summary>
        /// Used to run a query over entire entities.
        /// <see cref="UnitOfWorkAttribute"/> attribute is not always necessary (as opposite to <see cref="GetAll"/>)
        /// if <paramref name="queryMethod"/> finishes IQueryable with ToList, FirstOrDefault etc..
        /// </summary>
        /// <typeparam name="T">Type of return value of this method</typeparam>
        /// <param name="queryMethod">This method is used to query over entities</param>
        /// <returns>Query result</returns>
        //[Benchmark]
        public void Query()
        {
            AdmAreaTest entity = InsertData();
            Func<IQueryable<AdmAreaTest>, AdmAreaTest> queryMethod = q => q.Where(x => 1 == 1).FirstOrDefault();
            var result = repository.Query(queryMethod);
        }
        /// <summary>
        ///     Gets the list paged.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="pageIndex">The page number.</param>
        /// <param name="pageSize">The items per page.</param>
        /// <param name="orderByExpressions">order Expression</param>
        /// <returns></returns>
        [Benchmark]
        public async Task GetPaggingListAsync()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = PredicateBuilder.True<AdmAreaTest>();
            IOrderByExpression<AdmAreaTest>[] orderByExpressions = {
                  new OrderByExpression<AdmAreaTest,int>(a=>a.Id)
            };
            int pageIndex = PageIndex;
            int pageSize = PageSize;
            var result = await repository.GetPaggingListAsync(predicate, pageIndex, pageSize, orderByExpressions);
        }
        /// <summary>
        /// check condition there  Exists
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns></returns>
        [Benchmark]
        public void Exists()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = PredicateBuilder.True<AdmAreaTest>();
            var result = repository.Count(predicate) > 0;
        }
        /// <summary>
        /// check condition there  Exists
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns></returns>
	    [Benchmark]
        public async Task ExistsAsync()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = PredicateBuilder.True<AdmAreaTest>();
            var result = await repository.CountAsync(predicate) > 0;
        }
        #endregion  Select/Get/Query
        #region Insert
        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
	    [Benchmark]
        public async Task InsertAsync()
        {
            AdmAreaTest entity = CreateAdmAreaTest();
            var result = await repository.InsertAsync(entity);
        }
        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
	    [Benchmark]
        public async Task InsertAndGetIdAsync()
        {
            AdmAreaTest entity = CreateAdmAreaTest();
            var result = await repository.InsertAndGetIdAsync(entity);
        }

        /// <summary>
        /// Inserts  new entities.
        /// </summary>
        /// <param name="entities">Inserted entities</param>
	    [Benchmark]
        public async Task InsertListAsync()
        {
            IEnumerable<AdmAreaTest> entities = new AdmAreaTest[] { CreateAdmAreaTest() };
            var result = await repository.InsertListAsync(entities);
        }

        #endregion Insert
        #region Update

        /// <summary>
        /// Updates an existing entity. 
        /// </summary>
        /// <param name="entity">Entity</param>
		[Benchmark]

        public async Task UpdateAsync()
        {
            AdmAreaTest entity = InsertData();
            var result = await repository.UpdateAsync(entity);
        }
        /// <summary>
        /// Updates entities
        /// </summary>
        /// <param name="updateExpression">The update expression</param>
        /// <param name="predicate"><![CDATA[ Expression<Func<AdmAreaTest, bool>>  ]]>predicate</param>
	    [Benchmark]
        public async Task UpdateAsync_Predicate()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = a => a.Id == entity.Id;
            Expression<Func<AdmAreaTest, AdmAreaTest>> updateExpression = a => new AdmAreaTest
            {
                AreaId = entity.AreaId
            };
            var result = await repository.UpdateAsync(updateExpression, predicate);
        }
        #endregion Update

        #region  Delete
        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
		[Benchmark]
        public async Task DeleteAsync()
        {
            AdmAreaTest entity = InsertData();
            var result = await repository.DeleteAsync(entity);
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
		[Benchmark]
        public async Task DeleteAsync_Id()
        {
            AdmAreaTest entity = InsertData();
            int id = entity.Id;
            var result = await repository.DeleteAsync(id);
        }
        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
		[Benchmark]
        public async Task DeleteAsync_Predicate()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = a => a.Id == entity.Id;
            var result = await repository.DeleteAsync(predicate);
        }
        #endregion Delete

        #region Aggregates
        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
		[Benchmark]
        public async Task CountAsync()
        {
            AdmAreaTest entity = InsertData();
            var result = await repository.CountAsync();
        }
        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
		[Benchmark]
        public async Task CountAsync_Predicate()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = PredicateBuilder.True<AdmAreaTest>();
            var result = await repository.CountAsync(predicate);
        }
        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
		[Benchmark]
        public async Task LongCountAsync()
        {
            AdmAreaTest entity = InsertData();
            var result = await repository.LongCountAsync();
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
		[Benchmark]
        public async Task LongCountAsync_Predicate()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = PredicateBuilder.True<AdmAreaTest>();
            var result = await repository.LongCountAsync(predicate);
        }
        #endregion Aggregates
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private AdmAreaTest InsertData()
        {
            AdmAreaTest entity = CreateAdmAreaTest();
            return repository.Insert(entity);
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

    }
}
