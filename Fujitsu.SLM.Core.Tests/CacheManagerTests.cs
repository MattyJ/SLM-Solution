using System;
using System.Linq;
using System.Runtime.Caching;
using Fujitsu.SLM.Core.Caching;
using Fujitsu.SLM.Core.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Core.Tests
{
    [TestClass]
    public class CacheManagerTests
    {
        private ICacheManager _target;
        private MemoryCache _cache;
        private const string CacheKey = "XXX";

        [TestInitialize]
        public void Initialize()
        {
            this._cache = MemoryCache.Default;
            var cacheKeys = this._cache.Select(kvp => kvp.Key).ToList();
            foreach (var cacheKey in cacheKeys)
            {
                this._cache.Remove(cacheKey);
            }
            this._target = new CacheManager();
        }

        [TestMethod]
        public void CacheManager_ExecuteAndCache_ReferenceItemNotInCache_ReturnsResults()
        {
            var result = this._target.ExecuteAndCache(CacheKey, GetClass);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(TestCacheClass));
        }

        [TestMethod]
        public void CacheManager_ExecuteAndCache_ReferenceItemNotInCache_ItemAddedToCache()
        {
            this._target.ExecuteAndCache(CacheKey, GetClass);
            var cacheItem = this._cache.Get(CacheKey);
            Assert.IsNotNull(cacheItem);
        }

        [TestMethod]
        public void CacheManager_ExecuteAndCache_ReferenceItemInCache_ItemNotAddedAgain()
        {
            this._target.ExecuteAndCache(CacheKey, GetClass);
            var result = this._target.ExecuteAndCache(CacheKey, GetClassDifferent);
            Assert.IsNotNull(result);
            Assert.AreEqual("XXX", result.Name);
        }

        [TestMethod]
        public void CacheManager_ExecuteAndCache_ValueItemNotInCache_ReturnsResults()
        {
            var result = this._target.ExecuteAndCache(CacheKey, GetInt);
            Assert.AreNotEqual(0, result);
            Assert.IsInstanceOfType(result, typeof(int));
        }

        [TestMethod]
        public void CacheManager_ExecuteAndCache_ValueItemNotInCache_ItemAddedToCache()
        {
            this._target.ExecuteAndCache(CacheKey, GetInt);
            var cacheItem = this._cache.Get(CacheKey);
            Assert.AreNotEqual(0, cacheItem);
        }

        [TestMethod]
        public void CacheManager_ExecuteAndCache_ValueItemInCache_ItemNotAddedAgain()
        {
            this._target.ExecuteAndCache(CacheKey, GetInt);
            var result = this._target.ExecuteAndCache(CacheKey, GetIntDifferent);
            Assert.AreNotEqual(0, result);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void CacheManager_Remove_ValueItemInCache_ItewmIsRemoved()
        {
            this._target.ExecuteAndCache(CacheKey, GetInt);
            this._target.Remove(CacheKey);
            Assert.IsFalse(this._cache.Contains(CacheKey));
        }

        private static TestCacheClass GetClass()
        {
            return new TestCacheClass
            {
                Name = "XXX"
            };
        }

        private static TestCacheClass GetClassDifferent()
        {
            return new TestCacheClass
            {
                Name = "YYY"
            };
        }

        private static int GetInt()
        {
            return 10;
        }

        private static int GetIntDifferent()
        {
            return 100;
        }

        private class TestCacheClass
        {
            public string Name { get; set; }
        }
    }
}
