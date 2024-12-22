using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Beef.TadmorMind;

public class TadmorMindThoughtProducer : BackgroundService
{
    private readonly ITadmorMindThoughtsRepository _tadmorMindThoughtsRepository;
    private readonly ITadmorMindClient _tadmorMindClient;
    private readonly ILogger<TadmorMindThoughtProducer> _logger;

    public TadmorMindThoughtProducer(
        ITadmorMindThoughtsRepository tadmorMindThoughtsRepository,
        ITadmorMindClient tadmorMindClient,
        ILogger<TadmorMindThoughtProducer> logger)
    {
        _tadmorMindThoughtsRepository = tadmorMindThoughtsRepository;
        _tadmorMindClient = tadmorMindClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (await _tadmorMindThoughtsRepository.GetCountAsync() < 64)
            {
                try
                {
                    _logger.LogInformation("Generating thoughts...");
                    var entries = await _tadmorMindClient.GenerateEntriesAsync();
                    await _tadmorMindThoughtsRepository.AddRangeAsync(entries);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while retrieving tadmor mind thoughts");
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