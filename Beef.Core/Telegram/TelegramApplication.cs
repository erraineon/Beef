using Discord;
using Telegram.Bot.Types;

namespace Beef.Core.Telegram;

public class TelegramApplication : IApplication
{
    public TelegramApplication(int ownerId)
    {
        Owner = new TelegramUser(new User { Id = ownerId });
    }

    public string Name => throw new NotImplementedException();
    public string Description => throw new NotImplementedException();
    public IReadOnlyCollection<string> RPCOrigins => throw new NotImplementedException();
    public ApplicationFlags Flags => throw new NotImplementedException();
    public ApplicationInstallParams InstallParams => throw new NotImplementedException();
    public IReadOnlyCollection<string> Tags => throw new NotImplementedException();
    public string IconUrl => throw new NotImplementedException();
    public bool IsBotPublic => throw new NotImplementedException();
    public bool BotRequiresCodeGrant => throw new NotImplementedException();
    public ITeam Team => throw new NotImplementedException();
    public IUser Owner { get; }
    public string TermsOfService => throw new NotImplementedException();
    public string PrivacyPolicy => throw new NotImplementedException();
    public string CustomInstallUrl => throw new NotImplementedException();
    public string RoleConnectionsVerificationUrl => throw new NotImplementedException();
    public string VerifyKey => throw new NotImplementedException();
    public ulong Id => throw new NotImplementedException();
    public DateTimeOffset CreatedAt => throw new NotImplementedException();
}