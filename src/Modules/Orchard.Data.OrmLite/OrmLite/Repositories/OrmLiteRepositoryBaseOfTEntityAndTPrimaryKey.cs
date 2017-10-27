using Orchard.Collections;
using Orchard.Domain.Entities;
using Orchard.Domain.Repositories;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace Orchard.Data.OrmLite.Repositories
{
    /// <summary>
    /// Base class for all repositories those uses NHibernate.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public  class OrmLiteRepositoryBase<TEntity, TPrimaryKey> :
        RepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// providing  connection factory
        /// </summary>
        public OrmLiteConnectionFactory DbFactory { get; private set; }


        /// <summary>
        /// Creates a new <see cref="OrmLiteRepositoryBase{TEntity,TPrimaryKey}"/> object.
        /// </summary>
        /// <param name="dbFactory">providing  connection factory</param>
        public   OrmLiteRepositoryBase(OrmLiteConnectionFactory dbFactory)
        {
            DbFactory = dbFactory;

        }
        #region Select/Get/Query
        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        public override TEntity Single(TPrimaryKey id)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.SingleById<TEntity>(id);
            }
        }
        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        public override  async  Task<TEntity> SingleAsync(TPrimaryKey id)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return await db.SingleByIdAsync<TEntity>(id);
            }
        }
        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// Throws exception if no entity or more than one entity.
        /// </summary>
        /// <param name="predicate">Entity</param>
        public override TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.Single(predicate);
            }
        }
        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// Throws exception if no entity or more than one entity.
        /// </summary>
        /// <param name="predicate">Entity</param>
        public override  async  Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return await db.SingleAsync(predicate);
            }
        }
        /// <summary>
        /// Creates an entity with given primary key without database access.
        /// </summary>
        /// <param name="id">Primary key of the entity to load</param>
        /// <returns>Entity</returns>
        public override TEntity Load(TPrimaryKey id)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.LoadSingleById<TEntity>(id);
            }
        }
        /// <summary>
        /// Used to get a IQueryable that is used to retrieve entities from entire table.
        /// </summary>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public override IQueryable<TEntity> GetAll()
        {
            var db = DbFactory.OpenDbConnection();
            return db.SelectLazy<TEntity>().AsQueryable();
        }

        /// <summary>
        /// Used to get a IQueryable that is used to retrieve entities from entire table.
        /// One or more 
        /// </summary>
        /// <param name="propertySelectors">A list of include expressions.</param>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public override IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            if (propertySelectors.IsNullOrEmpty())
            {
                return GetAll();
            }
            var db = DbFactory.OpenDbConnection();
            SqlExpression<TEntity> sqlExpression = db.From<TEntity>();
            foreach (var propertySelector in propertySelectors)
            {
                //TODO: Test if OrmLite supports multiple fetch.
                sqlExpression = sqlExpression.Select(propertySelector);
            }
            return db.SelectLazy(sqlExpression).AsQueryable();

        }


        /// <summary>
        /// Used to get all entities.
        /// </summary>
        /// <returns>List of all entities</returns>
        public override List<TEntity> GetAllList()
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.LoadSelect<TEntity>();
            }
        }
        /// <summary>
        /// Used to get all entities.
        /// </summary>
        /// <returns>List of all entities</returns>
        public override Task<List<TEntity>> GetAllListAsync()
        {
            return Task.FromResult(GetAllList());
        }
        /// <summary>
        /// Used to get  entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
        public override List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate, params IOrderByExpression<TEntity>[] orderByExpressions)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                SqlExpression<TEntity> sqlExpression = ApplySqlExpressionOrderBy(db.From<TEntity>().Where(predicate), orderByExpressions);
                return db.Select(sqlExpression);
            }
        }

        /// <summary>
        /// Used to get  entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
        public override Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, params IOrderByExpression<TEntity>[] orderByExpressions)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                SqlExpression<TEntity> sqlExpression = ApplySqlExpressionOrderBy(db.From<TEntity>().Where(predicate), orderByExpressions);
                return db.LoadSelectAsync(sqlExpression);
            }
        }

        /// <summary>
        /// Used to run a query over entire entities.
        /// <see cref="UnitOfWorkAttribute"/> attribute is not always necessary (as opposite to <see cref="GetAll"/>)
        /// if <paramref name="queryMethod"/> finishes IQueryable with ToList, FirstOrDefault etc..
        /// </summary>
        /// <typeparam name="T">Type of return value of this method</typeparam>
        /// <param name="queryMethod">This method is used to query over entities</param>
        /// <returns>Query result</returns>
        public override T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return queryMethod(GetAll());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public override IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, params IOrderByExpression<TEntity>[] orderByExpressions)
        {
            using (var db=DbFactory.OpenDbConnection())
            {
                SqlExpression<TEntity> sqlExpression =ApplySqlExpressionOrderBy(db.From<TEntity>().Where(predicate),orderByExpressions);
                return db.Select(sqlExpression);
            }
        }
        /// <summary>
        ///     Gets the list paged.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="pageIndex">The page number.</param>
        /// <param name="pageSize">The items per page.</param>
        /// <param name="orderByExpressions">order Expression</param>
        /// <returns></returns>
        public override List<TEntity> GetPaggingList(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize, IOrderByExpression<TEntity>[] orderByExpressions)
        {
            int totalCount = 0;
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 20;
            int totalPages = (totalCount / pageSize);
            if (totalCount % pageSize != 0) totalPages += 1;
            int startIndex = (pageIndex - 1) * pageSize;
            using (var db = DbFactory.OpenDbConnection())
            {
                SqlExpression<TEntity> sqlExpression = ApplySqlExpressionOrderBy(db.From<TEntity>().Where(predicate), orderByExpressions);
                if (pageIndex <= 1)
                {
                    sqlExpression = sqlExpression.Limit(pageSize);
                }
                else
                {
                    sqlExpression = sqlExpression.Limit(startIndex, pageSize);
                }
                return db.Select(sqlExpression);
            }
        }
        /// <summary>
        ///     Gets the list paged.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="pageIndex">The page number.</param>
        /// <param name="pageSize">The items per page.</param>
        /// <param name="orderByExpressions">order Expression</param>
        /// <returns></returns>
        public override  async  Task<List<TEntity>> GetPaggingListAsync(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize, IOrderByExpression<TEntity>[] orderByExpressions)
        {
            int totalCount = 0;
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 20;
            int totalPages = (totalCount / pageSize);
            if (totalCount % pageSize != 0) totalPages += 1;
            int startIndex = (pageIndex - 1) * pageSize;
            using (var db = DbFactory.OpenDbConnection())
            {
                SqlExpression<TEntity> sqlExpression = ApplySqlExpressionOrderBy(db.From<TEntity>().Where(predicate), orderByExpressions);
                if (pageIndex <= 1)
                {
                    sqlExpression = sqlExpression.Limit(pageSize);
                }
                else
                {
                    sqlExpression = sqlExpression.Limit(startIndex, pageSize);
                }
                return await db.SelectAsync(sqlExpression);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orderByExpressions"></param>
        /// <returns></returns>
        public SqlExpression<TEntity> ApplySqlExpressionOrderBy(SqlExpression<TEntity> query,
params IOrderByExpression<TEntity>[] orderByExpressions)
        {
            if (orderByExpressions == null || orderByExpressions.Count() == 0)
                return query;
            SqlExpression<TEntity> output = null;
            foreach (var orderByExpression in orderByExpressions)
            {
                if (output == null)
                {
                    output = orderByExpression.ApplyCustomOrderBy(query);
                }
                else
                {
                    output = orderByExpression.ApplyCustomThenBy(output);
                }
            }
            return output ?? query;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orderByExpressions"></param>
        /// <returns></returns>
        public SqlExpression<TResult> ApplySqlExpressionOrderBy<TResult>(SqlExpression<TResult> query,
params IOrderByExpression<TResult>[] orderByExpressions)
            where TResult:class
        {
            if (orderByExpressions == null || orderByExpressions.Count() == 0)
                return query;
            SqlExpression<TResult> output = null;
            foreach (var orderByExpression in orderByExpressions)
            {
                if (output == null)
                {
                    output = orderByExpression.ApplyCustomOrderBy(query);
                }
                else
                {
                    output = orderByExpression.ApplyCustomThenBy(output);
                }
            }
            return output ?? query;
        }
        #endregion  Select/Get/Query
        #region Insert
        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        public override TEntity Insert(TEntity entity)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                long result = db.Insert(entity, entity.IsTransient());
                if (entity.IsTransient())
                {
                    entity.Id = (TPrimaryKey)Convert.ChangeType(result, typeof(TPrimaryKey));
                }
                return entity;
            }
        }

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        public override  async  Task<TEntity> InsertAsync(TEntity entity)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                long result = await db.InsertAsync(entity, entity.IsTransient());
                if (entity.IsTransient())
                {
                    entity.Id = (TPrimaryKey)Convert.ChangeType(result, typeof(TPrimaryKey));
                }
                return entity;
            }
        }
        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        public override TPrimaryKey InsertAndGetId(TEntity entity)
        {
            return Insert(entity).Id;
        }

        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        public override  async  Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            entity = await InsertAsync(entity);
            return entity.Id;
        }

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="entity">Entity</param>
        public override TEntity InsertOrUpdate(TEntity entity)
        {
            return entity.IsTransient()
                ? Insert(entity)
                : Update(entity);
        }

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="entity">Entity</param>
        public override  async  Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            return entity.IsTransient()
                ? await InsertAsync(entity)
                : await UpdateAsync(entity);
        }
        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// Also returns Id of the entity.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        public override TPrimaryKey InsertOrUpdateAndGetId(TEntity entity)
        {
            return InsertOrUpdate(entity).Id;
        }
        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// Also returns Id of the entity.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        public override  async  Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
        {
            return await Task.FromResult(InsertOrUpdateAndGetId(entity));
        }
        /// <summary>
        /// Inserts  new entities.
        /// </summary>
        /// <param name="entities">Inserted entities</param>
        public override int InsertList(IEnumerable<TEntity> entities)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                db.InsertAll(entities);
                return entities.Count();
            }
        }

        /// <summary>
        /// Inserts  new entities.
        /// </summary>
        /// <param name="entities">Inserted entities</param>
        public override  async  Task<int> InsertListAsync(IEnumerable<TEntity> entities)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                await db.InsertAllAsync(entities);
            }
            return entities.Count();
        }
        #endregion Insert
        #region Update

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        public override TEntity Update(TEntity entity)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                int result = db.Update(entity);
                return entity;
            }
        }

        /// <summary>
        /// Updates an existing entity. 
        /// </summary>
        /// <param name="entity">Entity</param>
        public override  async  Task<TEntity> UpdateAsync(TEntity entity)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                int result = await db.UpdateAsync(entity);
                return entity;
            }
        }
        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="updateAction">Action that can be used to change values of the entity</param>
        /// <returns>Updated entity</returns>
        public override TEntity Update(TPrimaryKey id, Func<TEntity, TEntity> updateAction)
        {
            TEntity entity = null;
            using (var db = DbFactory.OpenDbConnection())
            {
                entity = db.SingleById<TEntity>(id);
                entity = updateAction(entity);
                db.Update(entity);
            }
            return entity;
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="updateAction">Action that can be used to change values of the entity</param>
        /// <returns>Updated entity</returns>
        public override  async  Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task<TEntity>> updateAction)
        {
            TEntity entity = null;
            var db = DbFactory.OpenDbConnection();
            entity = db.SingleById<TEntity>(id);
            entity = await updateAction(entity);
            await db.UpdateAsync(entity);
            return entity;
        }

        /// <summary>
        /// Updates entities
        /// </summary>
        /// <param name="updateExpression">The update expression</param>
        /// <param name="predicate"><![CDATA[ Expression<Func<TEntity, bool>>  ]]>predicate</param>
        public override int Update(Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> predicate)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
               return  db.UpdateOnly(updateExpression.ToUpdateFields(),predicate);
            }
        }
        /// <summary>
        /// Updates entities
        /// </summary>
        /// <param name="updateExpression">The update expression</param>
        /// <param name="predicate"><![CDATA[ Expression<Func<TEntity, bool>>  ]]>predicate</param>
        public override  async Task<int> UpdateAsync(Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> predicate)
        {
            //有问题
            using (var db = DbFactory.OpenDbConnection())
            {
                return await db.UpdateOnlyAsync(updateExpression.ToUpdateFields(), predicate);
            }
        }
        #endregion Update

        #region  Delete
        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public override int Delete(TEntity entity)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                int result = db.Delete(entity);
                return result;
            }
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public override  async  Task<int> DeleteAsync(TEntity entity)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
               return  await db.DeleteAsync(entity);
            }

        }
        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        public override int Delete(TPrimaryKey id)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
               return db.DeleteById<TEntity>(id);
            }
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        public override  async  Task<int> DeleteAsync(TPrimaryKey id)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
              return  await db.DeleteByIdAsync<TEntity>(id);
            }
        }
        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        public override int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
               return db.Delete(predicate);
            }
        }
        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        public override  async  Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
               return  await db.DeleteAsync(predicate);
            }
        }
        #endregion Delete

        #region Aggregates

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override int Count()
        {
            using (var db = DbFactory.OpenDbConnection())
            {

                return (int)db.Count<TEntity>();
            }
        }
        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override  async  Task<int> CountAsync()
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                var count = await db.CountAsync<TEntity>();
                return (int)count;
            }
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public override int Count(Expression<Func<TEntity, bool>> predicate)
        {
            using (var db = DbFactory.OpenDbConnection())
            {

                return (int)db.Count(predicate);
            }
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public override  async  Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                var count = await db.CountAsync<TEntity>(predicate);
                return (int)count;
            }
        }
        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override long LongCount()
        {
            using (var db = DbFactory.OpenDbConnection())
            {

                return db.Count<TEntity>();
            }
        }
        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override  async  Task<long> LongCountAsync()
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return await db.CountAsync<TEntity>();
            }
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public override long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.Count(predicate);
            }
        }
        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public override  async  Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return await db.CountAsync(predicate);
            }
        }
        #endregion Aggregates

    }
}
