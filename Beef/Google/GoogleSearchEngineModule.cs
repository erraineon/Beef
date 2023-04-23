using Beef.Core;
using Discord;
using Discord.Interactions;

namespace Beef.Google;

public class GoogleSearchEngineModule : InteractionModuleBase<IInteractionContext>
{
    private readonly IGoogleSearchEngine _googleSearchEngine;

    public GoogleSearchEngineModule(IGoogleSearchEngine googleSearchEngine)
    {
        _googleSearchEngine = googleSearchEngine;
    }

    [SlashCommand("g", "Searches on Google.")]
    public async Task<RuntimeResult> FindWebpageAsync(string query)
    {
        var webpageLink = await _googleSearchEngine.FindWebpageLinkAsync(query) ??
            throw new ModuleException("no results");
        return SuccessResult.Ok(webpageLink);
    }

    [SlashCommand("gi", "Searches on Google Images.")]
    public async Task<RuntimeResult> FindImageAsync(string query)
    {
        var imageLink = await _googleSearchEngine.FindImageLinkAsync($"{query} -site:me.me") ??
            throw new ModuleException("no results");
        return SuccessResult.Ok(imageLink);
    }

    [SlashCommand("yt", "Searches on YouTube.")]
    public async Task<RuntimeResult> FindVideoAsync(string query)
    {
        var videoLink = await _googleSearchEngine.FindWebpageLinkAsync($"{query} site:youtube.com") ??
            throw new ModuleException("no results");
        return SuccessResult.Ok(videoLink);
    }
}