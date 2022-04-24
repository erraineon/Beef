using Beef.Gpt3;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Beef;

public class RequireTrustedGuildAttribute : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        var trustedGuilds = services.GetRequiredService<IOptionsSnapshot<TrustedGuilds>>().Value;
        var result = trustedGuilds.Ids.Contains(context.Guild.Id)
            ? PreconditionResult.FromSuccess()
            : PreconditionResult.FromError("Contact the bot's owner to whitelist this chat.");
        return Task.FromResult(result);
    }
}