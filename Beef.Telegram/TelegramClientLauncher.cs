using Beef.Core;
using Discord.Interactions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Beef.Telegram;

public class TelegramClientLauncher : IHostedService
{
    private readonly IInteractionFactory _interactionFactory;
    private readonly IInteractionHandler _interactionHandler;
    private readonly ILogger<TelegramClientLauncher> _logger;

    private readonly TelegramChatClient _telegramChatClient;
    private readonly ITelegramGuildFactory _telegramGuildFactory;

    public TelegramClientLauncher(
        TelegramChatClient telegramChatClient,
        ITelegramGuildFactory telegramGuildFactory,
        IInteractionFactory interactionFactory,
        ILogger<TelegramClientLauncher> logger,
        IInteractionHandler interactionHandler
    )
    {
        _telegramChatClient = telegramChatClient;
        _telegramGuildFactory = telegramGuildFactory;
        _interactionFactory = interactionFactory;
        _logger = logger;
        _interactionHandler = interactionHandler;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _telegramChatClient.StartAsync();
        _telegramChatClient.Update += OnUpdateAsync;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _telegramChatClient.StopAsync();
    }

    private async Task OnUpdateAsync(Update update)
    {
        try
        {
            if (update.Message is { Chat.Type: not ChatType.Private } telegramMessage)
            {
                var telegramGuild = await _telegramGuildFactory.CreateAsync(telegramMessage.Chat);
                var message = telegramGuild.CacheMessage(telegramMessage);

                const string commandPrefix = ".";
                if (message.Content.StartsWith(commandPrefix) && message.Content.Length > 1 && !message.Author.IsBot)
                {
                    var interaction = _interactionFactory.CreateInteraction(
                        message.Author,
                        message.Channel,
                        message.Content.Substring(commandPrefix.Length)
                    );
                    var interactionContext = new InteractionContext(_telegramChatClient, interaction, telegramGuild);
                    _interactionHandler.HandleInteractionContext(interactionContext);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while handling a telegram update.");
        }
    }
}