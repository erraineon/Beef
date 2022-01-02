using Beef.Core.Chats.Interactions.Registration;

namespace Beef.Discord;

public class DiscordGuildsCommandRegistrationService : ICommandRegistrationService
{
    private readonly IDiscordChatClient _discordChatClient;
    private readonly IDiscordCommandRegistrar _discordCommandRegistrar;

    public DiscordGuildsCommandRegistrationService(
        IDiscordChatClient discordChatClient,
        IDiscordCommandRegistrar discordCommandRegistrar
    )
    {
        _discordChatClient = discordChatClient;
        _discordCommandRegistrar = discordCommandRegistrar;
    }

    public async Task RegisterCommandsAsync()
    {
        foreach (var guild in await _discordChatClient.GetGuildsAsync())
            await _discordCommandRegistrar.RegisterCommandsToGuildAsync(guild.Id);
    }
}