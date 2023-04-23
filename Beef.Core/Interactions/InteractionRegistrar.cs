using System.Reflection;
using Beef.Core.Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Beef.Core.Interactions;

public class InteractionRegistrar : IHostedService
{
    private readonly DiscordSocketClient _discordClient;

    private readonly InteractionService _interactionService;
    private readonly ILogger<DiscordClientLauncher> _logger;
    private readonly IServiceProvider _serviceProvider;

    public InteractionRegistrar(
        DiscordSocketClient discordClient,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        ILogger<DiscordClientLauncher> logger
    )
    {
        _discordClient = discordClient;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        await _interactionService.AddModulesAsync(GetType().Assembly, _serviceProvider);

        _discordClient.GuildAvailable += RegisterCommandsToGuildAsync;
        _discordClient.JoinedGuild += RegisterCommandsToGuildAsync;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task RegisterCommandsToGuildAsync(SocketGuild guild)
    {
        try
        {
            await _interactionService.RegisterCommandsToGuildAsync(guild.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while registering commands against a guild.");
        }
    }
}