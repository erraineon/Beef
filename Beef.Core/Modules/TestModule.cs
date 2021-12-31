using Discord;
using Discord.Interactions;

namespace Beef.Core.Modules;

public class TestModule : InteractionModuleBase<IInteractionContext>
{

    [SlashCommand("ping", "Responds with pong.")]
    public async Task Ping()
    {
        await RespondAsync("pong");
    }
}