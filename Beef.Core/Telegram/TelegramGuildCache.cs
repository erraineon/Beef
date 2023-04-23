using Microsoft.Extensions.Caching.Memory;

namespace Beef.Core.Telegram;

public class TelegramGuildCache : ITelegramGuildCache
{
    private readonly IMemoryCache _memoryCache;

    public TelegramGuildCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<TelegramGuild> GetOrCreateAsync(long guildId, Func<Task<TelegramGuild>> factory)
    {
        var guild = await _memoryCache.GetOrCreateAsyncLock(
            $"telegram-chats-{guildId}",
            async e =>
            {
                e.SlidingExpiration = TimeSpan.FromDays(1);
                var guild = await factory();
                var cachedChatIds = GetAllCachedChatIds();
                cachedChatIds.Add(guildId);
                return guild;
            }
        );
        return guild;
    }

    public ICollection<long> GetAllCachedChatIds()
    {
        var chatIds = _memoryCache.GetOrCreate("telegram-chats-list", _ => new HashSet<long>());
        return chatIds;
    }
}