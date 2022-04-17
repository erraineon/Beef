namespace Beef.Telegram
{
    public interface ITelegramGuildCache
    {
        Task<TelegramGuild> GetOrCreateAsync(long guildId, Func<Task<TelegramGuild>> factory);
        ICollection<long> GetAllCachedChatIds();
    }
}