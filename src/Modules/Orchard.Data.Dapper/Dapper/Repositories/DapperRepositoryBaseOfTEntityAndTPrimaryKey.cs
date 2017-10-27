using Dapper;
using DapperExtensions;
using Orchard.Data;
using Orchard.Data.Dapper.Expressions;
using Orchard.Data.Dapper.Extensions;
using Orchard.Data.Dapper.Filters.Query;
using Orchard.Domain.Entities;
using Orchard.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Orchard.Data.Dapper.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public class DapperRepositoryBase<TEntity, TPrimaryKey> :
        RepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// 
        /// </summary>
        private OrchardConnectionFactory dbFactory;

        /// <summary>
        ///     Gets the active transaction. If Dapper is active then <see cref="IUnitOfWork" /> should be started before
        ///     and there must be an active transaction.
        /// </summary>
        /// <value>
        ///     The active transaction.
        /// </value>
        private DbTransaction ActiveTransaction = null;
        /// <summary>
        /// 
        /// </summary>
        public IDapperQueryFilterExecuter DapperQueryFilterExecuter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbFactory"></param>
        public DapperRepositoryBase(OrchardConnectionFactory dbFactory)
        {
            this.dbFactory = dbFactory;
            //DapperQueryFilterExecuter = NullDapperQueryFilterExecuter.Instance;
            DapperQueryFilterExecuter = new DapperQueryFilterExecuter();
        }


        #region Select/Get/Query

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        public override TEntity Single(TPrimaryKey id)
        {
            return Single(CreateEqualityExpressionForId(id));
        }

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        public override Task<TEntity> SingleAsync(TPrimaryKey id)
        {
            return SingleAsync(CreateEqualityExpressionForId(id));
        }

        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// </summary>
        /// <param name="predicate">Entity</param>
        public override TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>(DapperQueryFilterExecuter);
            using (var conn = dbFactory.OpenDbConnection())
            {
                return conn.GetList<TEntity>(predicateGroup).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// </summary>
        /// <param name="predicate">Entity</param>
        public override async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>(DapperQueryFilterExecuter);
            using (var conn = dbFactory.OpenDbConnection())
            {
                var result = await conn.GetListAsync<TEntity>(predicateGroup);
                return result.FirstOrDefault();
            }
        }

        /// <summary>
        /// Creates an entity with given primary key without database access.
        /// </summary>
        /// <param name="id">Primary key of the entity to load</param>
        /// <returns>Entity</returns>
        public override TEntity Load(TPrimaryKey id)
        {
            return Single(id);
        }
        /// <summary>
        /// Used to get a IQueryable that is used to retrieve entities from entire table.
        /// </summary>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public override IQueryable<TEntity> GetAll()
        {
            return GetAllIncluding();
        }

        /// <summary>
        /// Used to get a IQueryable that is used to retrieve entities from entire table.
        /// One or more 
        /// </summary>
        /// <param name="propertySelectors">A list of include expressions.</param>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public override IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            PredicateGroup predicateGroup = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>();
            using (var conn = dbFactory.OpenDbConnection())
            {
                var list = conn.GetList<TEntity>(predicateGroup);


                //var query = list.AsQueryable();
                //if (!propertySelectors.IsNullOrEmpty())
                //{
                //    foreach (var propertySelector in propertySelectors)
                //    {
                //        query = query.Include(propertySelector);
                //    }
                //}
                //query = ApplyFilters(query);
                return list.AsQueryable();
            }
        }

        /// <summary>
        /// Used to get all entities.
        /// </summary>
        /// <returns>List of all entities</returns>
        public override List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        /// <summary>
        /// Used to get all entities.
        /// </summary>
        /// <returns>List of all entities</returns>
        public override async Task<List<TEntity>> GetAllListAsync()
        {
            PredicateGroup predicateGroup = new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate>()
            };
            using (var conn = dbFactory.OpenDbConnection())
            {
                var result = await conn.GetListAsync<TEntity>(predicateGroup);
                return result.ToList();
            }
        }

        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
        public override List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate, params IOrderByExpression<TEntity>[] orderByExpressions)
        {
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>(DapperQueryFilterExecuter);
            using (var conn = dbFactory.OpenDbConnection())
            {
                return conn.GetList<TEntity>(predicateGroup, sort: GetSortable(orderByExpressions)).ToList();
            }
        }
        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <param name="orderByExpressions"></param>
        /// <returns>List of all entities</returns>
        public override async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, params IOrderByExpression<TEntity>[] orderByExpressions)
        {
            //return ApplyOrderBy(GetAll().Where(predicate), orderByExpressions).ToListAsync();
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>(DapperQueryFilterExecuter);
            using (var conn = dbFactory.OpenDbConnection())
            {
                var result = await conn.GetListAsync<TEntity>(predicateGroup, sort: GetSortable(orderByExpressions));
                return result.ToList();
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

        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        public override TEntity Insert(TEntity entity)
        {
            using (var conn = dbFactory.OpenDbConnection())
            {
                TPrimaryKey primaryKey = conn.Insert(entity, ActiveTransaction);
                if (entity.IsTransient())
                {
                    entity.Id = primaryKey;
                }
                return entity;
            }

        }

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        public override async Task<TEntity> InsertAsync(TEntity entity)
        {
            using (var conn = dbFactory.OpenDbConnection())
            {
                TPrimaryKey primaryKey = await conn.InsertAsync(entity, ActiveTransaction);
                if (entity.IsTransient())
                {
                    entity.Id = primaryKey;
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
            using (var conn = dbFactory.OpenDbConnection())
            {
                TPrimaryKey primaryKey = conn.Insert(entity, ActiveTransaction);
                return primaryKey;
            }
        }

        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        public override async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            using (var conn = dbFactory.OpenDbConnection())
            {
                TPrimaryKey primaryKey = await conn.InsertAsync(entity, ActiveTransaction);
                return primaryKey;
            }
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
        public override Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            return entity.IsTransient()
               ? InsertAsync(entity)
               : UpdateAsync(entity);
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
            entity = InsertOrUpdate(entity);
            return entity.Id;
        }

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// Also returns Id of the entity.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        public override async Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
        {
            entity = await InsertOrUpdateAsync(entity);
            return entity.Id;
        }

        /// <summary>
        /// Inserts  new entities.
        /// </summary>
        /// <param name="entities">Inserted entities</param>
        public override int InsertList(IEnumerable<TEntity> entities)
        {
            using (var conn = dbFactory.OpenDbConnection())
            {
                conn.Insert(entities);
                return entities.Count();
            }
        }

        /// <summary>
        /// Inserts  new entities.
        /// </summary>
        /// <param name="entities">Inserted entities</param>
        public override async Task<int> InsertListAsync(IEnumerable<TEntity> entities)
        {
            using (var conn = dbFactory.OpenDbConnection())
            {
                await conn.InsertAsync(entities, ActiveTransaction);
                return entities.Count();
            }
        }
        #endregion

        #region Update

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        public override TEntity Update(TEntity entity)
        {
            using (var conn = dbFactory.OpenDbConnection())
            {
                conn.Update(entity, ActiveTransaction);
                return entity;
            }
        }

        /// <summary>
        /// Updates an existing entity. 
        /// </summary>
        /// <param name="entity">Entity</param>
        public override async Task<TEntity> UpdateAsync(TEntity entity)
        {
            using (var conn = dbFactory.OpenDbConnection())
            {
                await conn.UpdateAsync(entity, ActiveTransaction);
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
            using (var conn = dbFactory.OpenDbConnection())
            {
                var entity = Single(id);
                updateAction(entity);
                return entity;
            }
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="updateAction">Action that can be used to change values of the entity</param>
        /// <returns>Updated entity</returns>
        public override async Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task<TEntity>> updateAction)
        {
            var entity = await SingleAsync(id);
            await updateAction(entity);
            return entity;
        }

        /// <summary>
        /// Updates entities
        /// </summary>
        /// <param name="updateExpression">The update expression</param>
        /// <param name="predicate"><![CDATA[ Expression<Func<TEntity, bool>>  ]]>predicate</param>
        public override int Update(Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();

        }
        /// <summary>
        /// Updates entities
        /// </summary>
        /// <param name="updateExpression">The update expression</param>
        /// <param name="predicate"><![CDATA[ Expression<Func<TEntity, bool>>  ]]>predicate</param>
        public override async Task<int> UpdateAsync(Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> predicate)
        {
            //conn.BulkUpdate(orders, order => order.Items)
            //return await Task.FromResult(1);

            await Task.Factory.StartNew(() =>
            {

            });
            throw new NotImplementedException();
        }
        #endregion

        #region Delete

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public override int Delete(TEntity entity)
        {
            using (var conn = dbFactory.OpenDbConnection())
            {
                return conn.Delete(entity, ActiveTransaction) ? 1 : 0;
            }
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public override async Task<int> DeleteAsync(TEntity entity)
        {
            using (var conn = dbFactory.OpenDbConnection())
            {
                return await conn.DeleteAsync(entity, ActiveTransaction) ? 1 : 0;
            }
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        public override int Delete(TPrimaryKey id)
        {
            Expression<Func<TEntity, bool>> predicate = CreateEqualityExpressionForId(id);
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>();
            using (var conn = dbFactory.OpenDbConnection())
            {
                return conn.Delete<TEntity>(predicateGroup, ActiveTransaction) ? 1 : 0;
            }
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        public override async Task<int> DeleteAsync(TPrimaryKey id)
        {
            Expression<Func<TEntity, bool>> predicate = CreateEqualityExpressionForId(id);
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>();
            using (var conn = dbFactory.OpenDbConnection())
            {
                return await conn.DeleteAsync<TEntity>(predicateGroup, ActiveTransaction) ? 1 : 0;

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
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>();
            using (var conn = dbFactory.OpenDbConnection())
            {
                return conn.Delete<TEntity>(predicateGroup, ActiveTransaction) ? 1 : 0;
            }
        }

        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        public override async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>();
            using (var conn = dbFactory.OpenDbConnection())
            {
                return await conn.DeleteAsync<TEntity>(predicateGroup, ActiveTransaction) ? 1 : 0;
            }
        }

        #endregion

        #region Aggregates

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override int Count()
        {
            using (var conn = dbFactory.OpenDbConnection())
            {
                IPredicate predicateGroup = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>();
                return conn.Count<TEntity>(predicateGroup);
            }
        }

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override async Task<int> CountAsync()
        {
            IPredicate predicateGroup = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>();
            using (var conn = dbFactory.OpenDbConnection())
            {
                return await conn.CountAsync<TEntity>(predicateGroup);
            }
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public override int Count(Expression<Func<TEntity, bool>> predicate)
        {
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>(DapperQueryFilterExecuter);
            using (var conn = dbFactory.OpenDbConnection())
            {
                return conn.Count<TEntity>(predicateGroup);
            }
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public override async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>(DapperQueryFilterExecuter);
            using (var conn = dbFactory.OpenDbConnection())
            {
                return await conn.CountAsync<TEntity>(predicateGroup);
            }
        }

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override long LongCount()
        {
            IPredicate predicateGroup = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>();
            using (var conn = dbFactory.OpenDbConnection())
            {
                return conn.Count<TEntity>(predicateGroup);
            }
        }

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override async Task<long> LongCountAsync()
        {
            IPredicate predicateGroup = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>();
            using (var conn = dbFactory.OpenDbConnection())
            {
                var result = await conn.CountAsync<TEntity>(predicateGroup);
                return result;
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
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>(DapperQueryFilterExecuter);
            using (var conn = dbFactory.OpenDbConnection())
            {
                return conn.Count<TEntity>(predicateGroup);
            }
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public override async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>(DapperQueryFilterExecuter);
            using (var conn = dbFactory.OpenDbConnection())
            {
                var result = await conn.CountAsync<TEntity>(predicateGroup);
                return result;
            }
        }
        #endregion  Aggregates

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
            if (pageIndex < 0) pageIndex = 0;
            if (pageSize < 1) pageSize = 20;
            int totalPages = (totalCount / pageSize);
            if (totalCount % pageSize != 0) totalPages += 1;
            pageIndex = pageIndex - 1;
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>(DapperQueryFilterExecuter);
            using (var conn = dbFactory.OpenDbConnection())
            {
                return conn.GetPage<TEntity>(predicateGroup, GetSortable(orderByExpressions), pageIndex, pageSize).ToList();
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
        public override async Task<List<TEntity>> GetPaggingListAsync(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize, IOrderByExpression<TEntity>[] orderByExpressions)
        {
            int totalCount = 0;
            if (pageIndex < 0) pageIndex = 0;
            if (pageSize < 1) pageSize = 20;
            int totalPages = (totalCount / pageSize);
            if (totalCount % pageSize != 0) totalPages += 1;
            pageIndex = pageIndex - 1;
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>(DapperQueryFilterExecuter);
            using (var conn = dbFactory.OpenDbConnection())
            {
                var result = await conn.GetPageAsync<TEntity>(predicateGroup, GetSortable(orderByExpressions), pageIndex, pageSize);
                return result.ToList();
            }
        }
        /// <summary>
        ///     Queries the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Query(string query, object parameters = null)
        {
            using (var conn = dbFactory.OpenDbConnection())
            {
                return conn.Query<TEntity>(query, parameters, ActiveTransaction);
            }
        }

        /// <summary>
        ///     Queries the asynchronous.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public async virtual Task<IEnumerable<TEntity>> QueryAsync(string query, object parameters = null)
        {
            using (var conn = dbFactory.OpenDbConnection())
            {
                return await conn.QueryAsync<TEntity>(query, parameters, ActiveTransaction);
            }
        }

        /// <summary>
        ///     Queries the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public virtual IEnumerable<TAny> Query<TAny>(string query, object parameters = null)
        {
            using (var conn = dbFactory.OpenDbConnection())
            {
                return conn.Query<TAny>(query, parameters, ActiveTransaction);
            }
        }

        /// <summary>
        ///     Queries the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public async virtual Task<IEnumerable<TAny>> QueryAsync<TAny>(string query, object parameters = null)
        {
            using (var conn = dbFactory.OpenDbConnection())
            {
                return await conn.QueryAsync<TAny>(query, parameters, ActiveTransaction);
            }
        }

        /// <summary>
        ///     Gets the set.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="firstResult">The first result.</param>
        /// <param name="maxResults">The maximum results.</param>
        /// <param name="sortingProperty">The sorting property.</param>
        /// <param name="ascending">if set to <c>true</c> [ascending].</param>
        /// <returns></returns>

        public virtual IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults, string sortingProperty, bool ascending = true)
        {
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>(DapperQueryFilterExecuter);
            using (var conn = dbFactory.OpenDbConnection())
            {
                return conn.GetSet<TEntity>(
                    predicateGroup,
                    new List<ISort> { new Sort { Ascending = ascending, PropertyName = sortingProperty } },
                    firstResult,
                    maxResults,
                    ActiveTransaction
                );
            }
        }
        /// <summary>
        ///     Gets the set.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="firstResult">The first result.</param>
        /// <param name="maxResults">The maximum results.</param>
        /// <param name="ascending">if set to <c>true</c> [ascending].</param>
        /// <param name="sortingExpression">The sorting expression.</param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults, bool ascending = true, params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            IPredicate predicateGroup = predicate.ToPredicateGroup<TEntity, TPrimaryKey>(DapperQueryFilterExecuter);
            using (var conn = dbFactory.OpenDbConnection())
            {
                return conn.GetSet<TEntity>(predicateGroup, sortingExpression.ToSortable(ascending), firstResult, maxResults, ActiveTransaction);
            }
        }
        private List<ISort> GetSortable(params IOrderByExpression<TEntity>[] orderByExpressions)
        {
            if (orderByExpressions == null || orderByExpressions.Count() == 0) return null;
            var sortList = new List<ISort>();
            foreach (var item in orderByExpressions)
            {
                var objSort = item.GetCustomSort<Sort>("Ascending", "PropertyName");
                if (objSort != null && !string.IsNullOrEmpty(objSort.PropertyName))
                {
                    sortList.Add(objSort);
                }
            }
            return sortList;
        }
    }
}
