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
    bool? IApplication.IsBotPublic => IsBotPublic;

    bool? IApplication.BotRequiresCodeGrant => BotRequiresCodeGrant;

    public bool IsBotPublic => throw new NotImplementedException();
    public bool BotRequiresCodeGrant => throw new NotImplementedException();
    public ITeam Team => throw new NotImplementedException();
    public IUser Owner { get; }
    public string TermsOfService => throw new NotImplementedException();
    public string PrivacyPolicy => throw new NotImplementedException();
    public string CustomInstallUrl => throw new NotImplementedException();
    public string RoleConnectionsVerificationUrl => throw new NotImplementedException();
    public string VerifyKey => throw new NotImplementedException();
    public PartialGuild Guild { get; }
    public IReadOnlyCollection<string> RedirectUris { get; }
    public string InteractionsEndpointUrl { get; }
    public int? ApproximateGuildCount { get; }
    public ApplicationDiscoverabilityState DiscoverabilityState { get; }
    public DiscoveryEligibilityFlags DiscoveryEligibilityFlags { get; }
    public ApplicationExplicitContentFilterLevel ExplicitContentFilterLevel { get; }
    public bool IsHook { get; }
    public IReadOnlyCollection<string> InteractionEventTypes { get; }
    public ApplicationInteractionsVersion InteractionsVersion { get; }
    public bool IsMonetized { get; }
    public ApplicationMonetizationEligibilityFlags MonetizationEligibilityFlags { get; }
    public ApplicationMonetizationState MonetizationState { get; }
    public ApplicationRpcState RpcState { get; }
    public ApplicationStoreState StoreState { get; }
    public ApplicationVerificationState VerificationState { get; }
    public IReadOnlyDictionary<ApplicationIntegrationType, ApplicationInstallParams> IntegrationTypesConfig { get; }
    public ulong Id => throw new NotImplementedException();
    public DateTimeOffset CreatedAt => throw new NotImplementedException();
}