namespace Beef.Core.Chats.Discord;

public class GuildSpecificCommandRegistrar : ICommandRegistrar
{
    private readonly IDiscordOptions _discordOptions;
    private readonly IDiscordInteractionService _discordInteractionService;

    public GuildSpecificCommandRegistrar(IDiscordOptions discordOptions, IDiscordInteractionService discordInteractionService)
    {
        _discordOptions = discordOptions;
        _discordInteractionService = discordInteractionService;
    }

    public async Task RegisterCommandsAsync(CancellationToken cancellationToken)
    {
        foreach (var testEnvironmentGuildId in _discordOptions.TestGuildIds)
        {
            await _discordInteractionService.RegisterCommandsToGuildAsync(testEnvironmentGuildId);
        }
    }
}