using Newtonsoft.Json.Linq;
using Orchard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrchardNorthwind.Services.GrainInterfaces
{
    /// <summary>
    /// IGrain相关公共接口
    /// </summary>
    public interface IGrainBase<TEntity, TPrimaryKey> : IDisposable
     where TEntity : class, IEntity<TPrimaryKey>
    {
        #region Select/Get/Query
        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        Task<TEntity> SingleAsync(TPrimaryKey id);
        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// Throws exception if no entity or more than one entity.
        /// </summary>
        /// <param name="jsonPredicate">A condition to filter entities</param>
        Task<TEntity> SingleAsync(JObject jsonPredicate);

        /// <summary>
        /// Used to get all entities based on given <paramref name="jsonPredicate"/>.
        /// </summary>
        /// <param name="jsonPredicate">A condition to filter entities</param>
        /// <param name="jsonOrderByExpressions">jsonOrderByExpressions</param>
        /// <returns>List of all entities</returns>
        Task<List<TEntity>> GetListAsync(JObject jsonPredicate, params JObject[] jsonOrderByExpressions);

        /// <summary>
        /// Used to run a query over entire entities.
        /// <see cref="UnitOfWorkAttribute"/> attribute is not always necessary (as opposite to <see cref="GetAll"/>)
        /// if <paramref name="queryMethod"/> finishes IQueryable with ToList, FirstOrDefault etc..
        /// </summary>
        /// <typeparam name="T">Type of return value of this method</typeparam>
        /// <param name="queryMethod">This method is used to query over entities</param>
        /// <returns>Query result</returns>
        Task<T> Query<T>(Func<IQueryable<TEntity>, T> queryMethod);
        /// <summary>
        ///     Gets the list paged.
        /// </summary>
        /// <param name="jsonPredicate">The predicate.</param>
        /// <param name="pageIndex">The page number.</param>
        /// <param name="pageSize">The items per page.</param>
        /// <param name="jsonOrderByExpressions">jsonOrderByExpressions</param>
        /// <returns></returns>
        Task<List<TEntity>> GetPaggingListAsync(JObject jsonPredicate, int pageIndex, int pageSize, JObject[] jsonOrderByExpressions);
        /// <summary>
        /// check condition there  Exists
        /// </summary>
        /// <param name="jsonPredicate">A condition to filter entities</param>
        Task<bool> Exists(JObject jsonPredicate);
        /// <summary>
        /// check condition there  Exists
        /// </summary>
        /// <param name="jsonPredicate">A condition to filter entities</param>
        Task<bool> ExistsAsync(JObject jsonPredicate);

        #endregion  Select/Get/Query

        #region Insert
        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        Task<TEntity> InsertAsync(TEntity entity);
        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity);

        /// <summary>
        /// Inserts  new entities.
        /// </summary>
        /// <param name="entities">Inserted entities</param>
        Task<int> InsertListAsync(IEnumerable<TEntity> entities);

        #endregion Insert
        #region Update

        /// <summary>
        /// Updates an existing entity. 
        /// </summary>
        /// <param name="entity">Entity</param>
        Task<TEntity> UpdateAsync(TEntity entity);

        /// <summary>
        /// Updates entities
        /// </summary>
        /// <param name="jsonUpdateExpression">The update expression</param>
        /// <param name="jsonPredicate"><![CDATA[ Expression<Func<TEntity, bool>>  ]]>predicate</param>
        Task<int> UpdateAsync(JObject jsonUpdateExpression, JObject jsonPredicate);

        #endregion Update

        #region  Delete
        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        Task<int> DeleteAsync(TEntity entity);

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        Task<int> DeleteAsync(TPrimaryKey id);
        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="jsonPredicate">A condition to filter entities</param>
        Task<int> DeleteAsync(JObject jsonPredicate);
        #endregion Delete

        #region Aggregates
        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        Task<int> CountAsync();
        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="jsonPredicate"/>.
        /// </summary>
        /// <param name="jsonPredicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        Task<int> CountAsync(JObject jsonPredicate);
        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        Task<long> LongCountAsync();

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="jsonPredicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="jsonPredicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        Task<long> LongCountAsync(JObject jsonPredicate);
        #endregion Aggregates
    }
}
