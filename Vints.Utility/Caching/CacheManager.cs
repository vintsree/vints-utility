namespace Vints.Utility.Caching
{
    using System;
    using System.Runtime.Caching;
    using System.Threading;
    using PolicyFunc = System.Func<System.Runtime.Caching.CacheItemPolicy>;

    public class CacheManager
    {
        private static readonly Lazy<CacheManager> instance = new Lazy<CacheManager>(() => new CacheManager(), LazyThreadSafetyMode.PublicationOnly);
        public static CacheManager Instance { get { return instance.Value; } }

        public T GetCached<T>(string key, Func<T> loadFunction, PolicyFunc getCacheItemPolicyFunction)
        {
            var cache = new CacheService<T>(MemoryCache.Default);
            return cache.GetOrAdd(key, loadFunction, getCacheItemPolicyFunction);
        }

        public T GetCached<T>(string key, Func<T> loadFunction, MemoryCache objectCache, PolicyFunc getCacheItemPolicyFunction)
        {
            var cache = new CacheService<T>(objectCache);
            return cache.GetOrAdd(key, loadFunction, getCacheItemPolicyFunction);
        }

        public T GetCached<T>(string key, Func<T> loadFunction)
        {
            var cache = new CacheService<T>(MemoryCache.Default);
            return cache.GetOrAdd(key, loadFunction, () => { return CreatePolicy(null, null); });
        }

        public CacheItemPolicy CreatePolicy(TimeSpan? slidingExpiration, DateTime? absoluteExpiration)
        {
            var policy = new CacheItemPolicy();

            if (absoluteExpiration.HasValue)
            {
                policy.AbsoluteExpiration = absoluteExpiration.Value;
            }
            else if (slidingExpiration.HasValue)
            {
                policy.SlidingExpiration = slidingExpiration.Value;
            }

            policy.Priority = CacheItemPriority.Default;
            return policy;
        }
    }
}