using Discord;

namespace Beef.Core.Interactions;

public class BotInteraction : ISlashCommandInteraction
{
    private readonly IMessageChannel _textChannel;

    public BotInteraction(IUser user, IMessageChannel textChannel, IApplicationCommandInteractionData data)
    {
        _textChannel = textChannel;
        User = user;
        Data = data;
        CreatedAt = DateTimeOffset.Now;
    }

    private static IEnumerable<string?> SplitLongText(string? contentToReplyWith)
    {
        // splits the string based off the last found line break, if any, to fit the character limit
        var index = 0;
        const int maxMessageLength = 2000;
        while (contentToReplyWith?.Length > maxMessageLength + index)
        {
            var lastLineBreakIndex = contentToReplyWith.LastIndexOf('\n', index + maxMessageLength);
            if (lastLineBreakIndex == -1) lastLineBreakIndex = maxMessageLength;
            yield return contentToReplyWith.Substring(index, lastLineBreakIndex);
            index += lastLineBreakIndex;
        }

        yield return contentToReplyWith?[index..];
    }

    public async Task<IUserMessage> FollowupAsync(
        string? text = null,
        Embed[]? embeds = null,
        bool isTts = false,
        bool ephemeral = false,
        AllowedMentions? allowedMentions = null,
        MessageComponent? components = null,
        Embed? embed = null,
        RequestOptions? options = null
    )
    {
        var textComponents = SplitLongText(text);
        IUserMessage? lastMessage = null;
        foreach (var textComponent in textComponents)
        {
            lastMessage = await _textChannel.SendMessageAsync(
                textComponent,
                isTts,
                embed,
                options,
                allowedMentions,
                null,
                components,
                null,
                embeds
            );
        }
        return lastMessage ?? throw new InvalidOperationException($"No messages were sent for text: {text}");
    }

    public Task RespondAsync(
        string text = null,
        Embed[] embeds = null,
        bool isTTS = false,
        bool ephemeral = false,
        AllowedMentions allowedMentions = null,
        MessageComponent components = null,
        Embed embed = null,
        RequestOptions options = null,
        PollProperties poll = null
    )
    {
        return FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);
    }

    public Task RespondWithFilesAsync(
        IEnumerable<FileAttachment> attachments,
        string text = null,
        Embed[] embeds = null,
        bool isTTS = false,
        bool ephemeral = false,
        AllowedMentions allowedMentions = null,
        MessageComponent components = null,
        Embed embed = null,
        RequestOptions options = null,
        PollProperties poll = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IUserMessage> FollowupAsync(
        string text = null,
        Embed[] embeds = null,
        bool isTTS = false,
        bool ephemeral = false,
        AllowedMentions allowedMentions = null,
        MessageComponent components = null,
        Embed embed = null,
        RequestOptions options = null,
        PollProperties poll = null
    )
    {
        return FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);
    }

    public Task<IUserMessage> FollowupWithFilesAsync(
        IEnumerable<FileAttachment> attachments,
        string text = null,
        Embed[] embeds = null,
        bool isTTS = false,
        bool ephemeral = false,
        AllowedMentions allowedMentions = null,
        MessageComponent components = null,
        Embed embed = null,
        RequestOptions options = null,
        PollProperties poll = null
    )
    {
        throw new NotImplementedException();
    }

    public Task<IUserMessage> GetOriginalResponseAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IUserMessage> ModifyOriginalResponseAsync(
        Action<MessageProperties> func,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public Task DeleteOriginalResponseAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task DeferAsync(bool ephemeral = false, RequestOptions? options = null)
    {
        return Task.CompletedTask;
    }

    public Task RespondWithModalAsync(Modal modal, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task RespondWithPremiumRequiredAsync(RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    ulong IDiscordInteraction.Id => throw new NotImplementedException();

    public InteractionType Type => InteractionType.ApplicationCommand;
    IDiscordInteractionData IDiscordInteraction.Data => Data;

    public IApplicationCommandInteractionData Data { get; }

    public string Token => throw new NotImplementedException();
    public int Version => 0;
    public bool HasResponded => false;
    public IUser User { get; }
    public string UserLocale => "en-US";
    public string GuildLocale => "en-US";
    public bool IsDMInteraction => false;
    public ulong? ChannelId => _textChannel.Id;
    public ulong? GuildId => (_textChannel as IGuildChannel)?.GuildId;
    public ulong ApplicationId => throw new NotImplementedException();
    public IReadOnlyCollection<IEntitlement> Entitlements { get; }
    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> IntegrationOwners { get; }
    public InteractionContextType? ContextType { get; }
    public GuildPermissions Permissions { get; }
    public ulong AttachmentSizeLimit { get; }
    ulong IEntity<ulong>.Id => throw new NotImplementedException();
    public DateTimeOffset CreatedAt { get; }
}