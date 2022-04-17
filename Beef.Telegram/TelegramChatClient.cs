using Discord;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Beef.Telegram;

public class TelegramChatClient : IDiscordClient
{
    private readonly ITelegramBotClient _api;
    private readonly ITelegramGuildCache _telegramGuildCache;
    private readonly ITelegramGuildFactory _telegramGuildFactory;
    private readonly IOptions<TelegramOptions> _telegramOptions;
    private IApplication? _application;
    private CancellationTokenSource _cancellationTokenSource = new();

    public TelegramChatClient(
        IOptions<TelegramOptions> telegramOptions,
        ITelegramGuildFactory telegramGuildFactory,
        ITelegramBotClient api,
        ITelegramGuildCache telegramGuildCache
    )
    {
        _telegramOptions = telegramOptions;
        _telegramGuildFactory = telegramGuildFactory;
        _api = api;
        _telegramGuildCache = telegramGuildCache;
    }

    public void Dispose()
    {
        StopAsync();
    }

    public Task StartAsync()
    {
        _api.StartReceiving(OnUpdateAsync, OnErrorAsync, cancellationToken: _cancellationTokenSource.Token);
        CurrentUser = new TelegramSelfUser(_api.BotId.Value);
        _application = new TelegramApplication(_telegramOptions.Value.BotOwnerId);
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        return Task.CompletedTask;
    }

    public Task<IApplication?> GetApplicationInfoAsync(RequestOptions? options = null)
    {
        return Task.FromResult(_application);
    }

    public async Task<IChannel?> GetChannelAsync(
        ulong id,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        var channel = await _telegramGuildFactory.CreateAsync((long)id);
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
        var guild = await _telegramGuildFactory.CreateAsync((long)id);
        return guild;
    }

    public async Task<IReadOnlyCollection<IGuild>> GetGuildsAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        var guilds = Task.WhenAll(
            _telegramGuildCache.GetAllCachedChatIds()
                .Select(_telegramGuildFactory.CreateAsync)
        );
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
    public ISelfUser? CurrentUser { get; private set; }
    public TokenType TokenType => TokenType.Bot;

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    private Task OnErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public event Func<Update, Task> Update = _ => Task.CompletedTask;

    private async Task OnUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        await Update(update);
    }

    public async Task<byte[]> DownloadFileAsync(string fileId)
    {
        var memoryStream = new MemoryStream();
        await _api.GetInfoAndDownloadFileAsync(fileId, memoryStream);
        return memoryStream.ToArray();
    }

    public async Task<string?> GetAvatarIdAsync(IUser user)
    {
        var userPhotos = (await _api.GetUserProfilePhotosAsync((int)user.Id, 0, 1)).Photos;
        return userPhotos.FirstOrDefault()?.FirstOrDefault().FileId;
    }
}