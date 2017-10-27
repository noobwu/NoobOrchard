using NHibernate;
using NHibernate.Linq;
using Orchard.Collections;
using Orchard.Domain.Entities;
using Orchard.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Orchard.Data.NHibernate.Repositories
{
    /// <summary>
    /// Base class for all repositories those uses NHibernate.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public class NhRepositoryBase<TEntity, TPrimaryKey> : RepositoryBase<TEntity, TPrimaryKey>
       where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Gets the NHibernate session object to perform database operations.
        /// </summary>
        private  ISession Session { get { return SessionFactory.OpenSession(); }  }

        public ISessionFactory SessionFactory { get; private set; }

        /// <summary>
        /// Creates a new <see cref="NhRepositoryBase{TEntity,TPrimaryKey}"/> object.
        /// </summary>
        /// <param name="sessionFactory">A sessionFactory provider to obtain sessionFactory for database operations</param>
        public  NhRepositoryBase(ISessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;
        }
        #region Select/Get/Query

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        public override TEntity Single(TPrimaryKey id)
        {
            return Session.Get<TEntity>(id);
        }

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        public override Task<TEntity> SingleAsync(TPrimaryKey id)
        {
            return Session.GetAsync<TEntity>(id);
        }

        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// </summary>
        /// <param name="predicate">Entity</param>
        public override TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().SingleOrDefault(predicate);
        }

        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// </summary>
        /// <param name="predicate">Entity</param>
        public override Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().SingleOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Creates an entity with given primary key without database access.
        /// </summary>
        /// <param name="id">Primary key of the entity to load</param>
        /// <returns>Entity</returns>
        public override TEntity Load(TPrimaryKey id)
        {
            return Session.Load<TEntity>(id);
        }
        /// <summary>
        /// Used to get a IQueryable that is used to retrieve entities from entire table.
        /// </summary>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public override IQueryable<TEntity> GetAll()
        {
            // return Session.Query<TEntity>().Cacheable();
            return Session.Query<TEntity>();
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

            var query = GetAll();

            foreach (var propertySelector in propertySelectors)
            {
                //TODO: Test if NHibernate supports multiple fetch.
                query = query.Fetch(propertySelector);
            }

            return query;
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
        public override Task<List<TEntity>> GetAllListAsync()
        {
            return GetAll().ToListAsync();
        }

        /// <summary>
        /// Used to get  entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
        public override List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate, params IOrderByExpression<TEntity>[] orderByExpressions)
        {
            return ApplyOrderBy(GetAll().Where(predicate),orderByExpressions).ToList();
        }

        /// <summary>
        /// Used to get  entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
        public override Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, params IOrderByExpression<TEntity>[] orderByExpressions)
        {
            return ApplyOrderBy(GetAll().Where(predicate), orderByExpressions).ToListAsync();
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
            return ApplyOrderBy(GetAll().Where(predicate), orderByExpressions).Skip(startIndex).Take(pageSize).ToList();
        }
        /// <summary>
        ///     Gets the list paged.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="pageIndex">The page number.</param>
        /// <param name="pageSize">The items per page.</param>
        /// <param name="orderByExpressions">order Expression</param>
        /// <returns></returns>
        public override Task<List<TEntity>> GetPaggingListAsync(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize, IOrderByExpression<TEntity>[] orderByExpressions)
        {
            int totalCount = 0;
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 20;
            int totalPages = (totalCount / pageSize);
            if (totalCount % pageSize != 0) totalPages += 1;
            int startIndex = (pageIndex - 1) * pageSize;
            return ApplyOrderBy(GetAll().Where(predicate), orderByExpressions).Skip(startIndex).Take(pageSize).ToListAsync();
        }
        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        public override TEntity Insert(TEntity entity)
        {
            var result = (TPrimaryKey)Session.Save(entity);
            return entity;
        }

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        public override async Task<TEntity> InsertAsync(TEntity entity)
        {
            var result =(TPrimaryKey)(await Session.SaveAsync(entity));
            return entity;
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
            entity = Insert(entity);
            if (entity.IsTransient())
            {
                Session.Flush();
            }
            return entity.Id;
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
            entity =await InsertAsync(entity);
            if (entity.IsTransient())
            {
                Session.Flush();
            }
            return entity.Id;
        }

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="entity">Entity</param>
        public override TEntity InsertOrUpdate(TEntity entity)
        {
            Session.SaveOrUpdate(entity);
            return entity;
        }

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="entity">Entity</param>
        public override async Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            await Session.SaveOrUpdateAsync(entity);
            return entity;
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
        public override async Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
        {
            return (await InsertOrUpdateAsync(entity)).Id;
        }
        /// <summary>
        /// Inserts  new entities.
        /// </summary>
        /// <param name="entities">Inserted entities</param>
        public override int InsertList(IEnumerable<TEntity> entities)
        {
            using (var s = SessionFactory.OpenStatelessSession())
            {
                try
                {
                    using (var trans = s.BeginTransaction())
                    {
                        try
                        {
                            foreach (var entity in entities)
                            {
                                s.Insert(entity);
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return entities.Count();
        }

        /// <summary>
        /// Inserts  new entities.
        /// </summary>
        /// <param name="entities">Inserted entities</param>
        public override  async  Task<int> InsertListAsync(IEnumerable<TEntity> entities)
        {
            await Task.Factory.StartNew(() =>
            {
                InsertList(entities);
            });
            return entities.Count();
        }
        #endregion

        #region Update

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        public override TEntity Update(TEntity entity)
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                session.Evict(entity);
                session.Update(entity);
                session.Flush();
                return entity;
            }
           
        }

        /// <summary>
        /// Updates an existing entity. 
        /// </summary>
        /// <param name="entity">Entity</param>
        public override Task<TEntity> UpdateAsync(TEntity entity)
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                session.EvictAsync(entity);
                session.UpdateAsync(entity);
                session.FlushAsync();
                return Task.FromResult(entity);
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
            using (ISession session = SessionFactory.OpenSession())
            {
                var entity = Single(id);
                entity = updateAction(entity);
                session.Update(entity);
                session.Flush();
                return entity;
            }
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="updateAction">Action that can be used to change values of the entity</param>
        /// <returns>Updated entity</returns>
        public override  async  Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task<TEntity>> updateAction)
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                var entity = await SingleAsync(id);
                entity = await updateAction(entity);
                await session.UpdateAsync(entity);
                await session.FlushAsync();
                return entity;
            }
        }

        /// <summary>
        /// Updates entities
        /// </summary>
        /// <param name="updateExpression">The update expression</param>
        /// <param name="predicate"><![CDATA[ Expression<Func<TEntity, bool>>  ]]>predicate</param>
        public override int Update(Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> predicate)
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                return 1;
            }
        }
        /// <summary>
        /// Updates entities
        /// </summary>
        /// <param name="updateExpression">The update expression</param>
        /// <param name="predicate"><![CDATA[ Expression<Func<TEntity, bool>>  ]]>predicate</param>
        public override  async Task<int> UpdateAsync(Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> predicate)
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                return await Task.FromResult(1);
            }
        }
        #endregion

        #region Delete

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public override int Delete(TEntity entity)
        {
            if (entity is ISoftDelete)
            {
                (entity as ISoftDelete).DeleteFlag = true;
                Update(entity);
            }
            else
            {
                using (ISession session = SessionFactory.OpenSession())
                {
                    session.Evict(entity);
                    session.Delete(entity);
                    session.Flush();
                }
            }
            return 1;
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public override Task<int> DeleteAsync(TEntity entity)
        {
            Delete(entity);
            return Task.FromResult(1);
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        public override int Delete(TPrimaryKey id)
        {
            var entity = Session.Load<TEntity>(id);
            if (entity is ISoftDelete)
            {
                (entity as ISoftDelete).DeleteFlag = true;
                Update(entity);
            }
            else
            {
                using (ISession session = SessionFactory.OpenSession())
                {
                    session.Evict(entity);
                    session.Delete(entity);
                    session.Flush();
                }
            }
            return 1;
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        public override Task<int> DeleteAsync(TPrimaryKey id)
        {
            Delete(id);
            return Task.FromResult(1);
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
            GetAll().Where(predicate).Delete() ;
            return 1;
        }

        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        public override Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Delete(predicate);
            return Task.FromResult(0);
        }

        #endregion

        #region Aggregates

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override int Count()
        {
            return GetAll().Count();
        }

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override Task<int> CountAsync()
        {
            return Task.FromResult(Count());
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public override int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).Count();
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public override Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Count(predicate));
        }

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override long LongCount()
        {
            return GetAll().LongCount();
        }

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override Task<long> LongCountAsync()
        {
            return Task.FromResult(LongCount());
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public override long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).LongCount();
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public override Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(LongCount(predicate));
        }
        #endregion  Aggregates
    }
}
