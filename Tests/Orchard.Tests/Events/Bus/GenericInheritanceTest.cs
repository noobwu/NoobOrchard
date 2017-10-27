using Orchard.Domain.Entities;
using Orchard.Events.Bus.Entities;
using Shouldly;
using NUnit.Framework;

namespace Orchard.Tests.Events.Bus
{
    public class GenericInheritanceTest : EventBusTestBase
    {
       [Test]
        public void Should_Trigger_For_Inherited_Generic_1()
        {
            var triggeredEvent = false;

            EventBus.Register<EntityChangedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Id.ShouldBe(42);
                    triggeredEvent = true;
                });

            EventBus.Trigger(new EntityUpdatedEventData<Person>(new Person { Id = 42 }));

            triggeredEvent.ShouldBe(true);
        }

       [Test]
        public void Should_Trigger_For_Inherited_Generic_2()
        {
            var triggeredEvent = false;

            EventBus.Register<EntityChangedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Id.ShouldBe(42);
                    triggeredEvent = true;
                });

            EventBus.Trigger(new EntityChangedEventData<Student>(new Student { Id = 42 }));

            triggeredEvent.ShouldBe(true);
        }
        
        
        public class Person : Entity
        {
            
        }

        public class Student : Person
        {
            
        }
    }
}