using Beef.Core.Telegram;
using LinqToTwitter;
using Microsoft.Extensions.Caching.Memory;

namespace Beef.Twitter;

public class CachedTwitterContextFactoryDecorator : ITwitterContextFactory
{
    private readonly IMemoryCache _memoryCache;
    private readonly ITwitterContextFactory _twitterContextFactory;

    public CachedTwitterContextFactoryDecorator(IMemoryCache memoryCache, ITwitterContextFactory twitterContextFactory)
    {
        _memoryCache = memoryCache;
        _twitterContextFactory = twitterContextFactory;
    }

    public Task<TwitterContext> CreateAsync()
    {
        var key = "twittercontext";
        return _memoryCache.GetOrCreateAsyncLock(
            key,
            e =>
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30);
                return _twitterContextFactory.CreateAsync();
            }
        );
    }
}