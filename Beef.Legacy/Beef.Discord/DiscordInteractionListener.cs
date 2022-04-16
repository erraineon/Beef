using Beef.Core.Chats.Interactions.Execution;
using Discord.Interactions;
using Discord.WebSocket;

namespace Beef.Discord;

public class DiscordInteractionListener : IInteractionListener
{
    private readonly IDiscordChatClient _discordChatClient;
    private readonly IInteractionHandler _interactionHandler;

    public DiscordInteractionListener(
        IDiscordChatClient discordChatClient,
        IInteractionHandler interactionHandler)
    {
        _discordChatClient = discordChatClient;
        _interactionHandler = interactionHandler;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _discordChatClient.InteractionCreated += HandleInteractionAsync;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _discordChatClient.InteractionCreated -= HandleInteractionAsync;
        return Task.CompletedTask;
    }

    private Task HandleInteractionAsync(SocketInteraction interaction)
    {
        // TODO: replace with BotInteractionContext when #2012 is fixed.
        var context = new SocketInteractionContext(
            (DiscordSocketClient) _discordChatClient,
            interaction
        );
        _interactionHandler.HandleInteractionNoAwait(context);
        return Task.CompletedTask;
    }
}