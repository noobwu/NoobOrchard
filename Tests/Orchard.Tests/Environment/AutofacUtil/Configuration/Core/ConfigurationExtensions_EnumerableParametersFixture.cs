using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Autofac;

namespace Orchard.Tests.Environment.AutofacUtil.Configuration.Core
{
    [TestFixture]
    public class ConfigurationExtensions_EnumerableParametersFixture : ConfigFixtureBase
    {
        public class A
        {
            public IList<string> List { get; set; }
        }

        [Test]
        public void PropertyStringListInjection()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_EnumerableParameters.xml").Build();

            var poco = container.Resolve<A>();

            Assert.True(poco.List.Count == 2);
             Assert.AreEqual(poco.List[0], "Val1");
             Assert.AreEqual(poco.List[1], "Val2");
        }

        public class B
        {
            public IList<int> List { get; set; }
        }

        [Test]
        public void ConvertsTypeInList()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_EnumerableParameters.xml").Build();

            var poco = container.Resolve<B>();

            Assert.True(poco.List.Count == 2);
             Assert.AreEqual(poco.List[0], 1);
             Assert.AreEqual(poco.List[1], 2);
        }

        public class C
        {
            public IList List { get; set; }
        }

        [Test]
        public void FillsNonGenericListWithString()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_EnumerableParameters.xml").Build();

            var poco = container.Resolve<C>();

            Assert.True(poco.List.Count == 2);
             Assert.AreEqual(poco.List[0], "1");
             Assert.AreEqual(poco.List[1], "2");
        }

        public class D
        {
            public int Num { get; set; }
        }

        [Test]
        public void InjectsSingleValueWithConversion()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_EnumerableParameters.xml").Build();

            var poco = container.Resolve<D>();

            Assert.True(poco.Num == 123);
        }

        public class E
        {
            public IList<int> List { get; set; }

            public E(IList<int> list)
            {
                List = list;
            }
        }

        [Test]
        public void InjectsConstructorParameter()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_EnumerableParameters.xml").Build();

            var poco = container.Resolve<E>();

            Assert.True(poco.List.Count == 2);
             Assert.AreEqual(poco.List[0], 1);
             Assert.AreEqual(poco.List[1], 2);
        }

        public class G
        {
            public IEnumerable Enumerable { get; set; }
        }

        [Test]
        public void InjectsIEnumerable()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_EnumerableParameters.xml").Build();

            var poco = container.Resolve<G>();

            Assert.NotNull(poco.Enumerable);
            var enumerable = poco.Enumerable.Cast<string>().ToList();
            Assert.True(enumerable.Count == 2);
             Assert.AreEqual(enumerable[0], "Val1");
             Assert.AreEqual(enumerable[1], "Val2");
        }

        public class H
        {
            public IEnumerable<int> Enumerable { get; set; }
        }

        [Test]
        public void InjectsGenericIEnumerable()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_EnumerableParameters.xml").Build();

            var poco = container.Resolve<H>();

            Assert.NotNull(poco.Enumerable);
            var enumerable = poco.Enumerable.ToList();
            Assert.True(enumerable.Count == 2);
             Assert.AreEqual(enumerable[0], 1);
             Assert.AreEqual(enumerable[1], 2);
        }

        public class I
        {
            public ICollection<int> Collection { get; set; }
        }

        [Test]
        public void InjectsGenericCollection()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_EnumerableParameters.xml").Build();

            var poco = container.Resolve<I>();

            Assert.NotNull(poco.Collection);
            Assert.True(poco.Collection.Count == 2);
             Assert.AreEqual(poco.Collection.First(), 1);
             Assert.AreEqual(poco.Collection.Last(), 2);
        }

        public class J
        {
            public J(IList<string> list)
            {
                this.List = list;
            }

            public IList<string> List { get; private set; }
        }

        [Test]
        public void ParameterStringListInjection()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_EnumerableParameters.xml").Build();

            var poco = container.Resolve<J>();

            Assert.True(poco.List.Count == 2);
             Assert.AreEqual(poco.List[0], "Val1");
             Assert.AreEqual(poco.List[1], "Val2");
        }

        public class K
        {
            public K(IList<string> list = null)
            {
                this.List = list;
            }

            public IList<string> List { get; private set; }
        }

        [Test]
        public void ParameterStringListInjectionOptionalParameter()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_EnumerableParameters.xml").Build();

            var poco = container.Resolve<K>();

            Assert.True(poco.List.Count == 2);
             Assert.AreEqual(poco.List[0], "Val1");
             Assert.AreEqual(poco.List[1], "Val2");
        }

        public class L
        {
            public L()
            {
                this.List = new List<string>();
            }

            public L(IList<string> list = null)
            {
                this.List = list;
            }

            public IList<string> List { get; private set; }
        }

        [Test]
        public void ParameterStringListInjectionMultipleConstructors()
        {
            var container = ConfigureContainerWithXml("ConfigurationExtensions_EnumerableParameters.xml").Build();

            var poco = container.Resolve<L>();

            Assert.True(poco.List.Count == 2);
             Assert.AreEqual(poco.List[0], "Val1");
             Assert.AreEqual(poco.List[1], "Val2");
        }
    }
}
