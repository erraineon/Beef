using Discord;
using Telegram.Bot.Types;

namespace Beef.Core.Telegram;

public class TelegramUser : IUser
{
    private readonly User _user;

    public TelegramUser(User user)
    {
        _user = user;
    }

    public IActivity Activity => throw new NotImplementedException();

    public virtual ulong Id => (ulong)_user.Id;
    public string Mention => $"@{Username}";
    public UserStatus Status => UserStatus.Online;
    public IReadOnlyCollection<ClientType> ActiveClients => throw new NotImplementedException();
    public IReadOnlyCollection<IActivity> Activities => throw new NotImplementedException();

    public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
    {
        throw new NotImplementedException();
    }

    public string GetDefaultAvatarUrl()
    {
        throw new NotImplementedException();
    }

    public Task<IDMChannel> CreateDMChannelAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public string AvatarId => throw new NotSupportedException();
    public string Discriminator => string.Empty;
    public ushort DiscriminatorValue => default;
    public bool IsBot => _user.IsBot;
    public bool IsWebhook => false;
    public string Username => _user.Username ?? string.Empty;
    public UserProperties? PublicFlags => throw new NotImplementedException();
    public DateTimeOffset CreatedAt => throw new NotImplementedException();
}