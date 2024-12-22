using Beef.Core.Interactions;
using Discord;
using Microsoft.Extensions.Hosting;

namespace Beef.Core.Telegram;

public class TelegramClientLauncher(
    TelegramChatClient telegramChatClient,
    IInteractionHandler interactionHandler)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        telegramChatClient.MessageReceived += OnMessageReceivedAsync;
        await telegramChatClient.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await telegramChatClient.StopAsync();
    }

    private Task OnMessageReceivedAsync(IUserMessage message)
    {
        interactionHandler.HandleMessage(telegramChatClient, message);
        return Task.CompletedTask;
    }
}