using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Caching.Distributed;

namespace Beef.TadmorMind;

public class TadmorMindThoughtsRepository(IDistributedCache distributedCache) : ITadmorMindThoughtsRepository
{
    private readonly SemaphoreSlim _queueOperationsSemaphore = new(1, 1);
    private TaskCompletionSource _hasAnyTaskCompletionSource = new();

    public string CacheKey => "tadmormind-buffer";

    public async Task<string> ReceiveAsync()
    {
        await _queueOperationsSemaphore.WaitAsync();
        if (await GetCountAsync() > 0) _hasAnyTaskCompletionSource.TrySetResult();
        await _hasAnyTaskCompletionSource.Task;
        var buffer = await GetBufferAsync();
        var entry = buffer.Dequeue();
        await SetBufferAsync(buffer);
        if (!buffer.Any()) _hasAnyTaskCompletionSource = new TaskCompletionSource();
        _queueOperationsSemaphore.Release();
        return entry;
    }

    public async Task<int> GetCountAsync()
    {
        return (await GetBufferAsync()).Count;
    }

    public async Task AddRangeAsync(List<string> entries)
    {
        var buffer = await GetBufferAsync();
        foreach (var entry in entries) buffer.Enqueue(entry);
        await SetBufferAsync(buffer);
        if (entries.Any()) _hasAnyTaskCompletionSource.TrySetResult();
    }

    private async Task SetBufferAsync(Queue<string> buffer)
    {
        await distributedCache.SetStringAsync(CacheKey, JsonSerializer.Serialize(buffer));
    }

    private async Task<Queue<string>> GetBufferAsync()
    {
        var listJson = await distributedCache.GetStringAsync(CacheKey);
        var buffer = !string.IsNullOrWhiteSpace(listJson) &&
                     JsonNode.Parse(listJson)?.AsArray().GetValues<string>().ToList() is { Count: > 0 } x
            ? x
            : [];
        return new Queue<string>(buffer);
    }
}