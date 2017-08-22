using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Cinch.CacheAside
{    
    public static class IMemoryCacheExtensions
    {
        public static T GetOrAdd<T>(this IMemoryCache cache, string key, int seconds, Func<T> factory, bool isAbsolute = true)
        {
            return cache.GetOrCreate<T>(key, entry => new Lazy<T>(() =>
            {
                if (isAbsolute)
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(seconds);
                else
                    entry.SlidingExpiration = TimeSpan.FromSeconds(seconds);

                return factory.Invoke();
            }).Value);
        }

        public static Task<T> GetOrAddAsync<T>(this IMemoryCache cache, string key, int seconds, Func<Task<T>> taskFactory, bool isAbsolute = true)
        {
            return cache.GetOrCreateAsync<T>(key, async entry => await new AsyncLazy<T>(async () =>
            { 
                if (isAbsolute)
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(seconds);
                else
                    entry.SlidingExpiration = TimeSpan.FromSeconds(seconds);

                return await taskFactory.Invoke();
            }).Value);
        }
    }
}
