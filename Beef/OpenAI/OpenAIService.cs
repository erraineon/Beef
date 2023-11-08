using Microsoft.Extensions.Options;
using OpenAI.ObjectModels.RequestModels;
using System.Text.Json.Serialization;
using System.Threading;

namespace Beef.OpenAi;

public class VisionChatMessage
{
    private readonly string _content;
    private readonly string? _url;

    [JsonPropertyName("role")]
    public string Role { get; set; }
    public VisionChatMessage(string role, string content, string? url)
    {
        Role = role;
        _content = content;
        _url = url;
    }

    [JsonPropertyName("content")]
    public List<VisionChatMessageContent> Content
    {
        get
        {
            var list = new List<VisionChatMessageContent>();
            if (_url != null) list.Add(new("image_url", _url));
            list.Add(new("text", _content));
            return list;
        }
    }
}
public class VisionChatMessageContent
{
    [JsonPropertyName("type")]
    public string Type { get; }
    [JsonPropertyName("text")]
    public string? Text { get; }
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; }

    public VisionChatMessageContent(string type, string value)
    {
        Type = type;
        if (type == "image_url") ImageUrl = value;
        else Text = value;
    }
}

public class OpenAiService : IOpenAiService
{
    private const int MaxTokensToGenerate = 512;
    private readonly IOptionsSnapshot<OpenAiOptions> _openAiOptions;
    private readonly IVisionEnabledOpenAIService _openAiService;

    public OpenAiService(
        IVisionEnabledOpenAIService openAiService,
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

    public async Task<string> GenerateChatCompletionAsync(string prompt, string? imageUrl)
    {
        var systemPrompt = _openAiOptions.Value.DefaultSystemPrompt;
        var messages = new List<VisionChatMessage>
        {
            new VisionChatMessage("user", prompt, imageUrl)
        };
        if (!string.IsNullOrWhiteSpace(systemPrompt))
            messages.Insert(0, new VisionChatMessage("system", systemPrompt, null));
        var completion = await _openAiService.CreateVisionCompletion(
            new VisionChatCompletionCreateRequest()
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