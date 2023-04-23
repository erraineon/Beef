using Discord;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
#pragma warning disable CS8625

namespace Beef.Core.Telegram;

public class TelegramChatClient : IDiscordClient
{
    private readonly ILogger<TelegramChatClient> _logger;
    private readonly ITelegramGuildCache _telegramGuildCache;
    private readonly IOptions<TelegramOptions> _telegramOptions;
    private CancellationTokenSource _cancellationTokenSource = new();
    private ITelegramBotClient? _telegramBotClient;

    public TelegramChatClient(
        IOptions<TelegramOptions> telegramOptions,
        ITelegramGuildCache telegramGuildCache,
        ILogger<TelegramChatClient> logger
    )
    {
        _telegramOptions = telegramOptions;
        _telegramGuildCache = telegramGuildCache;
        _logger = logger;
    }

    public ITelegramBotClient Client =>
        _telegramBotClient ?? throw new Exception("The Telegram client is not running.");

    public void Dispose()
    {
        StopAsync();
    }

    public Task StartAsync()
    {
        _telegramBotClient = new TelegramBotClient(_telegramOptions.Value.Token);
        Client.StartReceiving(
            OnUpdateAsync,
            errorHandler: OnErrorAsync,
            cancellationToken: _cancellationTokenSource.Token,
            receiverOptions: default
        );
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        _telegramBotClient = null;
        return Task.CompletedTask;
    }

    public Task<IApplication?> GetApplicationInfoAsync(RequestOptions? options = null)
    {
        return Task.FromResult((IApplication?)new TelegramApplication(_telegramOptions.Value.BotOwnerId));
    }

    public async Task<IChannel?> GetChannelAsync(
        ulong id,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        var channel = await GetOrCreateTelegramGuildAsync((long)id);
        return channel;
    }

    public Task<IReadOnlyCollection<IPrivateChannel>> GetPrivateChannelsAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IDMChannel>> GetDMChannelsAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IGroupChannel>> GetGroupChannelsAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IConnection>> GetConnectionsAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IApplicationCommand> GetGlobalApplicationCommandAsync(ulong id, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IApplicationCommand>> GetGlobalApplicationCommandsAsync(
        bool withLocalizations = false,
        string locale = null,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IApplicationCommand> CreateGlobalApplicationCommand(
        ApplicationCommandProperties properties,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IApplicationCommand>> BulkOverwriteGlobalApplicationCommand(
        ApplicationCommandProperties[] properties,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public async Task<IGuild> GetGuildAsync(
        ulong id,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        var guild = await GetOrCreateTelegramGuildAsync((long)id);
        return guild;
    }

    public async Task<IReadOnlyCollection<IGuild>> GetGuildsAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        var guilds = Task.WhenAll(_telegramGuildCache.GetAllCachedChatIds().Select(GetOrCreateTelegramGuildAsync));
        return await guilds;
    }

    public Task<IGuild> CreateGuildAsync(
        string name,
        IVoiceRegion region,
        Stream? jpegIcon = null,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IInvite> GetInviteAsync(string inviteId, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IUser> GetUserAsync(
        ulong id,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IUser> GetUserAsync(string username, string discriminator, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IVoiceRegion>> GetVoiceRegionsAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IVoiceRegion> GetVoiceRegionAsync(string id, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetRecommendedShardCountAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<BotGateway> GetBotGatewayAsync(RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public ConnectionState ConnectionState => throw new NotImplementedException();

    public ISelfUser CurrentUser => new TelegramSelfUser(
        Client.BotId ?? throw new Exception("Could not retrieve the Telegram bot ID.")
    );

    public TokenType TokenType => TokenType.Bot;

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Task<IReadOnlyCollection<IApplicationCommand>> GetGlobalApplicationCommandsAsync(
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    private async Task<TelegramGuild> GetOrCreateTelegramGuildAsync(long chatId)
    {
        var guild = await _telegramGuildCache.GetOrCreateAsync(
            chatId,
            () => CreateTelegramGuildAsync(chatId)
        );
        return guild;
    }

    private async Task<TelegramGuild> GetOrCreateTelegramGuildAsync(Chat chat)
    {
        var guild = await _telegramGuildCache.GetOrCreateAsync(
            chat.Id,
            () => CreateTelegramGuildAsync(chat)
        );
        return guild;
    }

    private async Task<TelegramGuild> CreateTelegramGuildAsync(long chatId)
    {
        var chat = await Client.GetChatAsync(new ChatId(chatId), CancellationToken.None);
        return await CreateTelegramGuildAsync(chat);
    }

    public async Task<TelegramGuild> CreateTelegramGuildAsync(Chat chat)
    {
        var telegramGuild = new TelegramGuild(
            chat,
            Client
        );
        var chatAdministrators = await Client.GetChatAdministratorsAsync(chat.Id);
        foreach (var chatAdministrator in chatAdministrators) telegramGuild.CreateGuildUser(chatAdministrator);
        return telegramGuild;
    }

    private Task OnErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public event Func<IUserMessage, Task> MessageReceived = _ => Task.CompletedTask;

    private async Task OnUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Message is { Chat.Type: not ChatType.Private } telegramMessage)
            {
                var telegramGuild = await GetOrCreateTelegramGuildAsync(telegramMessage.Chat);
                var message = telegramGuild.CacheMessage(telegramMessage);
                await MessageReceived(message);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while handling a telegram update.");
        }
    }

    public async Task<byte[]> DownloadFileAsync(string fileId)
    {
        var memoryStream = new MemoryStream();
        await Client.GetInfoAndDownloadFileAsync(fileId, memoryStream);
        return memoryStream.ToArray();
    }

    public async Task<string?> GetAvatarIdAsync(IUser user)
    {
        var userPhotos = (await Client.GetUserProfilePhotosAsync((int)user.Id, 0, 1)).Photos;
        return userPhotos.FirstOrDefault()?.FirstOrDefault()?.FileId;
    }
}