using Orchard.Data;
using Orchard.Domain.Entities;
using Orchard.Domain.Repositories;
using Orchard.Domain.Uow;
using Orchard.IServices;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Orchard.Services
{
    /// <summary>
    /// 服务相关操作
    /// </summary>
    public abstract class ServiceBase<TEntity, TPrimaryKey> : IServiceBase<TEntity, TPrimaryKey>
     where TEntity : class, IEntity<TPrimaryKey>
    {
        #region Fields
        private IRepository<TEntity, TPrimaryKey> _repository;
        protected readonly ILogger logger;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">TEntity repository</param>
        public ServiceBase(IRepository<TEntity, TPrimaryKey> repository, ILoggerFactory loggerFactory = null)
        {
            _repository = repository;
            if (loggerFactory != null)
            {
                logger = loggerFactory.GetLogger(this.GetType());
            }
        }

        #endregion
        #region Select/Get/Query
        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <return awaits>Entity</return awaits>
        public virtual async Task<TEntity> SingleAsync(TPrimaryKey id)
        {
            return await _repository.SingleAsync(id);
        }
        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// Throws exception if no entity or more than one entity.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        public virtual async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _repository.SingleAsync(predicate);
        }


        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <return awaits>List of all entities</return awaits>
        public virtual async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, params IOrderByExpression<TEntity>[] orderByExpressions)
        {
            return await _repository.GetListAsync(predicate, orderByExpressions);
        }

        /// <summary>
        /// Used to run a query over entire entities.
        /// <see cref="UnitOfWorkAttribute"/> attribute is not always necessary (as opposite to <see cref="GetAll"/>)
        /// if <paramref name="queryMethod"/> finishes IQueryable with ToList, FirstOrDefault etc..
        /// </summary>
        /// <typeparam name="T">Type of return await value of this method</typeparam>
        /// <param name="queryMethod">This method is used to query over entities</param>
        /// <return awaits>Query result</return awaits>
        public virtual T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return _repository.Query(queryMethod);
        }
        /// <summary>
        ///     Gets the list paged.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="pageIndex">The page number.</param>
        /// <param name="pageSize">The items per page.</param>
        /// <param name="orderByExpressions">order Expression</param>
        /// <return awaits></return awaits>
        public virtual async Task<List<TEntity>> GetPaggingListAsync(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize, IOrderByExpression<TEntity>[] orderByExpressions)
        {
            return await _repository.GetPaggingListAsync(predicate, pageIndex, pageSize, orderByExpressions);
        }
        /// <summary>
        /// check condition there  Exists
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <return awaits></return awaits>
        public virtual bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return _repository.Count(predicate) > 0;
        }
        /// <summary>
        /// check condition there  Exists
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <return awaits></return awaits>
        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _repository.CountAsync(predicate) > 0;
        }
        #endregion  Select/Get/Query
        #region Insert
        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            return await _repository.InsertAsync(entity);
        }
        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <return awaits>Id of the entity</return awaits>
        public virtual async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            return await _repository.InsertAndGetIdAsync(entity);
        }

        /// <summary>
        /// Inserts  new entities.
        /// </summary>
        /// <param name="entities">Inserted entities</param>
        public virtual async Task<int> InsertListAsync(IEnumerable<TEntity> entities)
        {
            return await _repository.InsertListAsync(entities);
        }

        #endregion Insert
        #region Update

        /// <summary>
        /// Updates an existing entity. 
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            return await _repository.UpdateAsync(entity);
        }
        /// <summary>
        /// Updates entities
        /// </summary>
        /// <param name="updateExpression">The update expression</param>
        /// <param name="predicate"><![CDATA[ Expression<Func<TEntity, bool>>  ]]>predicate</param>
        public virtual async Task<int> UpdateAsync(Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> predicate)
        {
            return await _repository.UpdateAsync(updateExpression, predicate);
        }
        #endregion Update

        #region  Delete
        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public virtual async Task<int> DeleteAsync(TEntity entity)
        {
            return await _repository.DeleteAsync(entity);
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        public virtual async Task<int> DeleteAsync(TPrimaryKey id)
        {
            return await _repository.DeleteAsync(id);
        }
        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        public virtual async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _repository.DeleteAsync(predicate);
        }
        #endregion Delete

        #region Aggregates
        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <return awaits>Count of entities</return awaits>
        public virtual async Task<int> CountAsync()
        {
            return await _repository.CountAsync();
        }
        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <return awaits>Count of entities</return awaits>
        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _repository.CountAsync(predicate);
        }
        /// <summary>
        /// Gets count of all entities in this repository (use if expected return await value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <return awaits>Count of entities</return awaits>
        public virtual async Task<long> LongCountAsync()
        {
            return await _repository.LongCountAsync();
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return await value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <return awaits>Count of entities</return awaits>
        public virtual async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _repository.LongCountAsync(predicate);
        }
        #endregion Aggregates
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {

        }
    }
}
