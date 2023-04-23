using Discord;
using Telegram.Bot.Types;

namespace Beef.Core.Telegram;

public class TelegramUserMessage : IUserMessage
{
    public TelegramUserMessage(
        IMessageChannel channel,
        IUser author,
        IReadOnlyCollection<IAttachment> attachments,
        IUserMessage? referencedMessage,
        Message apiMessage
    )
    {
        Channel = channel;
        Author = author;
        Attachments = attachments;
        ReferencedMessage = referencedMessage;
        ApiMessage = apiMessage;
    }

    public Message ApiMessage { get; init; }

    public ulong Id => (ulong)ApiMessage.MessageId;
    public DateTimeOffset CreatedAt => ApiMessage.Date;

    public Task DeleteAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public MessageType Type => MessageType.Default;
    public MessageSource Source => ApiMessage.From.IsBot ? MessageSource.Bot : MessageSource.User;
    public bool IsTTS => false;
    public bool IsPinned => throw new NotImplementedException();
    public bool IsSuppressed => throw new NotImplementedException();
    public bool MentionedEveryone => throw new NotImplementedException();
    public string Content => ApiMessage.Text ?? ApiMessage.Caption ?? string.Empty;
    public string CleanContent => Content;
    public DateTimeOffset Timestamp => ApiMessage.Date;
    public DateTimeOffset? EditedTimestamp => ApiMessage.EditDate;
    public IMessageChannel Channel { get; init; }
    public IUser Author { get; init; }
    public IThreadChannel Thread => throw new NotImplementedException();
    public IReadOnlyCollection<IAttachment> Attachments { get; init; }
    public IReadOnlyCollection<IEmbed> Embeds => new IEmbed[0];
    public IReadOnlyCollection<ITag> Tags => new ITag[0];
    public IReadOnlyCollection<ulong> MentionedChannelIds => new ulong[0];
    public IReadOnlyCollection<ulong> MentionedRoleIds => new ulong[0];
    public IReadOnlyCollection<ulong> MentionedUserIds => new ulong[0];
    public MessageActivity Activity => throw new NotImplementedException();
    public MessageApplication Application => throw new NotImplementedException();
    public MessageReference Reference => throw new NotImplementedException();
    public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => throw new NotImplementedException();
    public IReadOnlyCollection<IMessageComponent> Components => throw new NotImplementedException();
    IReadOnlyCollection<IStickerItem> IMessage.Stickers => throw new NotImplementedException();
    public MessageFlags? Flags => throw new NotImplementedException();
    public IMessageInteraction Interaction => throw new NotImplementedException();
    public MessageRoleSubscriptionData RoleSubscriptionData => throw new NotImplementedException();

    public Task ModifyAsync(Action<MessageProperties> func, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task PinAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task UnpinAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task CrosspostAsync(RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task AddReactionAsync(IEmote emote, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAllReactionsAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAllReactionsForEmoteAsync(IEmote emote, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(
        IEmote emoji,
        int limit,
        RequestOptions? options = null
    )
    {
        throw new NotImplementedException();
    }

    public string Resolve(
        TagHandling userHandling = TagHandling.Name,
        TagHandling channelHandling = TagHandling.Name,
        TagHandling roleHandling = TagHandling.Name,
        TagHandling everyoneHandling = TagHandling.Ignore,
        TagHandling emojiHandling = TagHandling.Name
    )
    {
        return Content;
    }

    public IUserMessage? ReferencedMessage { get; init; }
}