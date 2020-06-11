﻿namespace Vints.Utility.Caching
{
    using System;
    using System.Runtime.Caching;
    using System.Threading;
    using PolicyFunc = System.Func<System.Runtime.Caching.CacheItemPolicy>;

    public class CacheService<T>
    {
        private readonly ObjectCache cache;
        private readonly ReaderWriterLockSlim synclock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public CacheService(ObjectCache objectCache)
        {
            if (objectCache == null)
                throw new ArgumentNullException("objectCache");

            this.cache = objectCache;
        }

        public bool Contains(string key)
        {
            synclock.EnterReadLock();
            try
            {
                return this.cache.Contains(key);
            }
            finally
            {
                synclock.ExitReadLock();
            }
        }

        public T GetOrAdd(string key, Func<T> loadFunction, PolicyFunc getCacheItemPolicyFunction)
        {
            LazyLock<T> lazy;
            bool success;

            synclock.EnterReadLock();
            try
            {
                success = this.TryGetValue(key, out lazy);
            }
            finally
            {
                synclock.ExitReadLock();
            }

            if (!success)
            {
                synclock.EnterWriteLock();
                try
                {
                    if (!this.TryGetValue(key, out lazy))
                    {
                        lazy = new LazyLock<T>();
                        var policy = getCacheItemPolicyFunction();
                        this.cache.Add(key, lazy, policy);
                    }
                }
                finally
                {
                    synclock.ExitWriteLock();
                }
            }

            return lazy.Get(loadFunction);
        }

        public void Remove(string key)
        {
            synclock.EnterWriteLock();
            try
            {
                this.cache.Remove(key);
            }
            finally
            {
                synclock.ExitWriteLock();
            }
        }

        private bool TryGetValue(string key, out LazyLock<T> value)
        {
            value = (LazyLock<T>)this.cache.Get(key);
            if (value != null)
            {
                return true;
            }
            return false;
        }

        private sealed class LazyLock<TL>
        {
            private volatile bool got;

            private TL value;
            public TL Get(Func<TL> activator)
            {
                if (!got)
                {
                    if (activator == null)
                    {
                        return default(TL);
                    }

                    lock (this)
                    {
                        if (!got)
                        {
                            value = activator();

                            got = true;
                        }
                    }
                }
                return value;
            }
        }


    }
}
