namespace Beef.OpenAi;

public interface IOpenAiService
{
    Task<string> GenerateChatCompletionAsync(string contextKey, string prompt);
    Task SetSystemPromptAsync(string contextKey, string? systemPrompt);
}