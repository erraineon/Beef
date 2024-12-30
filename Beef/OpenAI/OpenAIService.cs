using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;

namespace Beef.OpenAi;

public class OpenAiService(
    IOpenAIService openAiService,
    IOptionsSnapshot<OpenAiOptions> openAiOptions,
    IDistributedCache distributedCache)
    : IOpenAiService
{
    public async Task<string> GenerateChatCompletionAsync(string contextKey, string prompt)
    {
        var systemPrompt = await distributedCache.GetStringAsync(GetKey(contextKey)) ??
                           openAiOptions.Value.DefaultSystemPrompt;
        var messages = new List<ChatMessage>
        {
            new("user", prompt)
        };
        if (!string.IsNullOrWhiteSpace(systemPrompt))
            messages.Insert(0, new ChatMessage("system", systemPrompt));
        var completion = await openAiService.ChatCompletion.CreateCompletion(
            new ChatCompletionCreateRequest
            {
                Messages = messages,
                MaxTokens = openAiOptions.Value.MaxTokensToGenerate,
                Temperature = openAiOptions.Value.Temperature,
                Stop = " END"
            },
            openAiOptions.Value.ChatCompletionModelName
        );

        var result = completion.Choices.First().Message.Content;
        return result;
    }

    public async Task SetSystemPromptAsync(string contextKey, string? systemPrompt)
    {
        await distributedCache.SetStringAsync(GetKey(contextKey), systemPrompt);
    }

    private static string GetKey(string contextKey) => $"openai-system-{contextKey}";
}