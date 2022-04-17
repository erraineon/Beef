using Telegram.Bot.Types;

namespace Beef.Telegram
{
    public class CachedTelegramGuildFactoryDecorator : ITelegramGuildFactory
    {
        private readonly ITelegramGuildFactory _telegramGuildFactory;
        private readonly ITelegramGuildCache _telegramGuildCache;

        public CachedTelegramGuildFactoryDecorator(
            ITelegramGuildFactory telegramGuildFactory,
            ITelegramGuildCache telegramGuildCache)
        {
            _telegramGuildFactory = telegramGuildFactory;
            _telegramGuildCache = telegramGuildCache;
        }

        public async Task<TelegramGuild> CreateAsync(long chatId)
        {
            var guild = await _telegramGuildCache.GetOrCreateAsync(
                chatId, 
                () => _telegramGuildFactory.CreateAsync(chatId));
            return guild;
        }

        public async Task<TelegramGuild> CreateAsync(Chat chat)
        {
            var guild = await _telegramGuildCache.GetOrCreateAsync(
                chat.Id,
                () => _telegramGuildFactory.CreateAsync(chat));
            return guild;
        }
    }
}