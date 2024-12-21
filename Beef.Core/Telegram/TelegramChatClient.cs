using Discord;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
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
        GC.SuppressFinalize(this);
        StopAsync();
    }

    public Task StartAsync()
    {
        var client = new TelegramBotClient(
            _telegramOptions.Value.Token,
            cancellationToken: _cancellationTokenSource.Token
        );
        client.OnError += OnErrorAsync;
        client.OnMessage += OnUpdateAsync;
        _telegramBotClient = client;
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

    public Task<IEntitlement> CreateTestEntitlementAsync(ulong skuId, ulong ownerId, SubscriptionOwnerType ownerType,
        RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTestEntitlementAsync(ulong entitlementId, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<IReadOnlyCollection<IEntitlement>> GetEntitlementsAsync(
        int limit = 100,
        ulong? afterId = null,
        ulong? beforeId = null,
        bool excludeEnded = false,
        ulong? guildId = null,
        ulong? userId = null,
        ulong[] skuIds = null,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<IReadOnlyCollection<IEntitlement>> GetEntitlementsAsync(int? limit, ulong? afterId = null, ulong? beforeId = null,
        bool excludeEnded = false, ulong? guildId = null, ulong? userId = null, ulong[] skuIds = null,
        RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<SKU>> GetSKUsAsync(RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task ConsumeEntitlementAsync(ulong entitlementId, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<IReadOnlyCollection<ISubscription>> GetSKUSubscriptionsAsync(
        ulong skuId,
        int limit = 100,
        ulong? afterId = null,
        ulong? beforeId = null,
        ulong? userId = null,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<ISubscription> GetSKUSubscriptionAsync(ulong skuId, ulong subscriptionId, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<Emote> GetApplicationEmoteAsync(ulong emoteId, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<Emote>> GetApplicationEmotesAsync(RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<Emote> ModifyApplicationEmoteAsync(ulong emoteId, Action<ApplicationEmoteProperties> args, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<Emote> CreateApplicationEmoteAsync(string name, Image image, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task DeleteApplicationEmoteAsync(ulong emoteId, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public ConnectionState ConnectionState => throw new NotImplementedException();

    public ISelfUser CurrentUser => new TelegramSelfUser(Client.BotId);

    public TokenType TokenType => TokenType.Bot;

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
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

    private Task OnErrorAsync(Exception exception, HandleErrorSource source)
    {
        return Task.CompletedTask;
    }

    public event Func<IUserMessage, Task> MessageReceived = _ => Task.CompletedTask;

    private async Task OnUpdateAsync(Message message, UpdateType type)
    {
        try
        {
            if (message is { Chat.Type: not ChatType.Private } telegramMessage)
            {
                var telegramGuild = await GetOrCreateTelegramGuildAsync(telegramMessage.Chat);
                var cachedMessage = telegramGuild.CacheMessage(telegramMessage);
                await MessageReceived(cachedMessage);
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
        await Client.GetInfoAndDownloadFile(fileId, memoryStream);
        return memoryStream.ToArray();
    }

    public async Task<string?> GetAvatarIdAsync(IUser user)
    {
        var userPhotos = (await Client.GetUserProfilePhotosAsync((int)user.Id, 0, 1)).Photos;
        return userPhotos.FirstOrDefault()?.FirstOrDefault()?.FileId;
    }
}