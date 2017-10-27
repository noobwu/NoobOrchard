using Orchard.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orchard.Events.Bus.Entities
{
    /// <summary>
    /// Used to trigger entity change events.
    /// </summary>
    public class EntityChangeEventHelper : ITransientDependency, IEntityChangeEventHelper
    {
        public  IEventBus EventBus { get; set; }
        public EntityChangeEventHelper()
        {

            EventBus = NullEventBus.Instance;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="changeReport"></param>
        public virtual void TriggerEvents(EntityChangeReport changeReport)
        {
            TriggerEventsInternal(changeReport);

            if (changeReport.IsEmpty())
            {
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="changeReport"></param>
        /// <returns></returns>
        public Task TriggerEventsAsync(EntityChangeReport changeReport)
        {
            TriggerEventsInternal(changeReport);

            if (changeReport.IsEmpty())
            {
                return Task.FromResult(0);
            }

            return Task.CompletedTask;
        }

        public virtual void TriggerEntityCreatingEvent(object entity)
        {
            TriggerEventWithEntity(typeof(EntityCreatingEventData<>), entity);
        }

        public virtual void TriggerEntityCreatedEventOnUowCompleted(object entity)
        {
            TriggerEventWithEntity(typeof(EntityCreatedEventData<>), entity);
        }

        public virtual void TriggerEntityUpdatingEvent(object entity)
        {
            TriggerEventWithEntity(typeof(EntityUpdatingEventData<>), entity);
        }

        public virtual void TriggerEntityUpdatedEventOnUowCompleted(object entity)
        {
            TriggerEventWithEntity(typeof(EntityUpdatedEventData<>), entity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public virtual void TriggerEntityDeletingEvent(object entity)
        {
            TriggerEventWithEntity(typeof(EntityDeletingEventData<>), entity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public virtual void TriggerEntityDeletedEventOnUowCompleted(object entity)
        {
            TriggerEventWithEntity(typeof(EntityDeletedEventData<>), entity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="changeReport"></param>
        public virtual void TriggerEventsInternal(EntityChangeReport changeReport)
        {
            TriggerEntityChangeEvents(changeReport.ChangedEntities);
            TriggerDomainEvents(changeReport.DomainEvents);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="changedEntities"></param>
        protected virtual void TriggerEntityChangeEvents(List<EntityChangeEntry> changedEntities)
        {
            foreach (var changedEntity in changedEntities)
            {
                switch (changedEntity.ChangeType)
                {
                    case EntityChangeType.Created:
                        TriggerEntityCreatingEvent(changedEntity.Entity);
                        TriggerEntityCreatedEventOnUowCompleted(changedEntity.Entity);
                        break;
                    case EntityChangeType.Updated:
                        TriggerEntityUpdatingEvent(changedEntity.Entity);
                        TriggerEntityUpdatedEventOnUowCompleted(changedEntity.Entity);
                        break;
                    case EntityChangeType.Deleted:
                        TriggerEntityDeletingEvent(changedEntity.Entity);
                        TriggerEntityDeletedEventOnUowCompleted(changedEntity.Entity);
                        break;
                    default:
                        throw new DefaultException("Unknown EntityChangeType: " + changedEntity.ChangeType);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainEvents"></param>
        protected virtual void TriggerDomainEvents(List<DomainEventEntry> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                EventBus.Trigger(domainEvent.EventData.GetType(), domainEvent.SourceEntity, domainEvent.EventData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="genericEventType"></param>
        /// <param name="entity"></param>
        protected virtual void TriggerEventWithEntity(Type genericEventType, object entity)
        {
            var entityType = entity.GetType();
            var eventType = genericEventType.MakeGenericType(entityType);
            EventBus.Trigger(eventType, (IEventData)Activator.CreateInstance(eventType, new[] { entity }));
        }
    }
}