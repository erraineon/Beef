using Microsoft.Extensions.Caching.Memory;

namespace Beef.Core.Telegram;

public class TelegramGuildCache(IMemoryCache memoryCache) : ITelegramGuildCache
{
    public async Task<TelegramGuild> GetOrCreateAsync(long guildId, Func<Task<TelegramGuild>> factory)
    {
        var guild = await memoryCache.GetOrCreateAsyncLock(
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
        var chatIds = memoryCache.GetOrCreate("telegram-chats-list", _ => new HashSet<long>());
        return chatIds;
    }
}