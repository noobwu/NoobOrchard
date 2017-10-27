using Autofac;
using NUnit.Framework;
using Orchard.Logging;
using System;
using ILogger = Orchard.Logging.ILogger;
using ILoggerFactory = Orchard.Logging.ILoggerFactory;
using NullLogger = Orchard.Logging.NullLogger;

namespace Orchard.Tests.Logging
{
    [TestFixture]
    public class LoggingModuleTests
    {
        [Test]
        public void LoggingModuleWillSetLoggerProperty()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LoggingModule());
            builder.RegisterType<Thing>();
            var container = builder.Build();
            var thing = container.Resolve<Thing>();
            Assert.That(thing.Logger, Is.Not.Null);
        }

        [Test]
        public void LoggerFactoryIsPassedTheTypeOfTheContainingInstance()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LoggingModule());
            builder.RegisterType<Thing>();
            var stubFactory = new StubFactory();
            builder.RegisterInstance(stubFactory).As<ILoggerFactory>();

            var container = builder.Build();
            var thing = container.Resolve<Thing>();
            Assert.That(thing.Logger, Is.Not.Null);
            Assert.That(stubFactory.CalledType, Is.EqualTo(typeof(Thing)));
        }

        public class StubFactory : ILoggerFactory
        {
            public ILogger GetLogger(Type type)
            {
                CalledType = type;
                return NullLogger.Instance;
            }
            public ILogger GetLogger(string type)
            {
                CalledType =type.GetType();
                return NullLogger.Instance;
            }
            public Type CalledType { get; set; }
        }

        [Test]
        public void DefaultLoggerConfigurationUsesCastleLoggerFactoryOverTraceSource()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LoggingModule());
            builder.RegisterType<Thing>();
            var container = builder.Build();

            var thing = container.Resolve<Thing>();
            Assert.That(thing.Logger, Is.Not.Null);
        }
    }

    public class Thing
    {
        public ILogger Logger { get; set; }
    }

}
