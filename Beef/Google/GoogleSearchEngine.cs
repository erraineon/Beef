using Google.Apis.Customsearch.v1;
using Microsoft.Extensions.Options;

namespace Beef.Google;

public class GoogleSearchEngine : IGoogleSearchEngine
{
    private readonly CustomsearchService _googleClient;
    private readonly IOptionsSnapshot<GoogleOptions> _googleOptions;

    public GoogleSearchEngine(IOptionsSnapshot<GoogleOptions> googleOptions, CustomsearchService googleClient)
    {
        _googleOptions = googleOptions;
        _googleClient = googleClient;
    }

    public Task<string?> FindWebpageLinkAsync(string query)
    {
        return FindFirst(query, false);
    }

    public Task<string?> FindImageLinkAsync(string query)
    {
        return FindFirst(query, true);
    }

    private async Task<string?> FindFirst(string query, bool image)
    {
        var listRequest = _googleClient.Cse.List();
        listRequest.Q = query;
        listRequest.Cx = _googleOptions.Value.SearchEngineId;
        if (image) listRequest.SearchType = CseResource.ListRequest.SearchTypeEnum.Image;
        var search = await listRequest.ExecuteAsync();
        return search.Items.FirstOrDefault()?.Link;
    }
}