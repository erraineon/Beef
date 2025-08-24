using BabyGame;
using Beef.Core;
using Beef.Furry;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Caching.Distributed;

namespace Beef.BabyGame;

public class MarriageModule(IMarriageService marriageService) : InteractionModuleBase<IInteractionContext>
{
    [SlashCommand("marry", "Offer to marry another user.")]
    public async Task<RuntimeResult> TryMarryAsync(IGuildUser user)
    {
        throw new NotImplementedException();
    }
}