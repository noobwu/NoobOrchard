using Newtonsoft.Json.Linq;
using OrchardNorthwind.Common.Entities;
using OrchardNorthwind.IServices;
using OrchardNorthwind.Services.GrainInterfaces;
using NoobOrleans.Core;
using Orchard.Data;
using Orchard.Orleans;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace OrchardNorthwind.Services.Grains
{

    /// <summary>
    /// TerritoriesServiceGrain相关操作
    /// </summary>
    public  partial class TerritoryServiceGrain:ServiceGrainBase<Territory,string>,ITerritoryGrain
    {
        #region Fields
        private ITerritoryService service;
        #endregion Fields

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="containerContext">containerContext</param>
        /// <param name="grainFactory">grainFactory</param>
        public TerritoryServiceGrain(ContainerContext containerContext, IGrainFactory grainFactory):base(containerContext,grainFactory)
        {
             service = Container.Resolve<ITerritoryService>();
        }

        #endregion Ctor
        
        #region Select/Get/Query
        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <return awaits>Entity</return awaits>
        public virtual async Task<Territory> SingleAsync(string id)
        {
            return await service.SingleAsync(id);
        }
        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// Throws exception if no entity or more than one entity.
        /// </summary>
        /// <param name="jsonPredicate">A condition to filter entities</param>
        public virtual async Task<Territory> SingleAsync(JObject jsonPredicate)
        {
            Expression<Func<Territory, bool>> predicate = jsonPredicate.ToPredicate<Territory>();
            return await service.SingleAsync(predicate);
        }


        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="jsonPredicate">A condition to filter entities</param>
        /// <param name="jsonOrderByExpressions">order by </param>
        /// <return awaits>List of all entities</return awaits>
        public virtual async Task<List<Territory>> GetListAsync(JObject jsonPredicate,params JObject[] jsonOrderByExpressions)
        {
            Expression<Func<Territory, bool>> predicate = jsonPredicate.ToPredicate<Territory>();
            IOrderByExpression<Territory>[] orderByExpressions = jsonOrderByExpressions.ToOrderByArray<Territory>();
            return await service.GetListAsync(predicate, orderByExpressions);
        }

        /// <summary>
        /// Used to run a query over entire entities.
        /// <see cref="UnitOfWorkAttribute"/> attribute is not always necessary (as opposite to <see cref="GetAll"/>)
        /// if <paramref name="queryMethod"/> finishes IQueryable with ToList, FirstOrDefault etc..
        /// </summary>
        /// <typeparam name="T">Type of return await value of this method</typeparam>
        /// <param name="queryMethod">This method is used to query over entities</param>
        /// <return awaits>Query result</return awaits>
        public virtual Task<T> Query<T>(Func<IQueryable<Territory>, T> queryMethod)
        {
            return Task.FromResult(service.Query(queryMethod));
        }
        /// <summary>
        ///     Gets the list paged.
        /// </summary>
        /// <param name="jsonPredicate">The predicate.</param>
        /// <param name="pageIndex">The page number.</param>
        /// <param name="pageSize">The items per page.</param>
        /// <param name="jsonOrderByExpressions">order Expression</param>
        /// <return awaits></return awaits>
        public virtual async Task<List<Territory>> GetPaggingListAsync(JObject jsonPredicate, int pageIndex, int pageSize,JObject[] jsonOrderByExpressions)
        {
            Expression<Func<Territory, bool>> predicate = jsonPredicate.ToPredicate<Territory>();
            IOrderByExpression<Territory>[] orderByExpressions = jsonOrderByExpressions.ToOrderByArray<Territory>();
            return await service.GetPaggingListAsync(predicate, pageIndex, pageSize, orderByExpressions);
        }
        /// <summary>
        /// check condition there  Exists
        /// </summary>
        /// <param name="jsonPredicate">A condition to filter entities</param>
        /// <return awaits></return awaits>
        public virtual async Task<bool> Exists(JObject jsonPredicate)
        {
            Expression<Func<Territory, bool>> predicate = jsonPredicate.ToPredicate<Territory>();
            return await service.CountAsync(predicate) > 0;
        }
        /// <summary>
        /// check condition there  Exists
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <return awaits></return awaits>
        public virtual async Task<bool> ExistsAsync(JObject jsonPredicate)
        {
            Expression<Func<Territory, bool>> predicate = jsonPredicate.ToPredicate<Territory>();
            return await service.CountAsync(predicate) > 0;
        }
        #endregion  Select/Get/Query
        #region Insert
        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        public virtual async Task<Territory> InsertAsync(Territory entity)
        {
            return await service.InsertAsync(entity);
        }
        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <return awaits>Id of the entity</return awaits>
        public virtual async Task<string> InsertAndGetIdAsync(Territory entity)
        {
            return await service.InsertAndGetIdAsync(entity);
        }

        /// <summary>
        /// Inserts  new entities.
        /// </summary>
        /// <param name="entities">Inserted entities</param>
        public virtual async Task<int> InsertListAsync(IEnumerable<Territory> entities)
        {
            return await service.InsertListAsync(entities);
        }

        #endregion Insert
        #region Update

        /// <summary>
        /// Updates an existing entity. 
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task<Territory> UpdateAsync(Territory entity)
        {
            return await service.UpdateAsync(entity);
        }
        /// <summary>
        /// Updates entities
        /// </summary>
        /// <param name="jsonUpdateExpression">The update expression</param>
        /// <param name="jsonPredicate"><![CDATA[ Expression<Func<Territory, bool>>  ]]>jsonPredicate</param>
        public virtual async Task<int> UpdateAsync(JObject jsonUpdateExpression, JObject jsonPredicate)
        {
            Expression<Func<Territory, bool>> predicate = jsonPredicate.ToPredicate<Territory>();
            var updateExpression = jsonUpdateExpression.ToUpdateExpression<Territory>();
            return await service.UpdateAsync(updateExpression, predicate);
        }
        #endregion Update

        #region  Delete
        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public virtual async Task<int> DeleteAsync(Territory entity)
        {
            return await service.DeleteAsync(entity);
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        public virtual async Task<int> DeleteAsync(string id)
        {
            return await service.DeleteAsync(id);
        }
        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="jsonPredicate">A condition to filter entities</param>
        public virtual async Task<int> DeleteAsync(JObject jsonPredicate)
        {
            Expression<Func<Territory, bool>> predicate = jsonPredicate.ToPredicate<Territory>();
            return await service.DeleteAsync(predicate);
        }
        #endregion Delete

        #region Aggregates
        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <return awaits>Count of entities</return awaits>
        public virtual async Task<int> CountAsync()
        {
            return await service.CountAsync();
        }
        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <return awaits>Count of entities</return awaits>
        public virtual async Task<int> CountAsync(JObject jsonPredicate)
        {
            Expression<Func<Territory, bool>> predicate = jsonPredicate.ToPredicate<Territory>();
            return await service.CountAsync(predicate);
        }
        /// <summary>
        /// Gets count of all entities in this repository (use if expected return await value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <return awaits>Count of entities</return awaits>
        public virtual async Task<long> LongCountAsync()
        {
            return await service.LongCountAsync();
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return await value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="jsonPredicate">A method to filter count</param>
        /// <return awaits>Count of entities</return awaits>
        public virtual async Task<long> LongCountAsync(JObject jsonPredicate)
        {
            Expression<Func<Territory, bool>> predicate = jsonPredicate.ToPredicate<Territory>();
            return await service.LongCountAsync(predicate);
        }
        #endregion Aggregates
        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
           
        }
    }
}
