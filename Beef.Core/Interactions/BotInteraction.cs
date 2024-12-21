﻿using Discord;

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

    public Task<IUserMessage> FollowupAsync(
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
        return _textChannel.SendMessageAsync(
            text,
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
        throw new NotImplementedException();
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
    ulong IEntity<ulong>.Id => throw new NotImplementedException();
    public DateTimeOffset CreatedAt { get; }
}