namespace Beef.OpenAI;

public interface IOpenAiService{
    Task<string> GenerateCompletionAsync(string prompt);
    Task<string> GenerateChatCompletionAsync(string prompt);
}