using Beef.Core.Interactions;
using Discord;
using Microsoft.Extensions.Hosting;

namespace Beef.Core.Telegram;

public class TelegramClientLauncher : IHostedService
{
    private readonly TelegramChatClient _telegramChatClient;
    private readonly IInteractionHandler _interactionHandler;

    public TelegramClientLauncher(
        TelegramChatClient telegramChatClient,
        IInteractionHandler interactionHandler
    )
    {
        _telegramChatClient = telegramChatClient;
        _interactionHandler = interactionHandler;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _telegramChatClient.MessageReceived += OnMessageReceivedAsync;
        await _telegramChatClient.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _telegramChatClient.StopAsync();
    }

    private Task OnMessageReceivedAsync(IUserMessage message)
    {
        _interactionHandler.HandleMessage(_telegramChatClient, message);
        return Task.CompletedTask;
    }
}