namespace Beef.OpenAI;

public class OpenAiOptions
{
    public string ApiKey { get; set; } = null!;
    public string CompletionModelName { get; set; } = null!;
    public float Temperature { get; set; } = 0.75f;
    public string ChatCompletionModelName { get; set; } = "gpt-4";
    public string? DefaultSystemPrompt { get; set; }
}