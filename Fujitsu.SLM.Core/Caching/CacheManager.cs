using System;
using System.Runtime.Caching;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Core.Properties;

namespace Fujitsu.SLM.Core.Caching
{
    public class CacheManager : ICacheManager, IDisposable
    {
        private readonly ObjectCache _cache;
        private readonly int _cacheDuration;
        private static volatile object _cacheLock = new object();

        public CacheManager()
        {
            this._cache = MemoryCache.Default;
            this._cacheDuration = Settings.Default.DefaultCacheDuration;
        }

        public TResult ExecuteAndCache<TResult>(string cacheItemKey, Func<TResult> underlyingGet)
        {
            var cacheItem = this._cache.GetCacheItem(cacheItemKey);
            if (cacheItem != null)
            {
                return (TResult)cacheItem.Value;
            }

            lock (_cacheLock)
            {
                cacheItem = this._cache.GetCacheItem(cacheItemKey);
                if (cacheItem != null)
                {
                    return (TResult)cacheItem.Value;
                }
                else
                {
                    return (TResult)this.CacheItem(cacheItemKey, underlyingGet);
                }
            }
        }

        public void Remove(string key)
        {
            if (this._cache.Contains(key))
            {
                lock (_cacheLock)
                {
                    if (this._cache.Contains(key))
                    {
                        this._cache.Remove(key);
                    }
                }
            }
        }

        private object CacheItem<TResult>(string key, Func<TResult> underlyingGet)
        {
            var result = underlyingGet();
            this._cache.Add(key,
                result,
                new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(this._cacheDuration)
                });

            return result;
        }

        public void Dispose()
        {
            // Nothing to do.
        }
    }
}