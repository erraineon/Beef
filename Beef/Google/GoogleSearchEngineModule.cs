using Beef.Core;
using Discord;
using Discord.Interactions;

namespace Beef.Google;

public class GoogleSearchEngineModule(IGoogleSearchEngine googleSearchEngine)
    : InteractionModuleBase<IInteractionContext>
{
    [SlashCommand("g", "Searches on Google.")]
    public async Task<RuntimeResult> FindWebpageAsync(string query)
    {
        var webpageLink = await googleSearchEngine.FindWebpageLinkAsync(query) ??
                          throw new ModuleException("no results");
        return new SuccessResult(webpageLink);
    }

    [SlashCommand("gi", "Searches on Google Images.")]
    public async Task<RuntimeResult> FindImageAsync(string query)
    {
        var imageLink = await googleSearchEngine.FindImageLinkAsync($"{query} -site:me.me -site:fbsbx.com") ??
                        throw new ModuleException("no results");
        return new SuccessResult(imageLink);
    }

    [SlashCommand("yt", "Searches on YouTube.")]
    public async Task<RuntimeResult> FindVideoAsync(string query)
    {
        var videoLink = await googleSearchEngine.FindWebpageLinkAsync($"{query} site:youtube.com") ??
                        throw new ModuleException("no results");
        return new SuccessResult(videoLink);
    }
}