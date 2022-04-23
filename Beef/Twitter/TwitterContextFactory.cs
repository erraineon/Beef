using LinqToTwitter;
using LinqToTwitter.OAuth;
using Microsoft.Extensions.Options;

namespace Beef.Twitter;

public class TwitterContextFactory : ITwitterContextFactory
{
    private readonly IOptions<TwitterOptions> _twitterOptions;
    private TwitterContext? _twitterContext;

    public TwitterContextFactory(IOptions<TwitterOptions> twitterOptions)
    {
        _twitterOptions = twitterOptions;
    }

    public async Task<TwitterContext> GetOrCreateAsync()
    {
        if (_twitterContext == null)
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
            _twitterContext = new TwitterContext(authorizer);
            var verifyResponse = await _twitterContext.Account
                .Where(acct => acct.Type == AccountType.VerifyCredentials)
                .SingleOrDefaultAsync();
            if (verifyResponse?.User is { } user) authorizer.CredentialStore.ScreenName = user.ScreenNameResponse;
        }

        return _twitterContext;
    }
}