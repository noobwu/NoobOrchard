using EntityFramework.DynamicFilters;
using Orchard.Collections;
using Orchard.Domain.Entities;
using Orchard.Domain.Uow;
using Orchard.Events;
using Orchard.Events.Bus;
using Orchard.Events.Bus.Entities;
using Orchard.Logging;
using Orchard.Reflection;
using Orchard.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Orchard.Data.EntityFramework
{
    /// <summary>
    /// Base class for all DbContext classes in the application.
    /// </summary>
    public abstract class EfDbContext : DbContext, ITransientDependency
    {

        /// <summary>
        /// Reference to the logger.
        /// </summary>
        public ILogger Logger { get; set; }
        /// <summary>
        /// Used to trigger entity change events.
        /// </summary>
        public IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

        /// <summary>
        /// Reference to the event bus.
        /// </summary>
        public IEventBus EventBus { get; set; }

        /// <summary>
        /// Reference to GUID generator.
        /// </summary>
        public IGuidGenerator GuidGenerator { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EfDbContext()
        {
            InitializeDbContext();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public EfDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            InitializeDbContext();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EfDbContext(DbCompiledModel model)
            : base(model)
        {
            InitializeDbContext();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EfDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            InitializeDbContext();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EfDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            InitializeDbContext();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EfDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
            InitializeDbContext();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EfDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            InitializeDbContext();
        }

        private void InitializeDbContext()
        {
            SetNullsForInjectedProperties();
            // RegisterToChanges();
        }

        private void RegisterToChanges()
        {
            ((IObjectContextAdapter)this)
                .ObjectContext
                .ObjectStateManager
                .ObjectStateManagerChanged += ObjectStateManager_ObjectStateManagerChanged;
        }

        protected virtual void ObjectStateManager_ObjectStateManagerChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e)
        {
            var contextAdapter = (IObjectContextAdapter)this;
            if (e.Action != CollectionChangeAction.Add)
            {
                return;
            }
            var entry = contextAdapter.ObjectContext.ObjectStateManager.GetObjectStateEntry(e.Element);
            switch (entry.State)
            {
                case EntityState.Added:
                    CheckAndSetId(entry.Entity);
                    SetCreationAuditProperties(entry.Entity, GetAuditUserId());
                    break;
                default:
                    break;

            }
        }

        private void SetNullsForInjectedProperties()
        {
            Logger = LoggerManager.GetLogger(this.GetType());
            EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
            GuidGenerator = SequentialGuidGenerator.Instance;
            EventBus = NullEventBus.Instance;
        }

        public virtual void Initialize()
        {
            Database.Initialize(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //过滤条件
            modelBuilder.Filter(DefaultDataFilters.SoftDelete, (ISoftDelete d) => d.DeleteFlag, false);
        }

        public override int SaveChanges()
        {
            try
            {
                //var changedEntities = ApplyConcepts();
                var result = base.SaveChanges();
                //EntityChangeEventHelper.TriggerEvents(changedEntities);
                return result;
            }
            catch (DbEntityValidationException ex)
            {
                LogDbEntityValidationException(ex);
                throw;
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                //var changeReport = ApplyAbpConcepts();
                var result = await base.SaveChangesAsync(cancellationToken);
                // await EntityChangeEventHelper.TriggerEventsAsync(changeReport);
                return result;
            }
            catch (DbEntityValidationException ex)
            {
                LogDbEntityValidationException(ex);
                throw ex;
            }
        }


        protected virtual void LogDbEntityValidationException(DbEntityValidationException exception)
        {
            if (Logger != null)
            {
                Logger.Error("There are some validation errors while saving changes in EntityFramework:");
                foreach (var ve in exception.EntityValidationErrors.SelectMany(eve => eve.ValidationErrors))
                {

                    Logger.Error(" - " + ve.PropertyName + ": " + ve.ErrorMessage);
                }
            }

        }
        protected virtual void CheckAndSetId(object entityAsObj)
        {
            //Set GUID Ids
            if (entityAsObj is IEntity<Guid> entity && entity.Id == Guid.Empty)
            {
                var entityType = ObjectContext.GetObjectType(entityAsObj.GetType());
                var idProperty = entityType.GetProperty("Id");
                var dbGeneratedAttr =
                    ReflectionHelper.GetSingleAttributeOrDefault<DatabaseGeneratedAttribute>(idProperty);
                if (dbGeneratedAttr == null || dbGeneratedAttr.DatabaseGeneratedOption == DatabaseGeneratedOption.None)
                {
                    entity.Id = GuidGenerator.Create();
                }
            }
        }




        #region Apb Concepts
        protected virtual EntityChangeReport ApplyConcepts()
        {
            var changeReport = new EntityChangeReport();

            var userId = GetAuditUserId();

            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                ApplyAbpConcepts(entry, userId, changeReport);
            }

            return changeReport;
        }
        protected virtual void ApplyAbpConcepts(DbEntityEntry entry, long? userId, EntityChangeReport changeReport)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ApplyConceptsForAddedEntity(entry, userId, changeReport);
                    break;
                case EntityState.Modified:
                    ApplyConceptsForModifiedEntity(entry, userId, changeReport);
                    break;
                case EntityState.Deleted:
                    ApplyConceptsForDeletedEntity(entry, userId, changeReport);
                    break;
            }

            AddDomainEvents(changeReport.DomainEvents, entry.Entity);
        }

        protected virtual void ApplyConceptsForAddedEntity(DbEntityEntry entry, long? userId, EntityChangeReport changeReport)
        {
            CheckAndSetId(entry.Entity);
            SetCreationAuditProperties(entry.Entity, userId);
            changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Created));
        }

        protected virtual void ApplyConceptsForModifiedEntity(DbEntityEntry entry, long? userId, EntityChangeReport changeReport)
        {
            SetModificationAuditProperties(entry.Entity, userId);

            if (entry.Entity is ISoftDelete && entry.Entity.As<ISoftDelete>().DeleteFlag)
            {
                SetDeletionAuditProperties(entry.Entity, userId);
                changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Deleted));
            }
            else
            {
                changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Updated));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="userId"></param>
        /// <param name="changeReport"></param>
        protected virtual void ApplyConceptsForDeletedEntity(DbEntityEntry entry, long? userId, EntityChangeReport changeReport)
        {
            CancelDeletionForSoftDelete(entry);
            SetDeletionAuditProperties(entry.Entity, userId);
            changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Deleted));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainEvents"></param>
        /// <param name="entityAsObj"></param>
        protected virtual void AddDomainEvents(List<DomainEventEntry> domainEvents, object entityAsObj)
        {
            var generatesDomainEventsEntity = entityAsObj as IGeneratesDomainEvents;
            if (generatesDomainEventsEntity == null)
            {
                return;
            }

            if (generatesDomainEventsEntity.DomainEvents.IsNullOrEmpty())
            {
                return;
            }

            domainEvents.AddRange(
                generatesDomainEventsEntity.DomainEvents.Select(
                    eventData => new DomainEventEntry(entityAsObj, eventData)));
            generatesDomainEventsEntity.DomainEvents.Clear();
        }
        #endregion Apb Concepts

        #region Audit
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityAsObj"></param>
        /// <param name="userId"></param>
        protected virtual void SetCreationAuditProperties(object entityAsObj, long? userId)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityAsObj"></param>
        /// <param name="userId"></param>
        protected virtual void SetModificationAuditProperties(object entityAsObj, long? userId)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        protected virtual void CancelDeletionForSoftDelete(DbEntityEntry entry)
        {
            if (!(entry.Entity is ISoftDelete))
            {
                return;
            }
            var softDeleteEntry = entry.Cast<ISoftDelete>();
            softDeleteEntry.Reload();
            softDeleteEntry.State = EntityState.Modified;
            softDeleteEntry.Entity.DeleteFlag = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityAsObj"></param>
        /// <param name="userId"></param>
        protected virtual void SetDeletionAuditProperties(object entityAsObj, long? userId)
        {

        }


        protected virtual long? GetAuditUserId()
        {
            return null;
        }

        #endregion Audit
    }
}
