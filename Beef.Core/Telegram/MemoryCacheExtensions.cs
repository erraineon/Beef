using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace Beef.Core.Telegram
{
    public static class MemoryCacheExtensions
    {
        private static readonly ConcurrentDictionary<object, SemaphoreSlim> Semaphores =
            new();
        public static async Task<TItem> GetOrCreateAsyncLock<TItem>(
            this IMemoryCache cache,
            object key,
            Func<ICacheEntry, Task<TItem>> factory)
        {
            var semaphore = Semaphores.GetOrAdd(key, _ => new SemaphoreSlim(1));
            await semaphore.WaitAsync();
            try
            {
                return await cache.GetOrCreateAsync(key, factory);
            }
            finally
            {
                semaphore.Release();
                Semaphores.Remove(key, out _);
            }
        }
    }
}