using FluentNHibernate.Mapping;
using Orchard.Domain.Entities;
using System;

namespace Orchard.Data.NHibernate.EntityMappings
{
    /// <summary>
    /// This class is base class to map entities to database tables.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Type of primary key of the entity</typeparam>
    public abstract class EntityMap<TEntity, TPrimaryKey> : ClassMap<TEntity> where TEntity : IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tableName">Table name</param>
        protected EntityMap(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName)) //TODO: Use code contracts?
            {
                throw new ArgumentNullException("tableName");
            }

            Table(tableName);
            Id(x => x.Id);
            //过滤有删除标识的记录
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                Where("IsDeleted = 0"); //TODO: Test with other DBMS then SQL Server
            }

        }
    }
}
