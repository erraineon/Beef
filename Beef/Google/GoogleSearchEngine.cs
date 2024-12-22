using Google.Apis.Customsearch.v1;
using Microsoft.Extensions.Options;

namespace Beef.Google;

public class GoogleSearchEngine(IOptionsSnapshot<GoogleOptions> googleOptions, CustomsearchService googleClient)
    : IGoogleSearchEngine
{
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
        var listRequest = googleClient.Cse.List();
        listRequest.Q = query;
        listRequest.Cx = googleOptions.Value.SearchEngineId;
        if (image) listRequest.SearchType = CseResource.ListRequest.SearchTypeEnum.Image;
        var search = await listRequest.ExecuteAsync();
        return search.Items.FirstOrDefault()?.Link;
    }
}