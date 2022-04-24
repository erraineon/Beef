using Beef.Core;
using Discord;
using Discord.Interactions;

namespace Beef.Gpt3;

public class Gpt3Module : InteractionModuleBase<IInteractionContext>
{
    private readonly IGpt3Client _gpt3Client;

    public Gpt3Module(IGpt3Client gpt3Client)
    {
        _gpt3Client = gpt3Client;
    }

    [SlashCommand("gen", "Generate a thought.")]
    [RequireTrustedGuild]
    public async Task<RuntimeResult> GenerateThoughtAsync(string? prompt = default)
    {
        prompt ??= string.Empty;
        if (prompt.Length >= 512) throw new ModuleException("the prompt can be up to 512 characters long");
        var generatedText = await _gpt3Client.GenerateCompletionAsync(prompt);
        return SuccessResult.Ok(generatedText);
    }
}