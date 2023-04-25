using E621;
using Microsoft.Extensions.Caching.Distributed;

namespace Beef.Furry;

public class E621SearchEngine : IE621SearchEngine
{
    private readonly IDistributedCache _distributedCache;
    private readonly IE621Client _e621Client;

    public E621SearchEngine(IDistributedCache distributedCache, IE621Client e621Client)
    {
        _distributedCache = distributedCache;
        _e621Client = e621Client;
    }

    public async Task<IList<E621Post>> SearchLatestAsync(string contextKey, string tags)
    {
        var key = $"e621-{contextKey}-{tags}";
        var lastSeenId = long.TryParse(await _distributedCache.GetStringAsync(key), out var x) ? x : default(long?);
        var searchResults = await SearchLatestAsync(tags, lastSeenId);
        var posts = (lastSeenId != default ? searchResults.Reverse().Take(8) : searchResults.Take(1))
            .ToList();
        if (posts.LastOrDefault() is { } latestPost)
            await _distributedCache.SetStringAsync(key, latestPost.Id.ToString());

        return posts;
    }

    private async Task<IList<E621Post>> SearchLatestAsync(string tags, long? afterId)
    {
        var options = new E621SearchOptions
        {
            Tags = tags,
            AfterId = afterId
        };
        var posts = await _e621Client.Search(options);
        return posts;
    }
}