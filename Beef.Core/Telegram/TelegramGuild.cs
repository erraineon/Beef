using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Discord;
using Discord.Audio;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace Beef.Core.Telegram;

public class TelegramGuild : IGuild, ITextChannel
{
    private readonly ITelegramBotClient _client;
    private readonly Chat _chat;
    private readonly FixedSizedQueue<IUserMessage> _messageCache = new(1000);

    public TelegramGuild(
        Chat chat,
        ITelegramBotClient client
    )
    {
        _client = client;
        _chat = chat;
    }

    private IDictionary<ulong, TelegramGuildUser> _cachedUsers { get; } = new Dictionary<ulong, TelegramGuildUser>();

    public Task DeleteAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public ulong Id => (ulong)_chat.Id;

    public Task ModifyAsync(Action<GuildProperties> func, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task ModifyWidgetAsync(Action<GuildWidgetProperties> func, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task ReorderChannelsAsync(IEnumerable<ReorderChannelProperties> args, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task ReorderRolesAsync(IEnumerable<ReorderRoleProperties> args, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public async Task LeaveAsync(RequestOptions? options = null)
    {
        await _client.LeaveChatAsync(_chat.Id, options?.CancelToken ?? default);
    }

    public IAsyncEnumerable<IReadOnlyCollection<IBan>> GetBansAsync(int limit = 1000, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<IReadOnlyCollection<IBan>> GetBansAsync(
        ulong fromUserId,
        Direction dir,
        int limit = 1000,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<IReadOnlyCollection<IBan>> GetBansAsync(
        IUser fromUser,
        Direction dir,
        int limit = 1000,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IBan> GetBanAsync(IUser user, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IBan> GetBanAsync(ulong userId, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task AddBanAsync(IUser user, int pruneDays = 0, string? reason = null, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task AddBanAsync(ulong userId, int pruneDays = 0, string? reason = null, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task RemoveBanAsync(IUser user, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task RemoveBanAsync(ulong userId, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IGuildChannel>> GetChannelsAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IGuildChannel> GetChannelAsync(
        ulong id,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<ITextChannel>> GetTextChannelsAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        return Task.FromResult((IReadOnlyCollection<ITextChannel>)new ITextChannel[] { this });
    }

    public Task<ITextChannel> GetTextChannelAsync(
        ulong id,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        return Task.FromResult((ITextChannel)this);
    }

    public Task<IReadOnlyCollection<IVoiceChannel>> GetVoiceChannelsAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<ICategoryChannel>> GetCategoriesAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IVoiceChannel> GetVoiceChannelAsync(
        ulong id,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IStageChannel> GetStageChannelAsync(
        ulong id,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IStageChannel>> GetStageChannelsAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IVoiceChannel> GetAFKChannelAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<ITextChannel> GetSystemChannelAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<ITextChannel> GetDefaultChannelAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IGuildChannel> GetWidgetChannelAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<ITextChannel> GetRulesChannelAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<ITextChannel> GetPublicUpdatesChannelAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IThreadChannel> GetThreadChannelAsync(
        ulong id,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IThreadChannel>> GetThreadChannelsAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<ITextChannel> CreateTextChannelAsync(
        string name,
        Action<TextChannelProperties>? func = null,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IVoiceChannel> CreateVoiceChannelAsync(
        string name,
        Action<VoiceChannelProperties>? func = null,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IStageChannel> CreateStageChannelAsync(
        string name,
        Action<VoiceChannelProperties> func = null,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<ICategoryChannel> CreateCategoryAsync(
        string name,
        Action<GuildChannelProperties>? func = null,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IForumChannel> CreateForumChannelAsync(string name, Action<ForumChannelProperties> func = null, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IVoiceRegion>> GetVoiceRegionsAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    Task<IReadOnlyCollection<IIntegration>> IGuild.GetIntegrationsAsync(RequestOptions options)
    {
        throw new NotImplementedException();
    }

    public Task DeleteIntegrationAsync(ulong id, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IInviteMetadata> GetVanityInviteAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public IRole GetRole(ulong id)
    {
        throw new NotImplementedException();
    }

    public Task<IRole> CreateRoleAsync(
        string name,
        GuildPermissions? permissions = null,
        Color? color = null,
        bool isHoisted = false,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IRole> CreateRoleAsync(
        string name,
        GuildPermissions? permissions = null,
        Color? color = null,
        bool isHoisted = false,
        bool isMentionable = false,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IGuildUser> AddGuildUserAsync(
        ulong userId,
        string accessToken,
        Action<AddGuildUserProperties>? func = null,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task DisconnectAsync(IGuildUser user)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        return Task.FromResult(GetUsers());
    }

    public async Task<IGuildUser?> GetUserAsync(
        ulong id,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        var user = await GetTelegramGuildUserAsync((int)id);
        return user;
    }

    public async Task<IGuildUser> GetCurrentUserAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        var currentApiUser = await _client.GetMeAsync(options?.CancelToken ?? default);
        var currentUser = CreateGuildUser(currentApiUser);
        return currentUser;
    }

    public Task<IGuildUser> GetOwnerAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task DownloadUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<int> PruneUsersAsync(
        int days = 30,
        bool simulate = false,
        RequestOptions options = null,
        IEnumerable<ulong> includeRoleIds = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IGuildUser>> SearchUsersAsync(
        string query,
        int limit = 1000,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IAuditLogEntry>> GetAuditLogsAsync(
        int limit = 100,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null,
        ulong? beforeId = null,
        ulong? userId = null,
        ActionType? actionType = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IWebhook>> GetWebhooksAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<GuildEmote>> GetEmotesAsync(RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<GuildEmote> GetEmoteAsync(ulong id, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<GuildEmote> CreateEmoteAsync(
        string name,
        Image image,
        Optional<IEnumerable<IRole>> roles = new(),
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<GuildEmote> ModifyEmoteAsync(
        GuildEmote emote,
        Action<EmoteProperties> func,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task MoveAsync(IGuildUser user, IVoiceChannel targetChannel)
    {
        throw new NotImplementedException();
    }

    public Task DeleteEmoteAsync(GuildEmote emote, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<ICustomSticker> CreateStickerAsync(
        string name,
        string description,
        IEnumerable<string> tags,
        Image image,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<ICustomSticker> CreateStickerAsync(
        string name,
        string description,
        IEnumerable<string> tags,
        string path,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<ICustomSticker> CreateStickerAsync(
        string name,
        string description,
        IEnumerable<string> tags,
        Stream stream,
        string filename,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<ICustomSticker> GetStickerAsync(
        ulong id,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<ICustomSticker>> GetStickersAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task DeleteStickerAsync(ICustomSticker sticker, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IGuildScheduledEvent> GetEventAsync(ulong id, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IGuildScheduledEvent>> GetEventsAsync(RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IGuildScheduledEvent> CreateEventAsync(
        string name,
        DateTimeOffset startTime,
        GuildScheduledEventType type,
        GuildScheduledEventPrivacyLevel privacyLevel = GuildScheduledEventPrivacyLevel.Private,
        string description = null,
        DateTimeOffset? endTime = null,
        ulong? channelId = null,
        string location = null,
        Image? coverImage = null,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IApplicationCommand>> GetApplicationCommandsAsync(bool withLocalizations = false, string locale = null, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IApplicationCommand>> GetApplicationCommandsAsync(RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IApplicationCommand> GetApplicationCommandAsync(
        ulong id,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IApplicationCommand> CreateApplicationCommandAsync(
        ApplicationCommandProperties properties,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IApplicationCommand>> BulkOverwriteApplicationCommandsAsync(
        ApplicationCommandProperties[] properties,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<WelcomeScreen> GetWelcomeScreenAsync(RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<WelcomeScreen> ModifyWelcomeScreenAsync(
        bool enabled,
        WelcomeScreenChannelProperties[] channels,
        string description = null,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IAutoModRule[]> GetAutoModRulesAsync(RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IAutoModRule> GetAutoModRuleAsync(ulong ruleId, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IAutoModRule> CreateAutoModRuleAsync(Action<AutoModRuleProperties> props, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public string Name => _chat.Title;

    public int AFKTimeout => throw new NotImplementedException();

    public bool IsWidgetEnabled { get; }

    public DefaultMessageNotifications DefaultMessageNotifications => throw new NotImplementedException();

    public MfaLevel MfaLevel => throw new NotImplementedException();

    public VerificationLevel VerificationLevel => throw new NotImplementedException();

    public ExplicitContentFilterLevel ExplicitContentFilter => throw new NotImplementedException();

    public string IconId => throw new NotImplementedException();

    public string IconUrl => throw new NotImplementedException();

    public string SplashId => throw new NotImplementedException();

    public string SplashUrl => throw new NotImplementedException();
    public string DiscoverySplashId { get; }
    public string DiscoverySplashUrl { get; }

    public bool Available => throw new NotImplementedException();

    public ulong? AFKChannelId => throw new NotImplementedException();

    public ulong? WidgetChannelId { get; }

    public ulong? SystemChannelId => throw new NotImplementedException();
    public ulong? RulesChannelId { get; }
    public ulong? PublicUpdatesChannelId { get; }

    public ulong OwnerId => throw new NotImplementedException();

    public ulong? ApplicationId => throw new NotImplementedException();

    public string VoiceRegionId => throw new NotImplementedException();

    public IAudioClient AudioClient => throw new NotImplementedException();

    public IRole EveryoneRole => throw new NotImplementedException();

    public IReadOnlyCollection<GuildEmote> Emotes => throw new NotImplementedException();
    public IReadOnlyCollection<ICustomSticker> Stickers { get; }

    GuildFeatures IGuild.Features { get; }

    public IReadOnlyCollection<IRole> Roles => throw new NotImplementedException();

    public PremiumTier PremiumTier => throw new NotImplementedException();

    public string BannerId => throw new NotImplementedException();

    public string BannerUrl => throw new NotImplementedException();

    public string VanityURLCode => throw new NotImplementedException();

    public SystemChannelMessageDeny SystemChannelFlags => throw new NotImplementedException();

    public string Description => throw new NotImplementedException();

    public int PremiumSubscriptionCount => throw new NotImplementedException();
    public int? MaxPresences { get; }
    public int? MaxMembers { get; }
    public int? MaxVideoChannelUsers { get; }
    public int? ApproximateMemberCount { get; }
    public int? ApproximatePresenceCount { get; }
    public int MaxBitrate { get; }

    public string PreferredLocale => throw new NotImplementedException();
    public NsfwLevel NsfwLevel { get; }

    public CultureInfo PreferredCulture => throw new NotImplementedException();
    public bool IsBoostProgressBarEnabled { get; }
    public ulong MaxUploadLimit { get; }

    public DateTimeOffset CreatedAt => throw new NotImplementedException();

    public Task<IInviteMetadata> CreateInviteToApplicationAsync(
        DefaultApplications application,
        int? maxAge,
        int? maxUses = null,
        bool isTemporary = false,
        bool isUnique = false,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IInviteMetadata> CreateInviteToStreamAsync(
        IUser user,
        int? maxAge,
        int? maxUses = null,
        bool isTemporary = false,
        bool isUnique = false,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IThreadChannel> CreateThreadAsync(
        string name,
        ThreadType type = ThreadType.PublicThread,
        ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay,
        IMessage message = null,
        bool? invitable = null,
        int? slowmode = null,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IThreadChannel>> GetActiveThreadsAsync(RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public int Position => throw new NotImplementedException();
    public ChannelFlags Flags { get; }

    public Task<ICategoryChannel> GetCategoryAsync(
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task SyncPermissionsAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IInviteMetadata> CreateInviteAsync(
        int? maxAge,
        int? maxUses = null,
        bool isTemporary = false,
        bool isUnique = false,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IInviteMetadata> CreateInviteToApplicationAsync(
        ulong applicationId,
        int? maxAge,
        int? maxUses = null,
        bool isTemporary = false,
        bool isUnique = false,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public ulong? CategoryId => throw new NotImplementedException();

    public Task ModifyAsync(Action<GuildChannelProperties> func, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public OverwritePermissions? GetPermissionOverwrite(IRole role)
    {
        throw new NotImplementedException();
    }

    public OverwritePermissions? GetPermissionOverwrite(IUser user)
    {
        throw new NotImplementedException();
    }

    public Task RemovePermissionOverwriteAsync(IRole role, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task RemovePermissionOverwriteAsync(IUser user, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task AddPermissionOverwriteAsync(
        IRole role,
        OverwritePermissions permissions,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task AddPermissionOverwriteAsync(
        IUser user,
        OverwritePermissions permissions,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(
        CacheMode mode,
        RequestOptions? options
    )
    {
        throw new NotImplementedException();
    }

    async Task<IUser?> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options)
    {
        return await GetUserAsync(id, mode, options);
    }

    public IGuild Guild => this;
    public ulong GuildId => Id;

    public IReadOnlyCollection<Overwrite> PermissionOverwrites => throw new NotImplementedException();

    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions? options)
    {
        return new[] { GetUsers() }.ToAsyncEnumerable();
    }

    public Task DeleteMessagesAsync(IEnumerable<IMessage> messages, RequestOptions? options = null)
    {
        return DeleteMessagesAsync(messages.Select(message => message.Id), options);
    }

    public async Task DeleteMessagesAsync(IEnumerable<ulong> messageIds, RequestOptions? options = null)
    {
        foreach (var messageId in messageIds) await DeleteMessageAsync(messageId);
    }

    public Task ModifyAsync(Action<TextChannelProperties> func, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IWebhook> CreateWebhookAsync(string name, Stream? avatar = null, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public bool IsNsfw => false;

    public string Topic => throw new NotImplementedException();

    public int SlowModeInterval => throw new NotImplementedException();
    public ThreadArchiveDuration DefaultArchiveDuration { get; }

    public async Task<IUserMessage> SendMessageAsync(
        string text = null,
        bool isTTS = false,
        Embed embed = null,
        RequestOptions options = null,
        AllowedMentions allowedMentions = null,
        MessageReference messageReference = null,
        MessageComponent components = null,
        ISticker[] stickers = null,
        Embed[] embeds = null,
        MessageFlags flags = MessageFlags.None
    )
    {
        if (string.IsNullOrEmpty(text))
        {
            if (embed == null) throw new Exception("either text or embed must be not null");
            text = ToText(embed);
        }

        var htmlText = TelegramMarkdownConverter.ConvertToHtml(text);
        var replyToMessageId = (int)(messageReference?.MessageId.Value ?? 0);
        var apiMessage = await _client.SendTextMessageAsync(
            _chat.Id,
            htmlText,
            ParseMode.Html,
            replyToMessageId: replyToMessageId
        );
        var userMessage = CacheMessage(apiMessage);
        return userMessage;
    }

    public Task<IUserMessage> SendFileAsync(
        string filePath,
        string text = null,
        bool isTTS = false,
        Embed embed = null,
        RequestOptions options = null,
        bool isSpoiler = false,
        AllowedMentions allowedMentions = null,
        MessageReference messageReference = null,
        MessageComponent components = null,
        ISticker[] stickers = null,
        Embed[] embeds = null,
        MessageFlags flags = MessageFlags.None
    )
    {
        throw new NotImplementedException();
    }

    public async Task<IUserMessage> SendFileAsync(
        Stream stream,
        string filename,
        string text = null,
        bool isTTS = false,
        Embed embed = null,
        RequestOptions options = null,
        bool isSpoiler = false,
        AllowedMentions allowedMentions = null,
        MessageReference messageReference = null,
        MessageComponent components = null,
        ISticker[] stickers = null,
        Embed[] embeds = null,
        MessageFlags flags = MessageFlags.None
    )
    {
        var htmlText = TelegramMarkdownConverter.ConvertToHtml(text);
        var videoExtensions = new[] { ".gif" };
        var replyToMessageId = (int)(messageReference?.MessageId.Value ?? 0);
        var apiMessage = videoExtensions.Any(filename.EndsWith)
            ? await _client.SendAnimationAsync(
                _chat.Id,
                new InputOnlineFile(stream, filename),
                caption: htmlText,
                replyToMessageId: replyToMessageId
            )
            : await _client.SendPhotoAsync(
                _chat.Id,
                new InputOnlineFile(stream),
                htmlText,
                replyToMessageId: replyToMessageId
            );
        var userMessage = CacheMessage(apiMessage);
        return userMessage;
    }

    public Task<IUserMessage> SendFileAsync(
        FileAttachment attachment,
        string text = null,
        bool isTTS = false,
        Embed embed = null,
        RequestOptions options = null,
        AllowedMentions allowedMentions = null,
        MessageReference messageReference = null,
        MessageComponent components = null,
        ISticker[] stickers = null,
        Embed[] embeds = null,
        MessageFlags flags = MessageFlags.None
    )
    {
        throw new NotImplementedException();
    }

    public Task<IUserMessage> SendFilesAsync(
        IEnumerable<FileAttachment> attachments,
        string text = null,
        bool isTTS = false,
        Embed embed = null,
        RequestOptions options = null,
        AllowedMentions allowedMentions = null,
        MessageReference messageReference = null,
        MessageComponent components = null,
        ISticker[] stickers = null,
        Embed[] embeds = null,
        MessageFlags flags = MessageFlags.None
    )
    {
        throw new NotImplementedException();
    }

    public Task<IMessage?> GetMessageAsync(
        ulong id,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        var message = _messageCache.FirstOrDefault(m => m.Id == id) as IMessage;
        return Task.FromResult(message);
    }

    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(
        int limit = 100,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        return GetMessagesAsync(0, Direction.After, limit, mode, options);
    }

    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(
        ulong fromMessageId,
        Direction dir,
        int limit = 100,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        // latest messages are first
        var selectedMessages = dir switch
        {
            Direction.After => _messageCache
                .SkipWhile(um => um.Id <= fromMessageId)
                .Take(limit),
            Direction.Before => _messageCache
                .Reverse()
                .SkipWhile(um => um.Id >= fromMessageId)
                .Take(limit),
            Direction.Around => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };
        return ToAsyncEnumerableReadOnlyCollection(selectedMessages);
    }

    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(
        IMessage fromMessage,
        Direction dir,
        int limit = 100,
        CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null
    )
    {
        return GetMessagesAsync(fromMessage.Id, dir, limit, mode, options);
    }

    public Task<IReadOnlyCollection<IMessage>> GetPinnedMessagesAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task DeleteMessageAsync(ulong messageId, RequestOptions? options = null)
    {
        return _client.DeleteMessageAsync(_chat.Id, (int)messageId, options?.CancelToken ?? default);
    }

    public Task DeleteMessageAsync(IMessage message, RequestOptions? options = null)
    {
        return DeleteMessageAsync(message.Id, options);
    }

    public Task<IUserMessage> ModifyMessageAsync(
        ulong messageId,
        Action<MessageProperties> func,
        RequestOptions options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task TriggerTypingAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public IDisposable EnterTypingState(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public string Mention => _chat.Title ?? _chat.Id.ToString();

    public IGuildUser CreateGuildUser(ChatMember chatMember)
    {
        var isAdmin = chatMember.Status is ChatMemberStatus.Administrator or ChatMemberStatus.Creator;
        var telegramGuildUser = new TelegramGuildUser(this, chatMember.User, isAdmin);
        _cachedUsers[telegramGuildUser.Id] = telegramGuildUser;
        return telegramGuildUser;
    }

    public IGuildUser CreateGuildUser(User user)
    {
        var isAdmin = _cachedUsers.TryGetValue((ulong)user.Id, out var existingUser) &&
            existingUser.GuildPermissions.Administrator;
        var telegramGuildUser = new TelegramGuildUser(this, user, isAdmin);
        _cachedUsers[telegramGuildUser.Id] = telegramGuildUser;
        return telegramGuildUser;
    }

    private async Task<IGuildUser?> GetTelegramGuildUserAsync(int id)
    {
        var chatMember = await _client.GetChatMemberAsync(_chat.Id, id);
        var currentUser = CreateGuildUser(chatMember);
        return currentUser;
    }

    private IReadOnlyCollection<IGuildUser> GetUsers()
    {
        return _cachedUsers.Values.Cast<IGuildUser>().ToList();
    }

    private static string ToText(IEmbed embed)
    {
        var builder = new StringBuilder();
        if (embed.Image != null) builder.AppendLine(embed.Image.Value.Url);
        if (embed.Author != null) builder.AppendLine($"**{embed.Author.Value.Name.TrimEnd('#')}**");
        if (embed.Title is { } title)
        {
            if (embed.Url is { } url) title = $"[{title}]({url})";
            builder.AppendLine(title);
        }

        builder.AppendLine(embed.Description);
        foreach (var embedField in embed.Fields)
        {
            builder.AppendLine($"**{embedField.Name}**");
            builder.AppendLine(embedField.Value);
            builder.AppendLine();
        }


        return builder.ToString();
    }

    private static IAsyncEnumerable<IReadOnlyCollection<T>> ToAsyncEnumerableReadOnlyCollection<T>(
        IEnumerable<T> values
    )
    {
        return new[] { new ReadOnlyCollection<T>(values.ToList()) }
            .ToAsyncEnumerable();
    }

    public IUserMessage CacheMessage(Message apiMessage)
    {
        var telegramGuildUser = CreateGuildUser(apiMessage.From);
        var userMessage = CreateMessage(apiMessage, this, telegramGuildUser);
        _messageCache.Enqueue(userMessage);
        return userMessage;
    }


    private IUserMessage CreateMessage(Message apiMessage, ITextChannel channel, IGuildUser author)
    {
        IEnumerable<IAttachment> GetAttachments()
        {
            if (apiMessage.Photo is { } photos)
                yield return new TelegramAttachment
                {
                    Filename = photos.Last().FileId
                };
        }

        var referencedMessage = apiMessage.ReplyToMessage is { MessageId: var messageId }
            ? _messageCache.FirstOrDefault(um => um.Id == (ulong)messageId)
            : null;

        var message = new TelegramUserMessage(
            channel,
            author,
            GetAttachments().ToList(),
            referencedMessage,
            apiMessage
        );
            return message;
    }
}