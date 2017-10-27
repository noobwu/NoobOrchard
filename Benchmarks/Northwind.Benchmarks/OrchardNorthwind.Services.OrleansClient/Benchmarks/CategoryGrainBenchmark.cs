using BenchmarkDotNet.Attributes;
using Orchard.Data;
using Orchard.Orleans;
using Orchard.Utility;
using OrchardNorthwind.Common.Entities;
using OrchardNorthwind.Services.GrainInterfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace OrchardNorthwind.Services.OrleansClient.Benchmarks
{
    /// <summary>
    /// Categories Grain测试
    /// </summary>
    public class CategoryGrainBenchmark : OrleansBenchmarkBase
    {
        private ICategoryGrain grain;
        [GlobalSetup]
        public void GlobalSetup()
        {
            StartClient();
            grain = GrainClient.GrainFactory.GetGrain<ICategoryGrain>(GetIntGrainId());
        }
        #region Select/Get/Query
        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <return awaits>Entity</return awaits>
        [Benchmark]
        public virtual async Task<Category> SingleAsync()
        {
            int id = RandomData.GetInt();
            return await grain.SingleAsync(id);
        }
        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// Throws exception if no entity or more than one entity.
        /// </summary>
        /// <param name="jsonPredicate">A condition to filter entities</param>
        [Benchmark]
        public virtual async Task<Category> SingleAsyncByPredicate()
        {
            Expression<Func<Category, bool>> predicate = PredicateBuilder.True<Category>();
            return await grain.SingleAsync(predicate.ToJObject());
        }


        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="jsonPredicate">A condition to filter entities</param>
        /// <param name="jsonOrderByExpressions">order by </param>
        /// <return awaits>List of all entities</return awaits>
	    [Benchmark]
        public virtual async Task<List<Category>> GetListAsync()
        {
            Expression<Func<Category, bool>> predicate = PredicateBuilder.True<Category>();
            IOrderByExpression<Category>[] orderByExpressions = {
                  new OrderByExpression<Category,int>(a=>a.CategoryId)
            };
            return await grain.GetListAsync(predicate.ToJObject(), orderByExpressions.ToJObjectArray());
        }

        /// <summary>
        /// Used to run a query over entire entities.
        /// <see cref="UnitOfWorkAttribute"/> attribute is not always necessary (as opposite to <see cref="GetAll"/>)
        /// if <paramref name="queryMethod"/> finishes IQueryable with ToList, FirstOrDefault etc..
        /// </summary>
        /// <typeparam name="T">Type of return await value of this method</typeparam>
        /// <param name="queryMethod">This method is used to query over entities</param>
        /// <return awaits>Query result</return awaits>
        public virtual Task<T> Query<T>(Func<IQueryable<Category>, T> queryMethod)
        {
            return grain.Query(queryMethod);
        }
        /// <summary>
        ///     Gets the list paged.
        /// </summary>
        /// <param name="jsonPredicate">The predicate.</param>
        /// <param name="pageIndex">The page number.</param>
        /// <param name="pageSize">The items per page.</param>
        /// <param name="jsonOrderByExpressions">order Expression</param>
        /// <return awaits></return awaits>
	    [Benchmark]
        public virtual async Task<List<Category>> GetPaggingListAsync()
        {
            Expression<Func<Category, bool>> predicate = PredicateBuilder.True<Category>();
            IOrderByExpression<Category>[] orderByExpressions = {
                  new OrderByExpression<Category,int>(a=>a.CategoryId)
            };
            int pageIndex = PageIndex;
            int pageSize = PageSize;
            return await grain.GetPaggingListAsync(predicate.ToJObject(), pageIndex, pageSize, orderByExpressions.ToJObjectArray());
        }
        /// <summary>
        /// check condition there  Exists
        /// </summary>
        /// <param name="jsonPredicate">A condition to filter entities</param>
        /// <return awaits></return awaits>
		[Benchmark]
        public virtual async Task<bool> Exists()
        {
            Expression<Func<Category, bool>> predicate = PredicateBuilder.True<Category>();
            return await grain.CountAsync(predicate.ToJObject()) > 0;
        }
        /// <summary>
        /// check condition there  Exists
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <return awaits></return awaits>
	    [Benchmark]
        public virtual async Task<bool> ExistsAsync()
        {
            Expression<Func<Category, bool>> predicate = PredicateBuilder.True<Category>();
            return await grain.CountAsync(predicate.ToJObject()) > 0;
        }
        #endregion  Select/Get/Query
        #region Insert
        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        public virtual async Task<Category> InsertAsync()
        {
            Category entity = CreateCategory();
            return await grain.InsertAsync(entity);
        }
        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <return awaits>Id of the entity</return awaits>
        public virtual async Task<int> InsertAndGetIdAsync()
        {
            Category entity = CreateCategory();
            return await grain.InsertAndGetIdAsync(entity);
        }

        /// <summary>
        /// Inserts  new entities.
        /// </summary>
        /// <param name="entities">Inserted entities</param>
        public virtual async Task<int> InsertListAsync()
        {
            IEnumerable<Category> entities = new Category[] { CreateCategory() };
            return await grain.InsertListAsync(entities);
        }

        #endregion Insert
        #region Update

        /// <summary>
        /// Updates an existing entity. 
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task<Category> UpdateAsync()
        {
            Category entity = CreateCategory();
            entity = await grain.InsertAsync(entity);
            return await grain.UpdateAsync(entity);
        }
        /// <summary>
        /// Updates entities
        /// </summary>
        /// <param name="jsonUpdateExpression">The update expression</param>
        /// <param name="jsonPredicate"><![CDATA[ Expression<Func<Category, bool>>  ]]>jsonPredicate</param>
        public virtual async Task<int> UpdateAsyncByPredicate()
        {
            Category entity = CreateCategory();
            entity = await grain.InsertAsync(entity);
            Expression<Func<Category, bool>> predicate = a => a.CategoryId == entity.CategoryId;
            Expression<Func<Category, Category>> updateExpression = a => new Category
            {
                CategoryName = entity.CategoryName
            };
            return await grain.UpdateAsync(updateExpression.ToJObject<Category>(), predicate.ToJObject());
        }
        #endregion Update

        #region  Delete
        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public virtual async Task<int> DeleteAsync()
        {
            Category entity = CreateCategory();
            entity = await grain.InsertAsync(entity);
            return await grain.DeleteAsync(entity);
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        public virtual async Task<int> DeleteAsyncById()
        {
            Category entity = CreateCategory();
            entity = await grain.InsertAsync(entity);
            int id = entity.Id;
            return await grain.DeleteAsync(id);
        }
        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="jsonPredicate">A condition to filter entities</param>
        public virtual async Task<int> DeleteAsyncByPredicate()
        {
            Category entity = CreateCategory();
            entity = await grain.InsertAsync(entity);
            Expression<Func<Category, bool>> predicate = a => a.CategoryId == entity.CategoryId;
            return await grain.DeleteAsync(predicate.ToJObject());
        }
        #endregion Delete

        #region Aggregates
        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <return awaits>Count of entities</return awaits>
	    [Benchmark]
        public virtual async Task<int> CountAsync()
        {
            return await grain.CountAsync();
        }
        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <return awaits>Count of entities</return awaits>
		[Benchmark]
        public virtual async Task<int> CountAsyncByPredicate()
        {
            Expression<Func<Category, bool>> predicate = PredicateBuilder.True<Category>();
            return await grain.CountAsync(predicate.ToJObject());
        }
        /// <summary>
        /// Gets count of all entities in this repository (use if expected return await value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <return awaits>Count of entities</return awaits>
		[Benchmark]
        public virtual async Task<long> LongCountAsync()
        {
            return await grain.LongCountAsync();
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return await value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="jsonPredicate">A method to filter count</param>
        /// <return awaits>Count of entities</return awaits>
		[Benchmark]
        public virtual async Task<long> LongCountAsyncByPredicate()
        {
            Expression<Func<Category, bool>> predicate = PredicateBuilder.True<Category>();
            return await grain.LongCountAsync(predicate.ToJObject());
        }
        #endregion Aggregates

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Category CreateCategory()
        {
            Category entity = new Category()
            {
                CategoryName = RandomData.GetString(),//CategoryName
                Description = RandomData.GetString(),//Description
                Picture = default(byte[]),//Picture
            };
            return entity;
        }



    }
}
