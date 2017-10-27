using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Orchard.Domain.Entities;
using Orchard.Domain.Uow;
using Orchard.Events;
using Orchard.Logging;
using Orchard.Utility;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Orchard.Data.EntityFrameworkCore
{
    /// <summary>
    /// Base class for all DbContext classes in the application.
    /// </summary>
    public abstract class EfCoreDbContext : DbContext, ITransientDependency
    {
      

        /// <summary>
        /// Used to trigger entity change events.
        /// </summary>
        //public IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

        /// <summary>
        /// Reference to the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Reference to the event bus.
        /// </summary>
        public IEventBus EventBus { get; set; }

        /// <summary>
        /// Reference to GUID generator.
        /// </summary>
        public IGuidGenerator GuidGenerator { get; set; }

        /// <summary>
        /// Reference to the current UOW provider.
        /// </summary>
        //public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }


        /// <summary>
        /// Can be used to suppress automatically setting TenantId on SaveChanges.
        /// Default: false.
        /// </summary>
        public bool SuppressAutoSetTenantId { get; set; }

        /// <summary>
        /// 是否过滤软删除
        /// </summary>
        protected virtual bool IsSoftDeleteFilterEnabled => true;
        /// <summary>
        /// 
        /// </summary>
        private static MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(EfCoreDbContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic);
        /// <summary>
        /// Constructor.
        /// </summary>
        protected EfCoreDbContext(DbContextOptions options)
            : base(options)
        {
            InitializeDbContext();
        }

        private void InitializeDbContext()
        {
            SetNullsForInjectedProperties();
        }

        private void SetNullsForInjectedProperties()
        {
            Logger = LoggerManager.GetLogger(this.GetType());
            //EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
            GuidGenerator = SequentialGuidGenerator.Instance;
            //EventBus = NullEventBus.Instance;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //过滤条件
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureGlobalFiltersMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] { modelBuilder, entityType });
            }
        }
        public override int SaveChanges()
        {
            try
            {
                //var changeReport = ApplyAbpConcepts();
                var result = base.SaveChanges();
                //EntityChangeEventHelper.TriggerEvents(changeReport);
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DbConcurrencyException(ex.Message, ex);
            }
        }

        public override  async  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                //var changeReport = ApplyAbpConcepts();
                var result = await base.SaveChangesAsync(cancellationToken);
                //await EntityChangeEventHelper.TriggerEventsAsync(changeReport);
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DbConcurrencyException(ex.Message, ex);
            }
        }
        protected virtual void CancelDeletionForSoftDelete(EntityEntry entry)
        {
            if (!(entry.Entity is ISoftDelete))
            {
                return;
            }
            entry.Reload();
            entry.State = EntityState.Modified;
            entry.Entity.As<ISoftDelete>().DeleteFlag = true;
        }
        protected virtual long? GetAuditUserId()
        {

            return null;
        }


        protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
    where TEntity : class
        {
            if (entityType.BaseType == null && ShouldFilterEntity<TEntity>(entityType))
            {
                var filterExpression = CreateFilterExpression<TEntity>();
                if (filterExpression != null)
                {
                    modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entityType"></param>
        /// <returns></returns>
        protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }
            return false;
        }

        protected virtual Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
            where TEntity : class
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                /* This condition should normally be defined as below:
                 * !IsSoftDeleteFilterEnabled || !((ISoftDelete) e).IsDeleted
                 * But this causes a problem with EF Core (see https://github.com/aspnet/EntityFrameworkCore/issues/9502)
                 * So, we made a workaround to make it working. It works same as above.
                 */

                Expression<Func<TEntity, bool>> softDeleteFilter = e => !((ISoftDelete)e).DeleteFlag;
                expression = expression == null ? softDeleteFilter : expression.And(softDeleteFilter);
            }

            return expression;
        }
    }
}
