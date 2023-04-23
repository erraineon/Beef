using Discord;
using Discord.Interactions;

namespace Beef.Core.Modules;

public class UtilityModule : InteractionModuleBase<IInteractionContext>
{
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [RequireOwner]
    [SlashCommand("ping", "Responds with pong.")]
    public Task<RuntimeResult> Ping()
    {
        return Task.FromResult<RuntimeResult>(new SuccessResult("pong"));
    }

    [SlashCommand("pick", "Randomly picks one of the specified values.")]
    public Task<RuntimeResult> Pick(string options)
    {
        var tokens = options.SplitBySpaceAndQuotes().ToList();
        var random = new Random();
        return Task.FromResult<RuntimeResult>(new SuccessResult(tokens[random.Next(tokens.Count)]));
    }

    [SlashCommand("roll", "Rolls a die with the specified number of sides.")]
    public Task<RuntimeResult> Roll(int sides)
    {
        var random = new Random();
        return Task.FromResult<RuntimeResult>(new SuccessResult(random.Next(sides) + 1));
    }

    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [RequireOwner]
    [SlashCommand("echo", "Repeats the message.")]
    public Task<RuntimeResult> Echo(string message)
    {
        return Task.FromResult<RuntimeResult>(new SuccessResult(message));
    }
}