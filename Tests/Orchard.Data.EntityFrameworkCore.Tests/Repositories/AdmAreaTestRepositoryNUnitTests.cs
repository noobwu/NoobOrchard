using Autofac;
using Orchard.Tests.Common.Domain.Entities;
using Orchard.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Orchard.Data.EntityFrameworkCore.Tests
{

    /// <summary>
    /// wt_adm_area_test测试
    /// </summary>
    [TestFixture]
    [Category("BTV")]
    [Category("EfCoreRepositories")]
    [Category("AdmAreaTest")]
    public partial class AdmAreaTestRepositoryNUnitTests : EfCoreNUnitTestBase
    {
        private Domain.Repositories.IRepository<AdmAreaTest> repository;
        protected override void Register(ContainerBuilder builder)
        {

        }
        protected override void Init()
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
        [TestCase]
        public async Task SingleAsync_Test()
        {
            AdmAreaTest entity = InsertData();
            int id = entity.Id;
            var result = await repository.SingleAsync(id);
            Assert.AreEqual(result.Id, id);
        }

        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// Throws exception if no entity or more than one entity.
        /// </summary>
        /// <param name="predicate">Entity</param>
	    [TestCase]
        public async Task SingleAsync_Predicate_Test()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = a => a.Id == entity.Id;
            var result = await repository.SingleAsync(predicate);
            Assert.AreEqual(result, entity);
        }


        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
		[TestCase]

        public async Task GetListAsync_Test()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = a => a.Id == entity.Id;
            IOrderByExpression<AdmAreaTest>[] orderByExpressions ={
                  new OrderByExpression<AdmAreaTest,int>(a=>a.Id)
            };
            var result = await repository.GetListAsync(predicate, orderByExpressions);
            Assert.AreEqual(1, result.Count);
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
        public void Query_Test()
        {
            AdmAreaTest entity = InsertData();
            Func<IQueryable<AdmAreaTest>, AdmAreaTest> queryMethod = q => q.Where(x => 1 == 1).FirstOrDefault();
            var result = repository.Query(queryMethod);
            Assert.NotNull(result);
        }
        /// <summary>
        ///     Gets the list paged.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="pageIndex">The page number.</param>
        /// <param name="pageSize">The items per page.</param>
        /// <param name="orderByExpressions">order Expression</param>
        /// <returns></returns>
        [TestCase]
        public async Task GetPaggingListAsync_Test()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = PredicateBuilder.True<AdmAreaTest>();
            IOrderByExpression<AdmAreaTest>[] orderByExpressions = {
                  new OrderByExpression<AdmAreaTest,int>(a=>a.Id)
            };
            int pageIndex = PageIndex;
            int pageSize = PageSize;
            var result = await repository.GetPaggingListAsync(predicate, pageIndex, pageSize, orderByExpressions);
            Assert.NotNull(result);
        }
        /// <summary>
        /// check condition there  Exists
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns></returns>
        [TestCase]
        public void Exists_Test()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = PredicateBuilder.True<AdmAreaTest>();
            var result = repository.Count(predicate) > 0;
            Assert.IsTrue(result);
        }
        /// <summary>
        /// check condition there  Exists
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns></returns>
	    [TestCase]
        public async Task ExistsAsync_Test()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = PredicateBuilder.True<AdmAreaTest>();
            var result = await repository.CountAsync(predicate) > 0;
            Assert.IsTrue(result);
        }
        #endregion  Select/Get/Query
        #region Insert
        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
	    [TestCase]
        public async Task InsertAsync_Test()
        {
            AdmAreaTest entity = CreateAdmAreaTest();
            var result = await repository.InsertAsync(entity);
            Assert.NotNull(result);
        }
        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
	    [TestCase]
        public async Task InsertAndGetIdAsync_Test()
        {
            AdmAreaTest entity = CreateAdmAreaTest();
            var result = await repository.InsertAndGetIdAsync(entity);
            Assert.IsTrue(result > 0);
        }

        /// <summary>
        /// Inserts  new entities.
        /// </summary>
        /// <param name="entities">Inserted entities</param>
	    [TestCase]
        public async Task InsertListAsync_Test()
        {
            IEnumerable<AdmAreaTest> entities = new AdmAreaTest[] { CreateAdmAreaTest() };
            var result = await repository.InsertListAsync(entities);
            Assert.IsTrue(result > 0);
        }

        #endregion Insert
        #region Update

        /// <summary>
        /// Updates an existing entity. 
        /// </summary>
        /// <param name="entity">Entity</param>
		[TestCase]

        public async Task UpdateAsync_Test()
        {
            AdmAreaTest entity = InsertData();
            var result = await repository.UpdateAsync(entity);
            Assert.NotNull(result);
        }
        /// <summary>
        /// Updates entities
        /// </summary>
        /// <param name="updateExpression">The update expression</param>
        /// <param name="predicate"><![CDATA[ Expression<Func<AdmAreaTest, bool>>  ]]>predicate</param>
	    [TestCase]
        public async Task UpdateAsync_Predicate_Test()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = a => a.Id == entity.Id;
            Expression<Func<AdmAreaTest, AdmAreaTest>> updateExpression = a => new AdmAreaTest
            {
                AreaId = entity.AreaId
            };
            var result = await repository.UpdateAsync(updateExpression, predicate);
            Assert.IsTrue(result > 0);
        }
        #endregion Update

        #region  Delete
        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
		[TestCase]
        public async Task DeleteAsync_Test()
        {
            AdmAreaTest entity = InsertData();
            var result = await repository.DeleteAsync(entity);
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
		[TestCase]
        public async Task DeleteAsync_Id_Test()
        {
            AdmAreaTest entity = InsertData();
            int id = entity.Id;
            var result = await repository.DeleteAsync(id);
            Assert.AreEqual(1, result);
        }
        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
		[TestCase]
        public async Task DeleteAsync_Predicate_Test()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = a => a.Id == entity.Id;
            var result = await repository.DeleteAsync(predicate);
            Assert.AreEqual(1, result);
        }
        #endregion Delete

        #region Aggregates
        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
		[TestCase]
        public async Task CountAsync_Test()
        {
            AdmAreaTest entity = InsertData();
            var result = await repository.CountAsync();
            Assert.IsTrue(result > 0);
        }
        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
		[TestCase]
        public async Task CountAsync_Predicate_Test()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = PredicateBuilder.True<AdmAreaTest>();
            var result = await repository.CountAsync(predicate);
            Assert.IsTrue(result > 0);
        }
        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
		[TestCase]
        public async Task LongCountAsync_Test()
        {
            AdmAreaTest entity = InsertData();
            var result = await repository.LongCountAsync();
            Assert.IsTrue(result > 0);
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
		[TestCase]
        public async Task LongCountAsync_Predicate_Test()
        {
            AdmAreaTest entity = InsertData();
            Expression<Func<AdmAreaTest, bool>> predicate = PredicateBuilder.True<AdmAreaTest>();
            var result = await repository.LongCountAsync(predicate);
            Assert.IsTrue(result > 0);
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

    }
}
