using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Beef.Telegram;

public class TelegramClientLauncher : IHostedService
{
    private readonly IOptions<TelegramOptions> _telegramOptions;
    private TelegramBotClient? _telegramBotClient;

    public TelegramClientLauncher(IOptions<TelegramOptions> telegramOptions)
    {
        _telegramOptions = telegramOptions;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _telegramBotClient = new TelegramBotClient(_telegramOptions.Value.Token);
        _telegramBotClient.StartReceiving(OnUpdateAsync, OnErrorAsync, cancellationToken: cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private Task OnErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task OnUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        if (update is { Type: UpdateType.Message, Message.Text: "ping" })
            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                "pong",
                replyToMessageId: update.Message.MessageId,
                cancellationToken: cancellationToken
            );
    }
}