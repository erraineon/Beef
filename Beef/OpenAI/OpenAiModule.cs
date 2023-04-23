using Beef.Core;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using RuntimeResult = Discord.Interactions.RuntimeResult;

namespace Beef.OpenAI;

public class OpenAiModule : InteractionModuleBase<IInteractionContext>
{
    private readonly IOpenAiService _openAiService;

    public OpenAiModule(IOpenAiService openAiService)
    {
        _openAiService = openAiService;
    }

    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("gen", "Complete a sentence.")]
    [RequireTrustedGuild]
    public async Task<RuntimeResult> GenerateThoughtAsync(string? prompt = default)
    {
        prompt ??= string.Empty;
        if (prompt.Length >= 512) throw new ModuleException("the prompt can be up to 512 characters long");
        var generatedText = await _openAiService.GenerateCompletionAsync(prompt);
        return new SuccessResult(generatedText);
    }

    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("chat", "Respond to a prompt.")]
    [RequireTrustedGuild]
    public async Task<RuntimeResult> GenerateChatCompletionAsync([Remainder] string prompt)
    {
        if (prompt.Length >= 512) throw new ModuleException("the prompt can be up to 512 characters long");
        var generatedText = await _openAiService.GenerateChatCompletionAsync(prompt);
        return new SuccessResult(generatedText);
    }
}