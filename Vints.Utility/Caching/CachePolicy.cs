namespace Vints.Utility.Caching
{
    using System;
    using System.Runtime.Caching;

    public static class CachePolicy
    {
        public static Func<CacheItemPolicy> Policy { get; set; }


        internal static void RegisterPolicy<T>(this CacheService<T> cacheService, ref Func<CacheItemPolicy> createPolicy)
        {
            if (cacheService == null) return;
            createPolicy = Policy;
        }
    }
}
