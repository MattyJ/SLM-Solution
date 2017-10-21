using System;

namespace Fujitsu.SLM.Core.Interfaces
{
    public interface ICacheManager
    {
        TResult ExecuteAndCache<TResult>(string cacheItemKey, Func<TResult> underlyingGet);
        void Remove(string key);
    }
}
