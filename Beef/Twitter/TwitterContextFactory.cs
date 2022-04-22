using LinqToTwitter;
using LinqToTwitter.OAuth;
using Microsoft.Extensions.Options;

namespace Beef.Twitter;

public class TwitterContextFactory : ITwitterContextFactory
{
    private readonly IOptions<TwitterOptions> _twitterOptions;

    public TwitterContextFactory(IOptions<TwitterOptions> twitterOptions)
    {
        _twitterOptions = twitterOptions;
    }

    public async Task<TwitterContext> CreateAsync()
    {
        var authorizer = new SingleUserAuthorizer
        {
            CredentialStore = new InMemoryCredentialStore
            {
                ConsumerKey = _twitterOptions.Value.ConsumerKey,
                ConsumerSecret = _twitterOptions.Value.ConsumerSecret,
                OAuthToken = _twitterOptions.Value.OAuthToken,
                OAuthTokenSecret = _twitterOptions.Value.OAuthTokenSecret
            }
        };

        await authorizer.AuthorizeAsync();
        var context = new TwitterContext(authorizer);
        var verifyResponse = await context.Account
            .Where(acct => acct.Type == AccountType.VerifyCredentials)
            .SingleOrDefaultAsync();
        if (verifyResponse?.User is { } user) authorizer.CredentialStore.ScreenName = user.ScreenNameResponse;
        return context;
    }
}