using Discord;
using Microsoft.Extensions.Caching.Memory;

namespace Beef.Telegram
{
    public class UserMessageCache : IUserMessageCache
    {
        private readonly IMemoryCache _cache;

        public UserMessageCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IEnumerable<IUserMessage> GetCachedUserMessages(ulong guildId)
        {
            var cachedMessages = GetOrCreateQueue(guildId);
            return cachedMessages;
        }

        private FixedSizedQueue<IUserMessage> GetOrCreateQueue(ulong guildId)
        {
            var cacheKey = GetCacheKey(guildId);
            // todo: parametrize queue size
            var cachedMessages = _cache.GetOrCreate(cacheKey, _ => new FixedSizedQueue<IUserMessage>(1000));
            return cachedMessages;
        }

        private static string GetCacheKey(ulong guildId)
        {
            return $"guild-{guildId}-users";
        }

        public void AddUserMessage(IUserMessage userMessage, ulong guildId)
        {
            var queue = GetOrCreateQueue(guildId);
            queue.Enqueue(userMessage);
        }
    }
}