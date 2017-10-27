using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Autofac.Configuration.Core;
using Autofac.Configuration.Util;
using Autofac.Core;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Autofac;

namespace Orchard.Tests.Environment.AutofacUtil.Configuration.Core
{
    [TestFixture]
    public class ConfigurationExtensionsFixture : ConfigFixtureBase
    {
        [Theory]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void DefaultAssembly_AssemblyNameEmpty(string value)
        {
            var config = SetUpDefaultAssembly(value);
            Assert.Null(config.DefaultAssembly());
        }

        [Test]
        public void DefaultAssembly_AssemblyNameMissing()
        {
            var config = new ConfigurationBuilder().Build();
            Assert.Null(config.DefaultAssembly());
        }

        [Test]
        public void DefaultAssembly_AssemblyNotFound()
        {
            var config = SetUpDefaultAssembly("NoSuchAssembly");
            Assert.Throws<FileNotFoundException>(() => config.DefaultAssembly());
        }

        [Test]
        public void DefaultAssembly_FullAssemblyName()
        {
            var expected = typeof(string).GetTypeInfo().Assembly;
            var config = SetUpDefaultAssembly(expected.FullName);
            Assert.AreEqual(expected, config.DefaultAssembly());
        }

        [Test]
        public void DefaultAssembly_NullConfiguration()
        {
            var config = (IConfiguration)null;
            Assert.Throws<ArgumentNullException>(() => config.DefaultAssembly());
        }

        [Test]
        public void DefaultAssembly_SimpleAssemblyName()
        {
            // String is in a different assembly depending on the
            // target framework. We have to calculate it and truncate
            // the full assembly name at the first comma.
            var expected = typeof(string).GetTypeInfo().Assembly;
            var fullName = expected.FullName.Substring(0, expected.FullName.IndexOf(','));
            var config = SetUpDefaultAssembly(fullName);
            Assert.AreEqual(expected, config.DefaultAssembly());
        }

        [Theory]
        [TestCase("")]
        [TestCase("   ")]
        public void GetAssembly_EmptyKey(string key)
        {
            var config = new ConfigurationBuilder().Build();
            Assert.Throws<ArgumentException>(() => config.GetAssembly(key));
        }

        [Test]
        public void GetAssembly_NullConfiguration()
        {
            var config = (IConfiguration)null;
            Assert.Throws<ArgumentNullException>(() => config.GetAssembly("defaultAssembly"));
        }

        [Test]
        public void GetAssembly_NullKey()
        {
            var config = new ConfigurationBuilder().Build();
            Assert.Throws<ArgumentNullException>(() => config.GetAssembly(null));
        }

        [Test]
        public void GetParameters_ListParameterPopulated()
        {
            var config = LoadJson("ConfigurationExtensions_Parameters.json");
            var component = config.GetSection("components").GetChildren().Where(kvp => kvp["type"] == typeof(HasEnumerableParameter).FullName).First();
            var objectParameter = typeof(HasEnumerableParameter).GetConstructors().First().GetParameters().First(pi => pi.Name == "list");
            var provider = (Func<object>)null;
            var parameter = component.GetParameters("parameters").Cast<Parameter>().FirstOrDefault(rp => rp.CanSupplyValue(objectParameter, new ContainerBuilder().Build(), out provider));
            Assert.NotNull(parameter);
            Assert.NotNull(provider);
            Assert.AreEqual(new List<string> { "a", "b" }, provider());
        }

        [Test]
        public void GetParameters_ParameterConversionUsesTypeConverterAttribute()
        {
            var container = ConfigureContainerWithJson("ConfigurationExtensions_Parameters.json").Build();
            var obj = container.Resolve<HasConvertibleParametersAndProperties>();
            Assert.NotNull(obj.Parameter);
            Assert.AreEqual(1, obj.Parameter.Value);
        }

        [Theory]
        [TestCase("text", "text")]
        [TestCase("url", "http://localhost")]
        public void GetParameters_SimpleParameters(string parameterName, object expectedValue)
        {
            var config = LoadJson("ConfigurationExtensions_Parameters.json");
            var component = config.GetSection("components").GetChildren().Where(kvp => kvp["type"] == typeof(HasSimpleParametersAndProperties).FullName).First();
            var objectParameter = typeof(HasSimpleParametersAndProperties).GetConstructors().First().GetParameters().First(pi => pi.Name == parameterName);
            var provider = (Func<object>)null;
            var parameter = component.GetParameters("parameters").Cast<Parameter>().FirstOrDefault(rp => rp.CanSupplyValue(objectParameter, new ContainerBuilder().Build(), out provider));
            Assert.NotNull(parameter);
            Assert.NotNull(provider);
            Assert.AreEqual(expectedValue, provider());
        }

        [Test]
        public void GetProperties_DictionaryPropertyEmpty()
        {
            var config = LoadJson("ConfigurationExtensions_Parameters.json");
            var component = config.GetSection("components").GetChildren().Where(kvp => kvp["type"] == typeof(HasDictionaryProperty).FullName).First();
            var property = typeof(HasDictionaryProperty).GetProperty("Empty");
            var provider = (Func<object>)null;
            var parameter = component.GetProperties("properties").Cast<Parameter>().FirstOrDefault(rp => rp.CanSupplyValue(property.SetMethod.GetParameters().First(), new ContainerBuilder().Build(), out provider));

            // Gotcha in ConfigurationModel - if the list/dictionary is empty
            // then configuration won't see it or add the key to the list.
            Assert.Null(parameter);
        }

        [Test]
        public void GetProperties_DictionaryPropertyPopulated()
        {
            var config = LoadJson("ConfigurationExtensions_Parameters.json");
            var component = config.GetSection("components").GetChildren().Where(kvp => kvp["type"] == typeof(HasDictionaryProperty).FullName).First();
            var property = typeof(HasDictionaryProperty).GetProperty("Populated");
            var provider = (Func<object>)null;
            var parameter = component.GetProperties("properties").Cast<Parameter>().FirstOrDefault(rp => rp.CanSupplyValue(property.SetMethod.GetParameters().First(), new ContainerBuilder().Build(), out provider));
            Assert.NotNull(parameter);
            Assert.NotNull(provider);
            var value = provider();
            Assert.NotNull(value);
            Assert.IsAssignableFrom(typeof(Dictionary<string, int>), value);
            var dict = (Dictionary<string, int>)value;
            Assert.AreEqual(1, dict["a"]);
            Assert.AreEqual(2, dict["b"]);
        }

        [Test]
        public void GetProperties_DictionaryPropertyUsesTypeConverterAttribute()
        {
            var container = ConfigureContainerWithJson("ConfigurationExtensions_Parameters.json").Build();
            var obj = container.Resolve<HasDictionaryProperty>();
            Assert.NotNull(obj.Convertible);
            Assert.AreEqual(2, obj.Convertible.Count);
            Assert.AreEqual(1, obj.Convertible["a"].Value);
            Assert.AreEqual(2, obj.Convertible["b"].Value);
        }

        [Test]
        public void GetProperties_ListConversionUsesTypeConverterAttribute()
        {
            var container = ConfigureContainerWithJson("ConfigurationExtensions_Parameters.json").Build();
            var obj = container.Resolve<HasEnumerableProperty>();
            Assert.NotNull(obj.Convertible);
            var convertible = obj.Convertible.ToArray();
            Assert.AreEqual(2, convertible.Length);
            Assert.AreEqual(1, convertible[0].Value);
            Assert.AreEqual(2, convertible[1].Value);
        }

        [Test]
        public void GetProperties_ListPropertyEmpty()
        {
            var config = LoadJson("ConfigurationExtensions_Parameters.json");
            var component = config.GetSection("components").GetChildren().Where(kvp => kvp["type"] == typeof(HasEnumerableProperty).FullName).First();
            var property = typeof(HasEnumerableProperty).GetProperty("Empty");
            var provider = (Func<object>)null;
            var parameter = component.GetProperties("properties").Cast<Parameter>().FirstOrDefault(rp => rp.CanSupplyValue(property.SetMethod.GetParameters().First(), new ContainerBuilder().Build(), out provider));

            // Gotcha in ConfigurationModel - if the list/dictionary is empty
            // then configuration won't see it or add the key to the list.
            Assert.Null(parameter);
        }

        [Test]
        public void GetProperties_ListPropertyPopulated()
        {
            var config = LoadJson("ConfigurationExtensions_Parameters.json");
            var component = config.GetSection("components").GetChildren().Where(kvp => kvp["type"] == typeof(HasEnumerableProperty).FullName).First();
            var property = typeof(HasEnumerableProperty).GetProperty("Populated");
            var provider = (Func<object>)null;
            var parameter = component.GetProperties("properties").Cast<Parameter>().FirstOrDefault(rp => rp.CanSupplyValue(property.SetMethod.GetParameters().First(), new ContainerBuilder().Build(), out provider));
            Assert.NotNull(parameter);
            Assert.NotNull(provider);
            Assert.AreEqual(new List<int> { 1, 2 }, provider());
        }

        [Test]
        public void GetProperties_PropertyConversionUsesTypeConverterAttribute()
        {
            var container = ConfigureContainerWithJson("ConfigurationExtensions_Parameters.json").Build();
            var obj = container.Resolve<HasConvertibleParametersAndProperties>();
            Assert.NotNull(obj.Property);
            Assert.AreEqual(2, obj.Property.Value);
        }

        [Theory]
        [TestCase("text", "text")]
        [TestCase("url", "http://localhost")]
        public void GetProperties_SimpleProperties(string propertyName, object expectedValue)
        {
            var config = LoadJson("ConfigurationExtensions_Parameters.json");
            var component = config.GetSection("components").GetChildren().Where(kvp => kvp["type"] == typeof(HasSimpleParametersAndProperties).FullName).First();
            var property = typeof(HasSimpleParametersAndProperties).GetProperties().First(pi => pi.Name == propertyName);
            var provider = (Func<object>)null;
            var parameter = component.GetProperties("properties").Cast<Parameter>().FirstOrDefault(rp => rp.CanSupplyValue(property.SetMethod.GetParameters().First(), new ContainerBuilder().Build(), out provider));
            Assert.NotNull(parameter);
            Assert.NotNull(provider);
            Assert.AreEqual(expectedValue, provider());
        }

        private static IEnumerable<object[]> GetParameters_SimpleParameters_Source()
        {
            yield return new object[] { "number", 1 };
            yield return new object[] { "ip", IPAddress.Parse("127.0.0.1") };
        }

        private static IEnumerable<object[]> GetProperties_SimpleProperties_Source()
        {
            yield return new object[] { "Text", "text" };
            yield return new object[] { "Url", new Uri("http://localhost") };
        }

        private static IConfiguration SetUpDefaultAssembly(string assemblyName)
        {
            var data = new Dictionary<string, string>
            {
                { "defaultAssembly", assemblyName },
            };
            return new ConfigurationBuilder().AddInMemoryCollection(data).Build();
        }

        public class BaseSimpleParametersAndProperties
        {
            // Issue #2 - Ensure properties in base classes can be set by config.
            public string Text { get; set; }
        }

        public class Convertible
        {
            public int Value { get; set; }
        }

        public class ConvertibleConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value == null)
                {
                    return null;
                }

                string str = value as string;
                if (str == null)
                {
                    return base.ConvertFrom(context, culture, value);
                }

                var converter = TypeDescriptor.GetConverter(typeof(int));
                return new Convertible { Value = (int)converter.ConvertFromString(context, culture, str) };
            }
        }

        public class ConvertibleDictionaryConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value == null)
                {
                    return null;
                }
                return base.ConvertFrom(context, culture, value);
            }
        }

        public class ConvertibleListConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value == null)
                {
                    return null;
                }
                return base.ConvertFrom(context, culture, value);
            }
        }

        public class HasConvertibleParametersAndProperties
        {
            public HasConvertibleParametersAndProperties([TypeConverter(typeof(ConvertibleConverter))] Convertible parameter)
            {
                this.Parameter = parameter;
            }

            public Convertible Parameter { get; set; }

            [TypeConverter(typeof(ConvertibleConverter))]
            public Convertible Property { get; set; }
        }

        public class HasDictionaryProperty
        {
            [TypeConverter(typeof(ConvertibleDictionaryConverter))]
            public IDictionary<string, Convertible> Convertible { get; set; }

            public Dictionary<string, int> Empty { get; set; }

            public Dictionary<string, int> Populated { get; set; }
        }

        public class HasEnumerableParameter
        {
            public HasEnumerableParameter(IList<string> list)
            {
                this.List = list;
            }

            public IList<string> List { get; private set; }
        }

        public class HasEnumerableProperty
        {
            [TypeConverter(typeof(ConvertibleListConverter))]
            public IEnumerable<Convertible> Convertible { get; set; }

            public IEnumerable<int> Empty { get; set; }

            public IEnumerable<int> Populated { get; set; }
        }

        public class HasSimpleParametersAndProperties : BaseSimpleParametersAndProperties
        {
            public HasSimpleParametersAndProperties(int number, IPAddress ip)
            {
                this.Number = number;
                this.IP = ip;
            }

            public IPAddress IP { get; private set; }

            public int Number { get; private set; }

            public Uri Url { get; set; }
        }
    }
}
