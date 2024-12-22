using Microsoft.Extensions.Options;
using OpenAI.ObjectModels.RequestModels;

namespace Beef.OpenAi;

public class OpenAiService(
    OpenAI.Interfaces.IOpenAIService openAiService,
    IOptionsSnapshot<OpenAiOptions> openAiOptions)
    : IOpenAiService
{
    public async Task<string> GenerateChatCompletionAsync(string prompt)
    {
        var systemPrompt = openAiOptions.Value.DefaultSystemPrompt;
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
}