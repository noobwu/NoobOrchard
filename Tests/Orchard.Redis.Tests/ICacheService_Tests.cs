using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Orchard.Caching.Services;
using System.Threading;

namespace Orchard.Redis.Tests
{
    [TestFixture]
    public class ICacheService_Tests : CachingTestsBase
    {
        private ICacheService _service;
        public override void Register(ContainerBuilder builder)
        {

        }
        public override void Init()
        {
            base.Init();
            _service = Container.Resolve<ICacheService>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        [Test]
        public void ICacheService_GetObject_Test()
        {
            var result = _service.Get<object>("testItem");
            Assert.IsNull(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [Test]
        public void ICacheService_Put_Test()
        {
            var value = "testResult";
            _service.Put("testItem", value);
            var result = _service.Get<string>("testItem");
            Assert.AreEqual(result, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="validFor"></param>
        [Test]
        public void ICacheService_Put_Expiredate_Test()
        {
            var value = "testResult";
            _service.Put("testItem", value,TimeSpan.FromSeconds(5));
            var result = _service.Get<string>("testItem");
            Assert.AreEqual(result, value);
            Thread.Sleep(6 * 1000);
            result = _service.Get<string>("testItem");
            Assert.AreNotEqual(result, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        [Test]
        public void ICacheService_Remove_Test()
        {
            var key = "testItem";
            var value = "testResult";
            var result = _service.Get(key,()=> { return value; });
            Assert.AreEqual(result, value);
            _service.Remove(key);
            result = _service.Get<string>(key);
            Assert.AreNotEqual(result, value);
        }
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void ICacheService_Clear_Test()
        {
            var key = "testItem";
            var value = "testResult";
            var result = _service.Get(key, () => { return value; });
            Assert.AreEqual(result, value);
            _service.Clear();
            result = _service.Get<string>(key);
            Assert.AreNotEqual(result, value);
        }
    }
}
