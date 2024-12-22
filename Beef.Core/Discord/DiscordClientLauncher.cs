using Beef.Core.Interactions;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Beef.Core.Discord;

public class DiscordClientLauncher(
    IOptions<DiscordOptions> discordOptions,
    DiscordSocketClient discordClient,
    IInteractionHandler interactionHandler)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        discordClient.InteractionCreated += OnInteractionCreatedAsync;

        var discordReady = new TaskCompletionSource();

        Task OnReady()
        {
            discordClient.Ready -= OnReady;
            discordReady.SetResult();
            return Task.CompletedTask;
        }

        discordClient.Ready += OnReady;
        discordClient.MessageReceived += OnMessageReceivedAsync;

        await discordClient.LoginAsync(TokenType.Bot, discordOptions.Value.Token);
        await discordClient.StartAsync();
        await discordReady.Task;
    }

    private Task OnMessageReceivedAsync(SocketMessage message)
    {
        if (message is IUserMessage { Channel: IGuildChannel } userMessage)
            interactionHandler.HandleMessage(discordClient, userMessage);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await discordClient.StopAsync();
    }

    private Task OnInteractionCreatedAsync(SocketInteraction interaction)
    {
        var context = new SocketInteractionContext(
            discordClient,
            interaction
        );
        interactionHandler.HandleInteractionContext(context);
        return Task.CompletedTask;
    }
}