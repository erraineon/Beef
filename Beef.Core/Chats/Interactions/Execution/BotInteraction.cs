using Discord;

namespace Beef.Core.Chats.Interactions.Execution;

public class BotInteraction : ISlashCommandInteraction
{
    private readonly IMessageChannel _textChannel;

    public BotInteraction(IUser user, IMessageChannel textChannel, IApplicationCommandInteractionData data)
    {
        _textChannel = textChannel;
        User = user;
        Data = data;
    }

    public Task RespondAsync(
        string text = null,
        Embed[] embeds = null,
        bool isTTS = false,
        bool ephemeral = false,
        AllowedMentions allowedMentions = null,
        MessageComponent components = null,
        Embed embed = null,
        RequestOptions options = null
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
        RequestOptions options = null
    )
    {
        return FollowupWithFilesAsync(
            attachments,
            text,
            embeds,
            isTTS,
            ephemeral,
            allowedMentions,
            components,
            embed,
            options
        );
    }

    public Task<IUserMessage> FollowupAsync(
        string text = null,
        Embed[] embeds = null,
        bool isTTS = false,
        bool ephemeral = false,
        AllowedMentions allowedMentions = null,
        MessageComponent components = null,
        Embed embed = null,
        RequestOptions options = null
    )
    {
        return _textChannel.SendMessageAsync(
            text,
            isTTS,
            embed,
            options,
            allowedMentions,
            null,
            components,
            null,
            embeds
        );
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
        RequestOptions options = null
    )
    {
        return _textChannel.SendFilesAsync(
            attachments,
            text,
            isTTS,
            embed,
            options,
            allowedMentions,
            null,
            components,
            null,
            embeds
        );
    }

    public Task<IUserMessage> GetOriginalResponseAsync(RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IUserMessage> ModifyOriginalResponseAsync(Action<MessageProperties> func, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task DeleteOriginalResponseAsync(RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task DeferAsync(bool ephemeral = false, RequestOptions options = null)
    {
        return Task.CompletedTask;
    }

    ulong IDiscordInteraction.Id => throw new NotImplementedException();

    public InteractionType Type => InteractionType.ApplicationCommand;
    IDiscordInteractionData IDiscordInteraction.Data => Data;

    public IApplicationCommandInteractionData Data { get; }

    public string Token => throw new NotImplementedException();
    public int Version => 0;
    public bool HasResponded => false;
    public IUser User { get; }

    ulong IEntity<ulong>.Id => throw new NotImplementedException();

    public DateTimeOffset CreatedAt => throw new NotImplementedException();
}