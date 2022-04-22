using Microsoft.Extensions.Caching.Memory;

namespace Beef.Twitter;

public class CachedTweetProviderDecorator : ITweetProvider
{
    private readonly IMemoryCache _memoryCache;
    private readonly ITweetProvider _tweetProvider;

    public CachedTweetProviderDecorator(IMemoryCache memoryCache, ITweetProvider tweetProvider)
    {
        _memoryCache = memoryCache;
        _tweetProvider = tweetProvider;
    }

    public async Task<IList<Tweet>> GetTweetsAsync(string displayName, ulong? minimumStatusId = default)
    {
        var key = $"tweets-{displayName}";
        var tweets = _memoryCache.GetOrCreate(
            key,
            e =>
            {
                e.SlidingExpiration = TimeSpan.FromDays(7);
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30);
                return new List<Tweet>();
            }
        );
        var latestStatusId = tweets.DefaultIfEmpty().Max(t => t?.Id);
        var newTweets = await _tweetProvider.GetTweetsAsync(displayName, latestStatusId);
        tweets.AddRange(newTweets);
        return tweets;
    }
}