using Discord;
using Discord.Interactions;

namespace Beef.Core.Modules;

public class TestModule2 : InteractionModuleBase<IInteractionContext>
{
    [SlashCommand("echo", "Echoes your message.")]
    public async Task<RuntimeResult> Echo(string value)
    {
        return CommandResult.Ok($"{value}!");
    }

    [Group("group", "Group")]
    public class TestModule2Group : InteractionModuleBase<IInteractionContext>
    {
        [SlashCommand("echo2", "Echoes your message with a question mark.")]
        public async Task<RuntimeResult> Echo(string value)
        {
            return CommandResult.Ok($"{value}?");
        }
    }
}