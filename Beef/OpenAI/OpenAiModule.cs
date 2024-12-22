using Beef.Core;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using RuntimeResult = Discord.Interactions.RuntimeResult;

namespace Beef.OpenAi;

public class OpenAiModule(IOpenAiService openAiService) : InteractionModuleBase<IInteractionContext>
{
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("chat", "Respond to a prompt.")]
    [RequireTrustedGuild]
    public async Task<RuntimeResult> GenerateChatCompletionAsync([Remainder] string prompt)
    {
        if (prompt.Length >= 512) throw new ModuleException("the prompt can be up to 512 characters long");
        var generatedText = await openAiService.GenerateChatCompletionAsync(prompt);
        return new SuccessResult(generatedText);
    }
}