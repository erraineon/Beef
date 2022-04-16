using Beef.Core.Chats.Interactions.Registration;

namespace Beef.Discord;

public class DiscordGlobalCommandRegistrationService : ICommandRegistrationService
{
    private readonly IDiscordCommandRegistrar _discordCommandRegistrar;

    public DiscordGlobalCommandRegistrationService(
        IDiscordCommandRegistrar discordCommandRegistrar
    )
    {
        _discordCommandRegistrar = discordCommandRegistrar;
    }

    public async Task RegisterCommandsAsync()
    {
        await _discordCommandRegistrar.RegisterCommandsGloballyAsync();
    }
}