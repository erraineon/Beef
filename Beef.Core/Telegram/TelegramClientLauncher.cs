using Beef.Core.Interactions;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Beef.Core.Telegram;

public class TelegramClientLauncher : IHostedService
{
    private readonly IInteractionFactory _interactionFactory;
    private readonly IInteractionHandler _interactionHandler;
    private readonly ILogger<TelegramClientLauncher> _logger;

    private readonly TelegramChatClient _telegramChatClient;

    public TelegramClientLauncher(
        TelegramChatClient telegramChatClient,
        IInteractionFactory interactionFactory,
        ILogger<TelegramClientLauncher> logger,
        IInteractionHandler interactionHandler
    )
    {
        _telegramChatClient = telegramChatClient;
        _interactionFactory = interactionFactory;
        _logger = logger;
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
        try
        {
            const string commandPrefix = ".";
            if (message.Content.StartsWith(commandPrefix) && message.Content.Length > 1 && !message.Author.IsBot)
            {
                var interaction = _interactionFactory.CreateInteraction(
                    message.Author,
                    message.Channel,
                    message.Content.Substring(commandPrefix.Length)
                );
                var interactionContext = new InteractionContext(_telegramChatClient, interaction, message.Channel);
                _interactionHandler.HandleInteractionContext(interactionContext);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while handling a telegram message.");
        }
        return Task.CompletedTask;
    }
}