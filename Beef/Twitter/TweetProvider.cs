using LinqToTwitter;
using LinqToTwitter.Common;

namespace Beef.Twitter;

public class TweetProvider : ITweetProvider
{
    private readonly ITwitterContextFactory _twitterContextFactory;

    public TweetProvider(ITwitterContextFactory twitterContextFactory)
    {
        _twitterContextFactory = twitterContextFactory;
    }

    public async Task<IList<Tweet>> GetTweetsAsync(string displayName, ulong? minimumStatusId)
    {
        var twitterContext = await _twitterContextFactory.GetOrCreateAsync();
        ulong maxId = default;
        var statuses = new List<Status>();
        List<Status> buffer;
        do
        {
            var query = twitterContext.Status.Where(
                tweet =>
                    tweet.Type == StatusType.User &&
                    tweet.ScreenName == displayName &&
                    tweet.Count == 200 &&
                    tweet.TweetMode == TweetMode.Compat &&
                    tweet.IncludeRetweets == false &&
                    tweet.ExcludeReplies == true
            );
            // ReSharper disable once AccessToModifiedClosure
            if (maxId != default) query = query.Where(tweet => tweet.MaxID == maxId);
            if (minimumStatusId != default) query = query.Where(tweet => tweet.SinceID == minimumStatusId);
            buffer = await query.ToListAsync();
            statuses.AddRange(buffer);
            if (buffer.Any()) maxId = buffer.Min(status => status.StatusID) - 1;
        } while (buffer.Any());

        var tweets = statuses.Select(
            s => new Tweet(
                s.StatusID,
                s.User?.ScreenNameResponse ?? displayName,
                s.Text ?? string.Empty,
                s.Entities?.MediaEntities?.Any() == true
            )
        ).ToList();
        return tweets;
    }
}