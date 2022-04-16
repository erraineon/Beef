using Discord;
using Discord.Interactions;

namespace Beef.Core.Modules;

public class PermsModule : InteractionModuleBase<IInteractionContext>
{
    [DefaultPermission(false)]
    [SlashCommand("perms", "Responds if you have permission.", true)]
    public async Task<RuntimeResult> Perms()
    {
        return CommandResult.Ok("Ok");
    }
}