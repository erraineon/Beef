using Microsoft.Extensions.Options;
using OpenAI.ObjectModels.RequestModels;

namespace Beef.OpenAi;

public class OpenAiService : IOpenAiService
{
    private const int MaxTokensToGenerate = 512;
    private readonly IOptionsSnapshot<OpenAiOptions> _openAiOptions;
    private readonly OpenAI.Interfaces.IOpenAIService _openAiService;

    public OpenAiService(
        OpenAI.Interfaces.IOpenAIService openAiService,
        IOptionsSnapshot<OpenAiOptions> openAiOptions
    )
    {
        _openAiService = openAiService;
        _openAiOptions = openAiOptions;
    }

    public async Task<string> GenerateCompletionAsync(string prompt)
    {
        var completion = await _openAiService.Completions.CreateCompletion(
            new CompletionCreateRequest
            {
                Prompt = prompt,
                MaxTokens = MaxTokensToGenerate,
                Temperature = _openAiOptions.Value.Temperature,
                Stop = " END"
            },
            _openAiOptions.Value.CompletionModelName
        );

        var result = $"{prompt}{completion.Choices.First().Text}";
        return result;
    }


    public async Task<string> GenerateChatCompletionAsync(string prompt)
    {
        var systemPrompt = _openAiOptions.Value.DefaultSystemPrompt;
        var messages = new List<ChatMessage>
        {
            new("user", prompt)
        };
        if (!string.IsNullOrWhiteSpace(systemPrompt))
            messages.Insert(0, new ChatMessage("system", systemPrompt));
        var completion = await _openAiService.ChatCompletion.CreateCompletion(
            new ChatCompletionCreateRequest
            {
                Messages = messages,
                MaxTokens = MaxTokensToGenerate,
                Temperature = _openAiOptions.Value.Temperature,
                Stop = " END"
            },
            _openAiOptions.Value.ChatCompletionModelName
        );

        var result = completion.Choices.First().Message.Content;
        return result;
    }
}