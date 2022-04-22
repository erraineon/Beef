using Beef.Core.Interactions;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Beef.Core.Discord;

public class DiscordClientLauncher : IHostedService
{
    private readonly DiscordSocketClient _discordClient;
    private readonly IOptions<DiscordOptions> _discordOptions;
    private readonly IInteractionHandler _interactionHandler;

    public DiscordClientLauncher(
        IOptions<DiscordOptions> discordOptions,
        DiscordSocketClient discordClient,
        IInteractionHandler interactionHandler
    )
    {
        _discordOptions = discordOptions;
        _discordClient = discordClient;
        _interactionHandler = interactionHandler;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _discordClient.InteractionCreated += OnInteractionCreatedAsync;

        var discordReady = new TaskCompletionSource();

        Task OnReady()
        {
            _discordClient.Ready -= OnReady;
            discordReady.SetResult();
            return Task.CompletedTask;
        }

        _discordClient.Ready += OnReady;

        await _discordClient.LoginAsync(TokenType.Bot, _discordOptions.Value.Token);
        await _discordClient.StartAsync();
        await discordReady.Task;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _discordClient.StopAsync();
    }

    private Task OnInteractionCreatedAsync(SocketInteraction interaction)
    {
        var context = new SocketInteractionContext(
            _discordClient,
            interaction
        );
        _interactionHandler.HandleInteractionContext(context);
        return Task.CompletedTask;
    }

    
}