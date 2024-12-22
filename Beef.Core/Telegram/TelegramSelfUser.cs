using Discord;
using Telegram.Bot.Types;

namespace Beef.Core.Telegram;

public class TelegramSelfUser(long botId) : TelegramUser(new User { Id = botId }), ISelfUser
{
    public Task ModifyAsync(Action<SelfUserProperties> func, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public string Email => throw new NotImplementedException();
    public bool IsVerified => throw new NotImplementedException();
    public bool IsMfaEnabled => throw new NotImplementedException();
    public UserProperties Flags => throw new NotImplementedException();
    public PremiumType PremiumType => throw new NotImplementedException();
    public string Locale => throw new NotImplementedException();
}