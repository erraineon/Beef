using Telegram.Bot;
using Telegram.Bot.Types;

namespace Beef.Telegram;

public class TelegramGuildFactory : ITelegramGuildFactory
{
    private readonly ITelegramBotClient _api;
    private readonly ITelegramUserMessageFactory _telegramUserMessageFactory;
    private readonly TelegramChatClient _telegramChatClient;
    private readonly IUserMessageCache _userMessageCache;

    public TelegramGuildFactory(
        ITelegramBotClient api,
        IUserMessageCache userMessageCache,
        ITelegramUserMessageFactory telegramUserMessageFactory,
        TelegramChatClient telegramChatClient
    )
    {
        _api = api;
        _userMessageCache = userMessageCache;
        _telegramUserMessageFactory = telegramUserMessageFactory;
        _telegramChatClient = telegramChatClient;
    }

    public async Task<TelegramGuild> CreateAsync(long chatId)
    {
        var chat = await _api.GetChatAsync(new ChatId(chatId), CancellationToken.None);
        return await CreateAsync(chat);
    }

    public async Task<TelegramGuild> CreateAsync(Chat chat)
    {
        var telegramGuild = new TelegramGuild(
            chat,
            _userMessageCache,
            _telegramUserMessageFactory,
            _api,
            _telegramChatClient
        );
        var chatAdministrators = await _api.GetChatAdministratorsAsync(chat.Id);
        foreach (var chatAdministrator in chatAdministrators) telegramGuild.CreateGuildUser(chatAdministrator);
        return telegramGuild;
    }
}