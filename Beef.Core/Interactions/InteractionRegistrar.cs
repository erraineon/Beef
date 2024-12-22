using System.Reflection;
using Beef.Core.Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Beef.Core.Interactions;

public class InteractionRegistrar(
    DiscordSocketClient discordClient,
    InteractionService interactionService,
    IServiceProvider serviceProvider,
    ILogger<DiscordClientLauncher> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
        await interactionService.AddModulesAsync(GetType().Assembly, serviceProvider);

        discordClient.GuildAvailable += RegisterCommandsToGuildAsync;
        discordClient.JoinedGuild += RegisterCommandsToGuildAsync;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task RegisterCommandsToGuildAsync(SocketGuild guild)
    {
        try
        {
            await interactionService.RegisterCommandsToGuildAsync(guild.Id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while registering commands against a guild.");
        }
    }
}