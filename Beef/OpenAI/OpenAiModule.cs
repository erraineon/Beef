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
        var generatedText = await openAiService.GenerateChatCompletionAsync(Context.Guild.Id.ToString(), prompt);
        return new SuccessResult(generatedText);
    }

    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("system", "Set the system prompt.")]
    public async Task<RuntimeResult> SetSystemAsync([Remainder] string systemPrompt = "")
    {
        if (systemPrompt.Length >= 2048) throw new ModuleException("the prompt can be up to 2048 characters long");
        await openAiService.SetSystemPromptAsync(Context.Guild.Id.ToString(), systemPrompt);
        return new SuccessResult();
    }
}