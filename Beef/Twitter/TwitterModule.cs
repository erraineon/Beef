using System.Text.RegularExpressions;
using Beef.Core;
using Discord;
using Discord.Interactions;

namespace Beef.Twitter;

[Group("twitter", "Twitter utilities.")]
public class TwitterModule : InteractionModuleBase<IInteractionContext>
{
    private readonly ITweetProvider _tweetProvider;

    public TwitterModule(ITweetProvider tweetProvider)
    {
        _tweetProvider = tweetProvider;
    }

    [SlashCommand("twitter", "Links a random tweet from the specified user.")]
    public async Task<RuntimeResult> GetRandomTweetAsync(
        string displayName,
        TweetType tweetType = TweetType.Any,
        string? filter = default
    )
    {
        var tweets = await _tweetProvider.GetTweetsAsync(displayName);
        var regex = filter != null
            ? new Regex(filter, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100))
            : null;
        var filteredTweets = tweets
            .Where(t => (tweetType != TweetType.Media || t.HasMedia) && regex?.IsMatch(t.Status) != false)
            .ToList();
        var tweet = !filteredTweets.Any()
            ? throw new ModuleException("no tweets that matched the filter were found")
            : filteredTweets[new Random().Next(filteredTweets.Count)];
        return CommandResult.Ok($"https://twitter.com/{tweet.AuthorName}/status/{tweet.Id}");
    }
}