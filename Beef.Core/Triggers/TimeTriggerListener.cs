using Beef.Core.Chats;
using Beef.Core.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Beef.Core.Triggers;

public class TimeTriggerListener : BackgroundService
{
    private readonly IChatServiceScopeFactory _chatServiceScopeFactory;
    private readonly ICurrentTimeProvider _currentTimeProvider;
    private readonly IBeefDbContext _dbContext;
    private readonly ITimeTriggerFactory _timeTriggerFactory;

    public TimeTriggerListener(
        IBeefDbContext dbContext,
        ICurrentTimeProvider currentTimeProvider,
        ITimeTriggerFactory timeTriggerFactory,
        IChatServiceScopeFactory chatServiceScopeFactory
    )
    {
        _dbContext = dbContext;
        _currentTimeProvider = currentTimeProvider;
        _timeTriggerFactory = timeTriggerFactory;
        _chatServiceScopeFactory = chatServiceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessTimeTriggersAsync(stoppingToken);

            var minimumTimeGranularity = TimeSpan.FromSeconds(5);
            await Task.Delay(minimumTimeGranularity, stoppingToken);
        }
    }

    private async Task ProcessTimeTriggersAsync(CancellationToken stoppingToken)
    {
        var now = _currentTimeProvider.Now;
        var guilds = _dbContext.Guilds.ToAsyncEnumerable();

        await foreach (var guild in guilds.WithCancellation(stoppingToken))
        {
            var updatedTriggers = await Task.WhenAll(
                guild.Value.Triggers
                    .OfType<TimeTrigger>()
                    .Where(t => t.FireAt <= now)
                    .Select(ExecuteTimeTriggerAsync)
            );
            guild.Value.Triggers.Clear();
            guild.Value.Triggers.AddRange(updatedTriggers.Where(t => t != default)!);
        }

        await _dbContext.SaveChangesAsync(stoppingToken);
    }

    private async Task<TimeTrigger?> ExecuteTimeTriggerAsync(TimeTrigger trigger)
    {
        var triggerScope = _chatServiceScopeFactory.CreateScope(trigger.Context.ChatType);
        var triggerExecutor = triggerScope.ServiceProvider.GetRequiredService<ITriggerExecutor>();
        await triggerExecutor.ExecuteAsync(trigger);
        var newTrigger = _timeTriggerFactory.Advance(trigger);
        return newTrigger;
    }
}