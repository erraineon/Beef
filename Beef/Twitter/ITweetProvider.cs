namespace Beef.Twitter;

public interface ITweetProvider
{
    Task<IList<Tweet>> GetTweetsAsync(string displayName, ulong? minimumStatusId = default);
}