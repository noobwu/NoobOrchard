using Microsoft.EntityFrameworkCore;
using Orchard.Collections;
using Orchard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Orchard.Data.EntityFrameworkCore.Repositories
{
    /// <summary>
    /// Base class for all repositories those uses .EntityFramework.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public   class EfCoreRepositoryBase<TEntity, TPrimaryKey> :
        Domain.Repositories.RepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Gets EF DbContext object.
        /// </summary>
        public   virtual EfCoreDbContext Context { get; private set; }

        /// <summary>
        /// Gets DbSet for given entity.
        /// </summary>
        public   virtual DbSet<TEntity> Table { get { return Context.Set<TEntity>(); } }

        //private readonly IDbContextProvider<TDbContext> _dbContextProvider;

        /// <summary>
        /// Creates a new <see cref="EfCoreRepositoryBase{TDbContext,TEntity,TPrimaryKey}"/> object.
        /// </summary>
        /// <param name="EfCoreDbContext">dbContext</param>
        public   EfCoreRepositoryBase(EfCoreDbContext dbContext)
        {
            Context = dbContext;
        }
        #region Select/Get/Query

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        public override  TEntity Single(TPrimaryKey id)
        {
            return Single(CreateEqualityExpressionForId(id));
        }

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        public override  Task<TEntity> SingleAsync(TPrimaryKey id)
        {
            return GetAll().FirstOrDefaultAsync(CreateEqualityExpressionForId(id));
        }

        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// </summary>
        /// <param name="predicate">Entity</param>
        public override  TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// </summary>
        /// <param name="predicate">Entity</param>
        public override  Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Creates an entity with given primary key without database access.
        /// </summary>
        /// <param name="id">Primary key of the entity to load</param>
        /// <returns>Entity</returns>
        public override  TEntity Load(TPrimaryKey id)
        {
            return Single(id);
        }
        /// <summary>
        /// Used to get a IQueryable that is used to retrieve entities from entire table.
        /// </summary>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public override  IQueryable<TEntity> GetAll()
        {
            return GetAllIncluding();
        }

        /// <summary>
        /// Used to get a IQueryable that is used to retrieve entities from entire table.
        /// One or more 
        /// </summary>
        /// <param name="propertySelectors">A list of include expressions.</param>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public override  IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var query = Table.AsQueryable();
            if (!propertySelectors.IsNullOrEmpty())
            {
                foreach (var propertySelector in propertySelectors)
                {
                    query = query.Include(propertySelector);
                }
            }
            query = ApplyFilters(query);
            return query;
        }

        /// <summary>
        /// Used to get all entities.
        /// </summary>
        /// <returns>List of all entities</returns>
        public override  List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        /// <summary>
        /// Used to get all entities.
        /// </summary>
        /// <returns>List of all entities</returns>
        public override  Task<List<TEntity>> GetAllListAsync()
        {
            return GetAll().ToListAsync();
        }

        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <param name="orderByExpressions"></param>
        /// <returns>List of all entities</returns>
        public override  List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate,params IOrderByExpression<TEntity>[] orderByExpressions)
        {
            return ApplyOrderBy(GetAll().Where(predicate),orderByExpressions).ToList();
        }

        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
        public override  async  Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate,params IOrderByExpression<TEntity>[] orderByExpressions)
        {
            return await ApplyOrderBy(GetAll().Where(predicate),orderByExpressions).ToListAsync();
        }

        /// <summary>
        /// Used to run a query over entire entities.
        /// <see cref="UnitOfWorkAttribute"/> attribute is not always necessary (as opposite to <see cref="GetAll"/>)
        /// if <paramref name="queryMethod"/> finishes IQueryable with ToList, FirstOrDefault etc..
        /// </summary>
        /// <typeparam name="T">Type of return value of this method</typeparam>
        /// <param name="queryMethod">This method is used to query over entities</param>
        /// <returns>Query result</returns>
        public override  T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
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
        public override  async Task<List<TEntity>> GetPaggingListAsync(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize, IOrderByExpression<TEntity>[] orderByExpressions)
        {
            int totalCount = 0;
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 20;
            int totalPages = (totalCount / pageSize);
            if (totalCount % pageSize != 0) totalPages += 1;
            int startIndex = (pageIndex - 1) * pageSize;
            return await ApplyOrderBy(GetAll().Where(predicate), orderByExpressions).Skip(startIndex).Take(pageSize).ToListAsync();
        }
        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        public override  TEntity Insert(TEntity entity)
        {
            entity = Table.Add(entity).Entity;
            //if (entity.IsTransient())
            //{
            //    Context.SaveChanges();
            //}
            Context.SaveChanges();
            return entity;
        }

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        public override  async Task<TEntity> InsertAsync(TEntity entity)
        {
            entity = Table.AddAsync(entity).Result.Entity;
            //if (entity.IsTransient())
            //{
            //    await Context.SaveChangesAsync();
            //}
            await Context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        public override  TPrimaryKey InsertAndGetId(TEntity entity)
        {
            entity = Insert(entity);
            //if (entity.IsTransient())
            //{
            //    Context.SaveChanges();
            //}
            Context.SaveChanges();
            return entity.Id;
        }

        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        public override  async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            entity = await InsertAsync(entity);
            return entity.Id;
        }

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="entity">Entity</param>
        public override  TEntity InsertOrUpdate(TEntity entity)
        {
            return entity.IsTransient()
                 ? Insert(entity)
                 : Update(entity);
        }

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="entity">Entity</param>
        public override  Task<TEntity> InsertOrUpdateAsync(TEntity entity)
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
        public override  TPrimaryKey InsertOrUpdateAndGetId(TEntity entity)
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
        public override  async Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
        {
            entity = await InsertOrUpdateAsync(entity);
            return entity.Id;
        }
        /// <summary>
        /// Inserts  new entities.
        /// </summary>
        /// <param name="entities">Inserted entities</param>
        public override  int InsertList(IEnumerable<TEntity> entities)
        {
            Table.AddRange(entities);
            return Context.SaveChanges();
        }

        /// <summary>
        /// Inserts  new entities.
        /// </summary>
        /// <param name="entities">Inserted entities</param>
        public override  async Task<int> InsertListAsync(IEnumerable<TEntity> entities)
        {
            Table.AddRange(entities);
            return await Context.SaveChangesAsync();
        }
        #endregion

        #region Update

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        public override  TEntity Update(TEntity entity)
        {
            AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
            return entity;
        }

        /// <summary>
        /// Updates an existing entity. 
        /// </summary>
        /// <param name="entity">Entity</param>
        public override  async Task<TEntity> UpdateAsync(TEntity entity)
        {
            AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="updateAction">Func that can be used to change values of the entity</param>
        /// <returns>Updated entity</returns>
        public override  TEntity Update(TPrimaryKey id, Func<TEntity, TEntity> updateAction)
        {
            var entity = Single(id);
            updateAction(entity);
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
            return entity;
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="updateAction">Action that can be used to change values of the entity</param>
        /// <returns>Updated entity</returns>
        public override  async Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task<TEntity>> updateAction)
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
            return Context.Set<TEntity>()
            .Where(predicate)
            .Update(updateExpression);
        }
        /// <summary>
        /// Updates entities
        /// </summary>
        /// <param name="updateExpression">The update expression</param>
        /// <param name="predicate"><![CDATA[ Expression<Func<TEntity, bool>>  ]]>predicate</param>
        public override  async Task<int> UpdateAsync(Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>()
           .Where(predicate)
           .UpdateAsync(updateExpression);
        }
        #endregion

        #region Delete

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public override  int Delete(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
           return Context.SaveChanges();
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public override  async Task<int> DeleteAsync(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
            return await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        public override  int Delete(TPrimaryKey id)
        {
            var entity = Table.Local.FirstOrDefault(ent => EqualityComparer<TPrimaryKey>.Default.Equals(ent.Id, id));
            if (entity == null)
            {
                entity = Single(id);
                if (entity == null)
                {
                    return 0;
                }
            }
            return Delete(entity);
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        public override  async Task<int> DeleteAsync(TPrimaryKey id)
        {
            var entity = Table.Local.FirstOrDefault(ent => EqualityComparer<TPrimaryKey>.Default.Equals(ent.Id, id));
            if (entity == null)
            {
                entity = Single(id);
                if (entity == null)
                {
                    return 0;
                }
            }
             return await DeleteAsync(entity);
        }

        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        public override  int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            //Table.RemoveRange(GetAll().Where(predicate));
            //return Context.SaveChanges();
            return  Context.Set<TEntity>().Where(predicate).Delete();
        }

        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        public override  async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            //Table.RemoveRange(GetAll().Where(predicate));
            //return await Context.SaveChangesAsync();
            return await Context.Set<TEntity>().Where(predicate).DeleteAsync();
        }

        #endregion

        #region Aggregates

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override  int Count()
        {
            return GetAll().Count();
        }

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override  async  Task<int> CountAsync()
        {
           return await Table.CountAsync();
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public override  int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).Count();
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public override  async  Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Table.Where(predicate).CountAsync();
        }

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override  long LongCount()
        {
            return GetAll().LongCount();
        }

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        public override  async  Task<long> LongCountAsync()
        {
            return await GetAll().LongCountAsync();
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public  override  long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return  GetAll().Where(predicate).LongCount();
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public override  async  Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).LongCountAsync();
        }
        #endregion  Aggregates

        protected virtual void AttachIfNot(TEntity entity)
        {
            if (!Table.Local.Contains(entity))
            {
                Table.Attach(entity);
            }
        }
        #region Method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> query)
        {
            //query = ApplySoftDeleteFilter(query);
            return query;
        }
        /// <summary>
        /// 过滤有删除标识的记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private IQueryable<TEntity> ApplySoftDeleteFilter(IQueryable<TEntity> query)
        {
            if (typeof(ISoftDelete).GetTypeInfo().IsAssignableFrom(typeof(TEntity)))
            {
                query = query.Where(e => !((ISoftDelete)e).DeleteFlag);
            }
            return query;
        }
        #endregion  Method
    }
}
