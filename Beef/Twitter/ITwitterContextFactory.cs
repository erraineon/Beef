using LinqToTwitter;

namespace Beef.Twitter;

public interface ITwitterContextFactory
{
    Task<TwitterContext> GetOrCreateAsync();
}