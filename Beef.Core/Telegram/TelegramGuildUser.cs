﻿using Discord;
using Telegram.Bot.Types;
#pragma warning disable CS8625

namespace Beef.Core.Telegram;

public class TelegramGuildUser : TelegramUser, IGuildUser
{
    private readonly bool _isAdmin;
    private readonly User _user;

    public TelegramGuildUser(TelegramGuild guild, User user, bool isAdmin) : base(user)
    {
        _user = user;
        _isAdmin = isAdmin;
        Guild = guild;
    }

    public ChannelPermissions GetPermissions(IGuildChannel channel)
    {
        throw new NotImplementedException();
    }

    public string GetGuildAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
    {
        return GetAvatarUrl(format, size);
    }

    public string GetGuildBannerUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
    {
        throw new NotImplementedException();
    }

    public string GetDisplayAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
    {
        throw new NotImplementedException();
    }

    public Task KickAsync(string? reason = null, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task ModifyAsync(Action<GuildUserProperties> func, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task AddRoleAsync(ulong roleId, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task AddRoleAsync(IRole role, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task AddRolesAsync(IEnumerable<ulong> roleIds, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRoleAsync(ulong roleId, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRoleAsync(IRole role, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRolesAsync(IEnumerable<ulong> roleIds, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task SetTimeOutAsync(TimeSpan span, RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public Task RemoveTimeOutAsync(RequestOptions options = null)
    {
        throw new NotImplementedException();
    }

    public DateTimeOffset? JoinedAt => throw new NotImplementedException();
    public string DisplayName => throw new NotImplementedException();
    public string? Nickname => _user.FirstName;
    public string DisplayAvatarId => throw new NotImplementedException();
    public string GuildAvatarId => throw new NotImplementedException();
    public string GuildBannerHash { get; }

    public GuildPermissions GuildPermissions => _isAdmin
        ? GuildPermissions.All
        : new GuildPermissions(sendMessages: true);

    public IGuild Guild { get; }
    public ulong GuildId => Guild.Id;

    public DateTimeOffset? PremiumSince => throw new NotImplementedException();

    public IReadOnlyCollection<ulong> RoleIds => Array.Empty<ulong>();
    public bool? IsPending => throw new NotImplementedException();
    public int Hierarchy => throw new NotImplementedException();
    public DateTimeOffset? TimedOutUntil { get; }
    public GuildUserFlags Flags { get; }

    public bool IsDeafened => throw new NotImplementedException();

    public bool IsMuted => throw new NotImplementedException();

    public bool IsSelfDeafened => throw new NotImplementedException();

    public bool IsSelfMuted => throw new NotImplementedException();

    public bool IsSuppressed => throw new NotImplementedException();

    public IVoiceChannel VoiceChannel => throw new NotImplementedException();

    public string VoiceSessionId => throw new NotImplementedException();

    public bool IsStreaming => throw new NotImplementedException();
    public bool IsVideoing { get; }
    public DateTimeOffset? RequestToSpeakTimestamp => throw new NotImplementedException();
}