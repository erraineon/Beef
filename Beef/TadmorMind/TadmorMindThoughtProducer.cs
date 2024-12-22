using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Beef.TadmorMind;

public class TadmorMindThoughtProducer(
    ITadmorMindThoughtsRepository tadmorMindThoughtsRepository,
    ITadmorMindClient tadmorMindClient,
    ILogger<TadmorMindThoughtProducer> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (await tadmorMindThoughtsRepository.GetCountAsync() < 64)
            {
                try
                {
                    logger.LogInformation("Generating thoughts...");
                    var entries = await tadmorMindClient.GenerateEntriesAsync();
                    await tadmorMindThoughtsRepository.AddRangeAsync(entries);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error while retrieving tadmor mind thoughts");
                    await GetDelayTask(stoppingToken);
                }
            }
            else
            {
                await GetDelayTask(stoppingToken);
            }
        }
    }

    private static Task GetDelayTask(CancellationToken stoppingToken)
    {
        return Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
    }
}