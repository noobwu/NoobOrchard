using System.Collections.Generic;
using System.Linq;
using Autofac.Builder;
using Autofac.Core;
using NUnit.Framework;
using Autofac;

namespace Orchard.Tests.Environment.AutofacUtil.Configuration.Core
{
    [TestFixture]
    public class ComponentRegistrarFixture: ConfigFixtureBase
    {
        [Test]
        public void RegisterConfiguredComponents_AllowsMultipleRegistrationsOfSameType()
        {
            var builder = ConfigureContainerWithJson("ComponentRegistrar_SameTypeRegisteredMultipleTimes.json");
            var container = builder.Build();
            var collection = container.Resolve<IEnumerable<SimpleComponent>>();
            Assert.AreEqual(2, collection.Count());

            // Test using Any() because we aren't necessarily guaranteed the order of resolution.
            Assert.True(collection.Any(a => a.Input == 5), "The first registration (5) wasn't found.");
            Assert.True(collection.Any(a => a.Input == 10), "The second registration (10) wasn't found.");
        }

        [Test]
        public void RegisterConfiguredComponents_AutoActivationEnabledOnComponent()
        {
            var builder = ConfigureContainerWithJson("ComponentRegistrar_EnableAutoActivation.json");
            var container = builder.Build();
            IComponentRegistration registration;
            Assert.True(container.ComponentRegistry.TryGetRegistration(new KeyedService("a", typeof(object)), out registration), "The expected component was not registered.");
            Assert.True(registration.Services.Any(a => a.GetType().Name == "AutoActivateService"), "Auto activate service was not registered on the component");
        }

        [Test]
        public void RegisterConfiguredComponents_AutoActivationNotEnabledOnComponent()
        {
            var builder = ConfigureContainerWithJson("ComponentRegistrar_EnableAutoActivation.json");
            var container = builder.Build();
            IComponentRegistration registration;
            Assert.True(container.ComponentRegistry.TryGetRegistration(new KeyedService("b", typeof(object)), out registration), "The expected component was not registered.");
            Assert.False(registration.Services.Any(a => a.GetType().Name == "AutoActivateService"), "Auto activate service was registered on the component when it shouldn't be.");
        }

        [Test]
        public void RegisterConfiguredComponents_ConstructorInjection()
        {
            var builder = ConfigureContainerWithXml("ComponentRegistrar_SingletonWithTwoServices.xml");
            var container = builder.Build();
            var cpt = (SimpleComponent)container.Resolve<ITestComponent>();
             Assert.AreEqual(1, cpt.Input);
        }

        [Test]
        public void RegisterConfiguredComponents_ExternalOwnership()
        {
            var builder = ConfigureContainerWithJson("ComponentRegistrar_ExternalOwnership.json");
            var container = builder.Build();
            IComponentRegistration registration;
            Assert.True(container.ComponentRegistry.TryGetRegistration(new TypedService(typeof(SimpleComponent)), out registration), "The expected component was not registered.");
             Assert.AreEqual(InstanceOwnership.ExternallyOwned, registration.Ownership);
        }

        [Test]
        public void RegisterConfiguredComponents_LifetimeScope_InstancePerDependency()
        {
            var builder = ConfigureContainerWithJson("ComponentRegistrar_InstancePerDependency.json");
            var container = builder.Build();
            Assert.AreNotSame(container.Resolve<SimpleComponent>(), container.Resolve<SimpleComponent>());
        }

        [Test]
        public void RegisterConfiguredComponents_LifetimeScope_Singleton()
        {
            var builder = ConfigureContainerWithXml("ComponentRegistrar_SingletonWithTwoServices.xml");
            var container = builder.Build();
            Assert.AreSame(container.Resolve<ITestComponent>(), container.Resolve<ITestComponent>());
        }

        [Test]
        public void RegisterConfiguredComponents_PropertyInjectionEnabledOnComponent()
        {
            var builder = ConfigureContainerWithJson("ComponentRegistrar_EnablePropertyInjection.json");
            builder.RegisterType<SimpleComponent>().As<ITestComponent>();
            builder.RegisterInstance("hello").As<string>();
            var container = builder.Build();
            var e = container.Resolve<ComponentConsumer>();
            Assert.NotNull(e.Component);

            // Issue #2 - Ensure properties in base classes can be set by config.
             Assert.AreEqual("hello", e.Message);
        }

        [Test]
        public void RegisterConfiguredComponents_PropertyInjectionWithProvidedValues()
        {
            var builder = ConfigureContainerWithXml("ComponentRegistrar_SingletonWithTwoServices.xml");
            var container = builder.Build();
            var cpt = (SimpleComponent)container.Resolve<ITestComponent>();
            Assert.True(cpt.ABool, "The Boolean property value was not properly parsed/converted.");

            // Issue #2 - Ensure properties in base classes can be set by config.
             Assert.AreEqual("hello", cpt.Message);
        }

        [Test]
        public void RegisterConfiguredComponents_RegistersMetadata()
        {
            var builder = ConfigureContainerWithJson("ComponentRegistrar_ComponentWithMetadata.json");
            var container = builder.Build();
            IComponentRegistration registration;
            Assert.True(container.ComponentRegistry.TryGetRegistration(new KeyedService("a", typeof(object)), out registration), "The expected service wasn't registered.");
             Assert.AreEqual(42, (int)registration.Metadata["answer"]);
        }

        [Test]
        public void RegisterConfiguredComponents_SingleComponentWithTwoServices()
        {
            var builder = ConfigureContainerWithXml("ComponentRegistrar_SingletonWithTwoServices.xml");
            var container = builder.Build();
            container.AssertRegistered<ITestComponent>("The ITestComponent wasn't registered.");
            container.AssertRegistered<object>("The object wasn't registered.");
            container.AssertNotRegistered<SimpleComponent>("The base SimpleComponent type was incorrectly registered.");
            Assert.AreSame(container.Resolve<ITestComponent>(), container.Resolve<object>());
        }

        private class ComponentConsumer : BaseComponentConsumer
        {
            public ITestComponent Component { get; set; }
        }

        private class BaseComponentConsumer
        {
            // Issue #2 - Ensure properties in base classes can be set by config.
            public string Message { get; set; }
        }

        private interface ITestComponent
        {
        }

        private class SimpleComponent : BaseComponent, ITestComponent
        {
            public SimpleComponent()
            {
            }

            public SimpleComponent(int input)
            {
                Input = input;
            }

            public bool ABool { get; set; }

            public int Input { get; set; }
        }

        private class BaseComponent
        {
            // Issue #2 - Ensure properties in base classes can be set by config.
            public string Message { get; set; }
        }
    }
}
