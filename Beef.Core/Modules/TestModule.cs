using Discord;
using Discord.Interactions;

namespace Beef.Core.Modules;

public class TestModule : InteractionModuleBase<IInteractionContext>
{
    [DefaultPermission(false)]
    [RequireOwner]
    [SlashCommand("ping", "Responds with pong.")]
    public Task<RuntimeResult> Ping()
    {
        return Task.FromResult(CommandResult.Ok("pong"));
    }

    [SlashCommand("pick", "Randomly picks one of the specified values.")]
    public Task<RuntimeResult> Pick(string options)
    {
        var tokens = options.SplitBySpaceAndQuotes().ToList();
        var random = new Random();
        return Task.FromResult(CommandResult.Ok(tokens[random.Next(tokens.Count)]));
    }

    [SlashCommand("roll", "Rolls a die with the specified number of sides.")]
    public Task<RuntimeResult> Pick(int sides)
    {
        var random = new Random();
        return Task.FromResult(CommandResult.Ok(random.Next(sides) + 1));
    }
}