using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Autofac;

namespace Orchard.Tests.Environment.AutofacUtil.Configuration.Core
{
    [TestFixture]
    public class ConfigurationExtensions_DictionaryParametersFixture: ConfigFixtureBase
    {
        public class A
        {
            public IDictionary<string, string> Dictionary { get; set; }
        }

        [Test]
        public void InjectsDictionaryProperty()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_DictionaryParameters.xml").Build();

            var poco = container.Resolve<A>();

            Assert.True(poco.Dictionary.Count == 2);
            Assert.True(poco.Dictionary.ContainsKey("Key1"));
            Assert.True(poco.Dictionary.ContainsKey("Key2"));
             Assert.AreEqual("Val1", poco.Dictionary["Key1"]);
             Assert.AreEqual("Val2", poco.Dictionary["Key2"]);
        }

        public class B
        {
            public IDictionary<string, string> Dictionary { get; set; }

            public B(IDictionary<string, string> dictionary)
            {
                Dictionary = dictionary;
            }
        }

        [Test]
        public void InjectsDictionaryParameter()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_DictionaryParameters.xml").Build();

            var poco = container.Resolve<B>();

            Assert.True(poco.Dictionary.Count == 2);
            Assert.True(poco.Dictionary.ContainsKey("Key1"));
            Assert.True(poco.Dictionary.ContainsKey("Key2"));
             Assert.AreEqual("Val1", poco.Dictionary["Key1"]);
             Assert.AreEqual("Val2", poco.Dictionary["Key2"]);
        }

        public class C
        {
            public IDictionary Dictionary { get; set; }
        }

        [Test]
        public void InjectsNonGenericDictionary()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_DictionaryParameters.xml").Build();

            var poco = container.Resolve<C>();

            Assert.True(poco.Dictionary.Count == 2);
            Assert.True(poco.Dictionary.Contains("Key1"));
            Assert.True(poco.Dictionary.Contains("Key2"));
             Assert.AreEqual("Val1", poco.Dictionary["Key1"]);
             Assert.AreEqual("Val2", poco.Dictionary["Key2"]);
        }

        public class D
        {
            public Dictionary<string, string> Dictionary { get; set; }
        }

        [Test]
        public void InjectsConcreteDictionary()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_DictionaryParameters.xml").Build();

            var poco = container.Resolve<D>();

            Assert.True(poco.Dictionary.Count == 2);
            Assert.True(poco.Dictionary.ContainsKey("Key1"));
            Assert.True(poco.Dictionary.ContainsKey("Key2"));
             Assert.AreEqual("Val1", poco.Dictionary["Key1"]);
             Assert.AreEqual("Val2", poco.Dictionary["Key2"]);
        }

        public class E
        {
            public IDictionary<int, string> Dictionary { get; set; }
        }

        [Test]
        public void NumericKeysZeroBasedListConvertedToDictionary()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_DictionaryParameters.xml").Build();

            var poco = container.Resolve<E>();

            Assert.True(poco.Dictionary.Count == 2);
            Assert.True(poco.Dictionary.ContainsKey(0));
            Assert.True(poco.Dictionary.ContainsKey(1));
             Assert.AreEqual("Val1", poco.Dictionary[0]);
             Assert.AreEqual("Val2", poco.Dictionary[1]);
        }

        public class F
        {
            public IDictionary<string, int> Dictionary { get; set; }
        }

        [Test]
        public void ConvertsDictionaryValue()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_DictionaryParameters.xml").Build();

            var poco = container.Resolve<F>();

            Assert.True(poco.Dictionary.Count == 2);
            Assert.True(poco.Dictionary.ContainsKey("Key1"));
            Assert.True(poco.Dictionary.ContainsKey("Key2"));
             Assert.AreEqual(1, poco.Dictionary["Key1"]);
             Assert.AreEqual(2, poco.Dictionary["Key2"]);
        }

        public class G
        {
            public IDictionary<int, string> Dictionary { get; set; }
        }

        [Test]
        public void NumericKeysZeroBasedNonSequential()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_DictionaryParameters.xml").Build();

            var poco = container.Resolve<G>();

            Assert.True(poco.Dictionary.Count == 3);
            Assert.True(poco.Dictionary.ContainsKey(0));
            Assert.True(poco.Dictionary.ContainsKey(5));
            Assert.True(poco.Dictionary.ContainsKey(10));
             Assert.AreEqual("Val0", poco.Dictionary[0]);
             Assert.AreEqual("Val1", poco.Dictionary[5]);
             Assert.AreEqual("Val2", poco.Dictionary[10]);
        }
    }
}