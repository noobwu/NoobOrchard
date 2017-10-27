using NUnit.Framework;
using Orchard.Domain.Entities;
using Orchard.Events.Bus;
using Orchard.Events.Bus.Entities;
using Shouldly;

namespace Orchard.Tests.Events.Bus
{
    public class EventBus_EntityEvents_Test : EventBusTestBase
    {
       [Test]
        public void Should_Call_Created_And_Changed_Once()
        {
            var handler = new MyEventHandler();

            EventBus.Register<EntityChangedEventData<MyEntity>>(handler);
            EventBus.Register<EntityCreatedEventData<MyEntity>>(handler);

            EventBus.Trigger(new EntityCreatedEventData<MyEntity>(new MyEntity()));

            handler.EntityCreatedEventCount.ShouldBe(1);
            handler.EntityChangedEventCount.ShouldBe(1);
        }

        public class MyEntity : Entity
        {
            
        }

        public class MyEventHandler : 
            IEventHandler<EntityChangedEventData<MyEntity>>,
            IEventHandler<EntityCreatedEventData<MyEntity>>
        {
            public int EntityChangedEventCount { get; set; }
            public int EntityCreatedEventCount { get; set; }

            public void HandleEvent(EntityChangedEventData<MyEntity> eventData)
            {
                EntityChangedEventCount++;
            }

            public void HandleEvent(EntityCreatedEventData<MyEntity> eventData)
            {
                EntityCreatedEventCount++;
            }
        }
    }
}
