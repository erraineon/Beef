using Beef.Core;
using Discord;
using Discord.Interactions;
using RuntimeResult = Discord.Interactions.RuntimeResult;

namespace Beef.TadmorMind;

public class TadmorMindModule(ITadmorMindThoughtsRepository tadmorMindThoughtsRepository)
    : InteractionModuleBase<IInteractionContext>
{
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("gen", "Say something.")]
    [RequireTrustedGuild]
    public async Task<RuntimeResult> GenerateThoughtAsync()
    {
        var generatedText = await tadmorMindThoughtsRepository.ReceiveAsync();
        return new SuccessResult(generatedText);
    }
}