namespace Beef.OpenAi;

public class OpenAiOptions
{
    public required string ApiKey { get; set; }
    public required string CompletionModelName { get; set; }
    public float Temperature { get; set; } = 0.75f;
    public string ChatCompletionModelName { get; set; } = "gpt-4";
    public string? DefaultSystemPrompt { get; set; }
}