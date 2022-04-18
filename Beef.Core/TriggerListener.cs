using Beef.Core.Data;
using Beef.Core.Interactions;
using Beef.Core.Telegram;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Beef.Core;

public class TriggerListener : BackgroundService
{
    private readonly IDbContextFactory<BeefDbContext> _dbContextFactory;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly IInteractionFactory _interactionFactory;
    private readonly IInteractionHandler _interactionHandler;
    private readonly TelegramChatClient _telegramChatClient;
    private readonly ILogger<TriggerListener> _logger;

    public TriggerListener(
        IDbContextFactory<BeefDbContext> dbContextFactory,
        IInteractionFactory interactionFactory,
        IInteractionHandler interactionHandler,
        DiscordSocketClient discordSocketClient,
        TelegramChatClient telegramChatClient,
        ILogger<TriggerListener> logger
    )
    {
        _dbContextFactory = dbContextFactory;
        _interactionFactory = interactionFactory;
        _interactionHandler = interactionHandler;
        _discordSocketClient = discordSocketClient;
        _telegramChatClient = telegramChatClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunDueTriggersAsync();
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task RunDueTriggersAsync()
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var utcNow = DateTime.UtcNow;
            var dueTriggers = dbContext.Triggers
                .OfType<TimeTrigger>()
                .Where(x => x.TriggerAtUtc < utcNow)
                .ToAsyncEnumerable();

            await foreach (var trigger in dueTriggers)
            {
                await RunDueTriggerAsync(trigger);
                trigger.Advance(utcNow);
                if (trigger is OneTimeTrigger) dbContext.Triggers.Remove(trigger);
            }

            await dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while processing all the time triggers.");
        }
    }

    private async Task RunDueTriggerAsync(TimeTrigger trigger)
    {
        try
        {
            IDiscordClient chatClient = trigger.ChatType == ChatType.Discord
                ? _discordSocketClient
                : _telegramChatClient;
            var guild = await chatClient.GetGuildAsync(trigger.GuildId) ??
                throw new Exception($"Guild {trigger.GuildId} was not found.");
            var channel = await guild.GetTextChannelAsync(trigger.ChannelId) ??
                throw new Exception($"Channel {trigger.ChannelId} was not found.");
            var user = await guild.GetUserAsync(trigger.UserId) ??
                throw new Exception($"User {trigger.UserId} was not found.");

            var interaction = _interactionFactory.CreateInteraction(user, channel, trigger.CommandToRun);
            _interactionHandler.HandleInteractionContext(
                new InteractionContext(chatClient, interaction, channel)
            );
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "An error occurred while processing a time trigger.");
        }
    }
}