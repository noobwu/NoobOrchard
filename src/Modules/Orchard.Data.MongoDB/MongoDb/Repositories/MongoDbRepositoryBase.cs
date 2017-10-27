using System;
using System.Collections.Generic;
using System.Linq;
using Driver = MongoDB.Driver;
using MongoDB.Driver.Linq;
using Orchard.Domain.Entities;
using Orchard.Domain.Repositories;
using Orchard.Exceptions;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Orchard.Data.MongoDb.Repositories
{
    /// <summary>
    /// Implements IRepository for MongoDB.
    /// </summary>
    /// <typeparam name="TEntity">Type of the Entity for this repository</typeparam>
    public class MongoDbRepositoryBase<TEntity> : MongoDbRepositoryBase<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        public MongoDbRepositoryBase(Driver.MongoDatabase database)
            : base(database)
        {
        }
    }

    /// <summary>
    /// Implements IRepository for MongoDB.
    /// </summary>
    /// <typeparam name="TEntity">Type of the Entity for this repository</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key of the entity</typeparam>
    public class MongoDbRepositoryBase<TEntity, TPrimaryKey> : RepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public Driver.MongoDatabase Database;
       

        public virtual Driver.MongoCollection<TEntity> Collection
        {
            get
            {
                return Database.GetCollection<TEntity>(typeof(TEntity).Name);
            }
        }
        public MongoDbRepositoryBase(Driver.MongoDatabase database)
        {
            Database = database;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IQueryable<TEntity> GetAll()
        {
            return Collection.AsQueryable();
        }
        public override TEntity Single(TPrimaryKey id)
        {
            var query = Driver.Builders.Query<TEntity>.EQ(e => e.Id, id);
            var entity = Collection.FindOne(query);
            if (entity == null)
            {
                throw new DefaultException("There is no such an entity with given primary key. Entity type: " + typeof(TEntity).FullName + ", primary key: " + id);
            }
            return entity;
        }
        public override TEntity Insert(TEntity entity)
        {
           var result= Collection.Insert(entity);
            return entity;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override TEntity Update(TEntity entity)
        {
            Collection.Save(entity);
            return entity;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public override int Delete(TEntity entity)
        {
           return Delete(entity.Id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public override int Delete(TPrimaryKey id)
        {
            var query = Driver.Builders.Query<TEntity>.EQ(e => e.Id, id);
            return (int)Collection.Remove(query).DocumentsAffected;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public override int InsertList(IEnumerable<TEntity> entities)
        {
           return Collection.InsertBatch(entities).Count();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateAction"></param>
        /// <returns></returns>
        public override TEntity Update(TPrimaryKey id, Func<TEntity, TEntity> updateAction)
        {
            var entity = Single(id);
            updateAction(entity);
            return entity;
        }

        public override int Update(Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public override Task<int> UpdateAsync(Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}