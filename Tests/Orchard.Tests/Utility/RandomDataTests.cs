//Github:
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Orchard.Utility;

namespace Orchard.Tests.Utility
{
    public class RandomDataTests
    {
        [Test]
        public void RandomInt()
        {
            int value = RandomData.GetInt(1, 5);
            Assert.IsTrue(value >= 1 && value <= 5);

            value = _numbers.Random();
            Assert.IsTrue(value >= 1 && value <= 3);
        }
        [Test]
        public void RandomLong()
        {
            long value = RandomData.GetLong(1, 5);
            Assert.IsTrue(value >= 1 && value <= 5);
        }
        [Test]
        public void RandomDecimal()
        {
            decimal value = RandomData.GetDecimal(1, 5);
            Assert.IsTrue(value >= 1 && value <= 5);
        }

        [Test]
        public void RandomDouble()
        {
            double value = RandomData.GetDouble(1, 5);
            Assert.IsTrue(value >= 1 && value <= 5);
        }
        [Test]
        public void RandomIp4Address()
        {
            string value = RandomData.GetIp4Address();
            Assert.IsNotEmpty(value);
        }

        [Test]
        public void GetEnumWithOneValueTest()
        {
            var result = RandomData.GetEnum<_days>();

            Assert.AreEqual(_days.Monday, result);
        }

        [Test]
        public void GetSentencesTest()
        {
            var result = RandomData.GetSentence();

            Assert.False(string.IsNullOrEmpty(result));
        }

        private int[] _numbers = new[] { 1, 2, 3 };

        private enum _days
        {
            Monday
        }
    }
}
