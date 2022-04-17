using Telegram.Bot.Types;

namespace Beef.Telegram
{
    public interface ITelegramGuildFactory
    {
        Task<TelegramGuild> CreateAsync(long chatId);
        Task<TelegramGuild> CreateAsync(Chat chat);
    }
}