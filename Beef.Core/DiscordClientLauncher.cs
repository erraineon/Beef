using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Beef.Core;

public class DiscordClientLauncher : IHostedService
{
    private readonly DiscordSocketClient _discordClient;
    private readonly IOptions<DiscordOptions> _discordOptions;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IInteractionHandler _interactionHandler;

    public DiscordClientLauncher(
        IOptions<DiscordOptions> discordOptions,
        DiscordSocketClient discordClient,
        IServiceProvider serviceProvider,
        InteractionService interactionService,
        IInteractionHandler interactionHandler
    )
    {
        _discordOptions = discordOptions;
        _discordClient = discordClient;
        _serviceProvider = serviceProvider;
        _interactionService = interactionService;
        _interactionHandler = interactionHandler;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _discordClient.InteractionCreated += OnInteractionCreatedAsync;

        await LoginAsync();
        await RegisterInteractionServiceAsync();
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

    private async Task LoginAsync()
    {
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

    private async Task RegisterInteractionServiceAsync()
    {
        var modules = await _interactionService.AddModulesAsync(GetType().Assembly, _serviceProvider);
        var testGuild = _discordClient.GetGuild(389466295063674881);
        await _interactionService.AddModulesToGuildAsync(
            testGuild,
            true,
            modules.ToArray()
        );
    }
}