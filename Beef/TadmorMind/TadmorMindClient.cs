using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;

namespace Beef.TadmorMind;

public class TadmorMindClient(
    IOptionsSnapshot<TadmorMindOptions> tadmorMindOptions,
    IHttpClientFactory httpClientFactory)
    : ITadmorMindClient
{
    public async Task<List<string>> GenerateEntriesAsync()
    {
        var text = await QueryGenerationAsync();
        const string eofDelimiter = "<|endoftext|>";
        var entriesStartIndex = Math.Max(0, text.IndexOf(eofDelimiter, StringComparison.Ordinal));
        var entries = text[entriesStartIndex..]
            .Split(eofDelimiter, StringSplitOptions.RemoveEmptyEntries)[..^1]
            .Select(e => e.Trim('\r', '\n', ' '))
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Distinct()
            .OrderBy(_ => Random.Shared.Next())
            .ToList();
        return entries;
    }

    private async Task<string> QueryGenerationAsync()
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromMinutes(10);
        var json = await httpClient.GetFromJsonAsync<JsonObject>(new Uri(tadmorMindOptions.Value.ServiceAddress));
        var text = json["text"].GetValue<string>() ??
                   throw new InvalidOperationException("no output was received from the text generator");
        return text;
    }
}